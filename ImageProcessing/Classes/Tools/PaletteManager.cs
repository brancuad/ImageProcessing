using ImageProcessing.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using Accord.MachineLearning;

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
			double[][] centroids = FormatData(data.centroids);

			// Perform KMeans w/additional black datapoint
			Accord.MachineLearning.KMeans kMeans = new Accord.MachineLearning.KMeans(PaletteSize + 1);
			kMeans.Centroids = centroids;

			var clusters = kMeans.Learn(formattedData);

			int[] labels = clusters.Decide(formattedData);

			// Get palette values
			double[][] doublePalette = kMeans.Centroids;
			List<Color> Palette = new List<Color>();

			foreach(double[] lab in doublePalette)
			{
				Palette.Add(new Color(L: lab[0], a: lab[1], b: lab[2]));
			}

			// Sort palette by luminance
			Palette = Palette.OrderBy(p => p.L).ToList();

			// Take all actual palette members
			Palette = Palette.TakeLast(PaletteSize).ToList();

			return Palette;
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

			// Add Black color to the centroids
			centroids.Add(new Color());

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
			else
			{
				return null;
			}

			// Get Color from Lab value
			return new Color(L: lMean, a: aMean, b: bMean);
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
						Color mean = GetMeanColor(bin);
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
		/// <param name="largestBins"></param>
		/// <returns></returns>
		private static List<int> GetLargestBins(List<List<List<List<Color>>>> histogram, List<int> largestBins = null)
		{
			List<int> LargestBins = largestBins;

			if (LargestBins == null)
				LargestBins = new List<int>();

			// No more searching needed
			if (LargestBins.Count >= PaletteSize)
				return LargestBins;

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
						if (histogram[i][j][k].Count > currentMaxSize && !LargestBins.Contains(bindex))
						{
							currentLargest = bindex;
							currentMaxSize = histogram[i][j][k].Count;
						}
					}
				}
			}

			// Set largest bins
			LargestBins.Add(currentLargest);

			// Return along with next largest
			return GetLargestBins(histogram, largestBins: LargestBins);
		}

		/// <summary>
		/// Format KMeansData for use in algorithm. With Lab color space.
		/// </summary>
		/// <param name=""></param>
		/// <returns></returns>
		private static double[][] FormatData(List<Color> data)
		{
			double[][] formatted = new double[0][];

			foreach (Color color in data)
			{
				if (color == null)
				{
					formatted = formatted.Append(new double[3]).ToArray();
				}
				else
				{
					formatted = formatted.Append(new double[] { color.L, color.a, color.b }).ToArray();
				}
			}

			return formatted;
		}
	}
}
