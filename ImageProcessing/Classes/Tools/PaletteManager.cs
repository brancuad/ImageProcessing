using ImageProcessing.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImageProcessing.Tools
{
	public static class PaletteManager
	{
		/// <summary>
		/// The PaletteSize (k) used
		/// </summary>
		public static int PaletteSize = 5;

		private struct KMeansData
		{
			public List<Color> means;
			public List<Color> centroids;
		}

		/// <summary>
		/// Get the palette of an image
		/// </summary>
		/// <param name="image"></param>
		public static List<Color> GetPalette(Image image)
		{
			// Get data for kmeans clustering
			KMeansData data = GetKMeansData(image.Pixels.Select(p => p.Color).ToList());

			// Format color data for KMeans (using Lab color space)
			double[][] formattedData = FormatData(data.means);

			// Perform KMeans w/additional black datapoint
			int[] centers = KMeans.Cluster(formattedData, PaletteSize + 1);

			// Get palette values
			List<Color> palette = new List<Color>();

			foreach(int index in centers)
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
		/// Get the data to input into KMeans Algorithm
		/// </summary>
		/// <param name="pixels"></param>
		/// <returns></returns>
		private static KMeansData GetKMeansData(List<Color> pixels)
		{
			// Construct 3D List for 3D histogram of colors in image
			List<List<List<Color[]>>> histogram = new List<List<List<Color[]>>>();

			// Separate pixels into bins for each color channel
			foreach (Color pixel in pixels)
			{
				int rBin = pixel.R / 16;
				int gBin = pixel.G / 16;
				int bBin = pixel.B / 16;

				histogram[rBin][gBin][bBin].Append(pixel);
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
		private static Color GetMeanColor(Color[] bin)
		{
			// Average Lab values
			double lTotal = 0;
			double aTotal = 0;
			double bTotal = 0;
			
			for (int i = 0; i < bin.Length; i++)
			{
				Color c = bin[i];

				lTotal += c.L;
				aTotal += c.a;
				bTotal += c.b;
			}

			double lMean = lTotal / bin.Length;
			double aMean = aTotal / bin.Length;
			double bMean = bTotal / bin.Length;

			// Get Color from Lab value
			return Color.GetColorFromLab(lMean, aMean, bMean);
		}

		/// <summary>
		/// Get the mean colors of each bin in the histogram
		/// </summary>
		/// <param name="histogram"></param>
		/// <returns></returns>
		private static List<Color> GetHistogramMeans(List<List<List<Color[]>>> histogram)
		{
			List<Color> means = new List<Color>();

			// Iterate through every bin and get its mean
			foreach (List<List<Color[]>> D2 in histogram)
			{
				foreach (List<Color[]> D1 in D2)
				{
					foreach (Color[] bin in D1)
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
		private static List<int> GetLargestBins(List<List<List<Color[]>>> histogram, List<int> excluding = null)
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
						if (histogram[i][j][k].Length > currentMaxSize && !excluding.Contains(bindex))
						{
							currentLargest = bindex;
							currentMaxSize = histogram[i][j][k].Length;
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
