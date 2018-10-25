using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ImageProcessing.DataObjects;

namespace ImageProcessing.Tools
{
	public static class PaletteManager
	{
		/// <summary>
		/// Get the palette of an image
		/// </summary>
		/// <param name="image"></param>
		public static List<Color> GetPalette(Image image)
		{
			// TODO
			return new List<Color>();
		}

		/// <summary>
		/// Get the data to input into KMeans Algorithm
		/// </summary>
		/// <param name="pixels"></param>
		/// <returns></returns>
		private static List<Color> GetKMeansData(List<Color> pixels)
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




		}

		/// <summary>
		/// Get the mean color of a histogram bin
		/// </summary>
		/// <param name="histogram"></param>
		/// <returns></returns>
		private static Color GetMeanColor(Color[] bin)
		{
			// TODO
			return new Color();
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
		/// Recursive function to find 5 largest bins
		/// </summary>
		/// <param name="histogram"></param>
		/// <param name="excluding"></param>
		/// <returns></returns>
		private static List<int> GetLargestBins(List<List<List<Color[]>>> histogram, List<int> excluding = null)
		{
			if (excluding == null)
				excluding = new List<int>();

			// TODO
			return new List<int>();
		}
	}
}
