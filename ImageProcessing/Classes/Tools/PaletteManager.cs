using ImageProcessing.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImageProcessing.Tools
{
	/// <summary>
	/// Handles Palette extraction and manipulation
	/// </summary>
	public static class PaletteManager
	{
		/// <summary>
		/// The PaletteSize (k) used
		/// </summary>
		public static int PaletteSize = 5;

		/// <summary>
		/// The number of bins in each histogram dimension
		/// </summary>
		public static int HistogramBinCount = 16;

		private struct KMeansData
		{
			public List<Color> means;
			public List<Color> centroids;
		}

		/// <summary>
		/// Get the palette of an image
		/// </summary>
		/// <param name="image"></param>
		public static List<Color> GetPalette(Image image, int paletteSize)
		{
			PaletteManager.PaletteSize = paletteSize;

			// Get data for kmeans clustering
			KMeansData data = GetKMeansData(image.Pixels.Select(p => p.Color).ToList());

			// Format color data for KMeans (using Lab color space)
			double[][] formattedData = FormatData(data.means);

			// Perform KMeans w/additional black datapoint
			int[] centers = KMeans.Cluster(formattedData, PaletteSize + 1);

			// Get palette values
			List<Color> palette = new List<Color>();

			foreach (int index in centers)
			{
				palette.Add(data.means[index]);
			}

			// Sort palette by luminance
			palette = palette.OrderBy(p => p.L).ToList();

			// Take all actual palette members
			palette = palette.Take(PaletteSize).ToList();

			return palette;
		}

		/// <summary>
		/// Get the RBF models for a color palette
		/// </summary>
		/// <param name="palette"></param>
		/// <returns></returns>
		public static List<alglib.rbfmodel> GetWeights(List<Color> palette)
		{
			List<alglib.rbfmodel> RBFs = new List<alglib.rbfmodel>();

			// TOOD: Do RBF for each member of palette
			if (palette.Count != PaletteSize)
			{
				return RBFs;
			}

			double[,] xy = new double[PaletteSize, 4];

			for (int i = 0; i < palette.Count; i++)
			{
				Color color = palette[i];
				xy[i, 0] = color.L;
				xy[i, 1] = color.a;
				xy[i, 2] = color.b;
				xy[i, 3] = 0.0;
			}

			for (int i = 0; i < PaletteSize; i++)
			{
				alglib.rbfcreate(3, 1, out alglib.rbfmodel model);

				// Set the appropriate value as 1.0
				for (int j = 0; j < PaletteSize; j++)
				{
					if (j == i)
					{
						xy[j, 3] = 1.0;
					}
					else
					{
						xy[j, 3] = 0.0;
					}
				}

				alglib.rbfsetpoints(model, xy);
				alglib.rbfsetalgohierarchical(model, 1.0, 3, 0.0);
				alglib.rbfbuildmodel(model, out alglib.rbfreport rep);

				var v = alglib.rbfcalc3(model, palette[i].L, palette[i].a, palette[i].b);
				RBFs.Add(model);
			}

			return RBFs;
		}

		/// <summary>
		/// Get the data to input into KMeans Algorithm
		/// </summary>
		/// <param name="pixels"></param>
		/// <returns></returns>
		private static KMeansData GetKMeansData(List<Color> pixels)
		{
			// Construct 3D List for 3D histogram of colors in image
			List<List<List<List<Color>>>> histogram = new List<List<List<List<Color>>>>();

			// Initialize histogram with 16^3 bins
			for (int i = 0; i < HistogramBinCount; i++)
			{
				List<List<List<Color>>> dim1 = new List<List<List<Color>>>();

				for (int j = 0; j < HistogramBinCount; j++)
				{
					List<List<Color>> dim2 = new List<List<Color>>();

					for (int k = 0; k < HistogramBinCount; k++)
					{
						List<Color> colors = new List<Color>();
						dim2.Add(colors);
					}

					dim1.Add(dim2);
				}

				histogram.Add(dim1);
			}

			// Separate pixels into bins for each color channel
			foreach (Color pixel in pixels)
			{
				int rBin = pixel.R / HistogramBinCount;
				int gBin = pixel.G / HistogramBinCount;
				int bBin = pixel.B / HistogramBinCount;

				histogram[rBin][gBin][bBin].Add(pixel);
			}

			// Get means of all bins in histogram
			List<Color> means = GetHistogramMeans(histogram);

			// Get the k largest bins
			List<int> largestBins = GetLargestBins(histogram);

			List<Color> centroids = new List<Color>();

			// Get the means of the largest bins
			foreach (int bin in largestBins)
			{
				centroids.Add(means[bin]);
			}

			// Add black to the centroids
			centroids.Add(Color.Black);

			// Return KMeansData containing means and centroids
			return new KMeansData()
			{
				means = means,
				centroids = centroids
			};
		}

		/// <summary>
		/// Get the mean color of a histogram bin
		/// </summary>
		/// <param name="histogram"></param>
		/// <returns></returns>
		private static Color GetMeanColor(List<Color> bin)
		{
			// Average Lab values
			double lTotal = 0;
			double aTotal = 0;
			double bTotal = 0;

			for (int i = 0; i < bin.Count; i++)
			{
				Color c = bin[i];

				lTotal += c.L;
				aTotal += c.a;
				bTotal += c.b;
			}

			double lMean = 0;
			double aMean = 0;
			double bMean = 0;

			if (bin.Count > 0)
			{
				lMean = lTotal / bin.Count;
				aMean = aTotal / bin.Count;
				bMean = bTotal / bin.Count;
			}

			// Get Color from Lab value
			return Color.GetColorFromLab(lMean, aMean, bMean);
		}

		/// <summary>
		/// Get the mean colors of each bin in the histogram
		/// </summary>
		/// <param name="histogram"></param>
		/// <returns></returns>
		private static List<Color> GetHistogramMeans(List<List<List<List<Color>>>> histogram)
		{
			List<Color> means = new List<Color>();

			// Iterate through every bin and get its mean
			foreach (List<List<List<Color>>> D2 in histogram)
			{
				foreach (List<List<Color>> D1 in D2)
				{
					foreach (List<Color> bin in D1)
					{
						// Calcluate Mean Color for bin
						means.Add(GetMeanColor(bin));
					}
				}
			}

			return means;
		}

		/// <summary>
		/// Recursive function to find k largest bins
		/// </summary>
		/// <param name="histogram"></param>
		/// <param name="excluding"></param>
		/// <returns></returns>
		private static List<int> GetLargestBins(List<List<List<List<Color>>>> histogram, List<int> excluding = null)
		{
			List<int> largestBins = new List<int>();

			if (excluding == null)
				excluding = new List<int>();

			// No more searching needed
			if (excluding.Count >= PaletteSize)
				return new List<int>();

			// Set values to surpass
			int currentLargest = int.MinValue;
			int currentMaxSize = int.MinValue;

			// Find the largets bin and return its mean
			for (int i = 0; i < histogram.Count; i++)
			{
				for (int j = 0; j < histogram.Count; j++)
				{
					for (int k = 0; k < histogram.Count; k++)
					{
						// Get the index in the flattened list of means
						int bindex = k + (j * 16) + (i * (int)Math.Pow(16, 2));

						// Find max size among non-excluded bins
						if (histogram[i][j][k].Count > currentMaxSize && !excluding.Contains(bindex))
						{
							currentLargest = bindex;
							currentMaxSize = histogram[i][j][k].Count;
						}
					}
				}
			}

			// Set largest bins
			largestBins.Add(currentLargest);

			// Return along with next largest
			return largestBins.Concat(GetLargestBins(histogram, excluding: largestBins)) as List<int>;
		}

		/// <summary>
		/// Format KMeansData for use in algorithm. With Lab color space.
		/// </summary>
		/// <param name=""></param>
		/// <returns></returns>
		private static double[][] FormatData(List<Color> data)
		{
			double[][] formatted = new double[data.Count][];

			foreach (Color color in data)
			{
				formatted.Append(new double[] { color.L, color.a, color.b });
			}

			return formatted;
		}
	}
}
