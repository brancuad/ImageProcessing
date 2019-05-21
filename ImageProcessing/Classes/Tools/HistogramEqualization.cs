using ImageProcessing.DataObjects;
using System.Collections.Generic;
using System.Linq;
using System;

namespace ImageProcessing.Tools
{
	/// <summary>
	/// Implement Image Enhancement Functionality
	/// </summary>
	public static class HistogramEqualization
	{
		/// <summary>
		/// List of implemented Histogram Equalization methods
		/// </summary>
		public enum EqualizationMethod
		{
			Dynamic,
			ContrastLimitedAdaptive,
			Traditional
		}

		/// <summary>
		/// Return the pixels making an enhanced image
		/// </summary>
		/// <param name="image"></param>
		/// <param name="percentage"></param>
		/// <returns></returns>
		public static List<Pixel> Enhance(Image image, double percentage, EqualizationMethod method = EqualizationMethod.Traditional)
		{
			List<Pixel> output = new List<Pixel>();

			switch (method)
			{
				case EqualizationMethod.Dynamic:
					output = DHE(image, percentage);
					break;
				case EqualizationMethod.ContrastLimitedAdaptive:
					output = CLAHE(image, percentage);
					break;
				case EqualizationMethod.Traditional:
				default:
					output = Equalization(image.Pixels, percentage);
					break;
			}

			return output;
		}

		/// <summary>
		/// Traditional Histogram Equalization.
		/// </summary>
		/// <param name="image"></param>
		/// <param name="percentage"></param>
		/// <returns></returns>
		private static List<Pixel> Equalization(List<Pixel> original, double percentage)
		{
			// Histogram containing pixel luminances
			List<double> lum_list = new List<double>();
			List<Pixel> pixels = original;
			int[] histogram = new int[101];

			for (int i = 0; i < pixels.Count; i++)
			{
				Pixel pixel = pixels[i];
				double lum = pixel.Color.L;

				lum_list.Add(lum);
				histogram[(int)Math.Floor(lum)]++;
			}

			// Prevent division by 0
			if (lum_list.Max() == 0)
			{
				return pixels;
			}

			// Get PDF
			double[] pdf = new double[histogram.Length];
			double[] cdf = new double[histogram.Length];
			double cum = 0;
			for (int i = 0; i < histogram.Length; i++)
			{
				pdf[i] = (double)histogram[i] / lum_list.Count;
				cum = pdf[i] + cum;
				cdf[i] = cum;
			}

			List<double> CDF = cdf.ToList();

			var max = CDF.Max();
			var min = CDF.Min();
			for (int i = 0; i < histogram.Length; i++)
			{
				CDF[i] = ((CDF[i] - min) / (max - min)) * 100;
			}
			
			// Set new luminance
			for (int i = 0; i < pixels.Count; i++)
			{
				double newLum = CDF[(int)Math.Floor(lum_list[i])];

				double lumValue = (newLum * percentage) + (pixels[i].Color.L * (1 - percentage));
				pixels[i].Color = new Color(L: lumValue, a: pixels[i].Color.a, b: pixels[i].Color.b);
			}

			return pixels;
		}

		/// <summary>
		/// Perform Dynamic Histogram Equalization
		/// </summary>
		/// <param name="image"></param>
		/// <param name="percentage"></param>
		/// <returns></returns>
		private static List<Pixel> DHE(Image image, double percentage)
		{
			/**
			 * Algorithm:
			 * 1. Get image histogram
			 * 2. Find local minima in histogram
			 * 3. Divide image histogram based on local minima
			 * 4. Assign specific gray levels to each partition
			 * 5. On each partition, HE is applied
			 */

			// 1. Get histogram of L in Lab color space


			return new List<Pixel>();

		}

		/// <summary>
		/// Perform Contrast Limited Adaptive Histogram Equalization
		/// </summary>
		/// <param name="image"></param>
		/// <param name="percentage"></param>
		/// <returns></returns>
		private static List<Pixel> CLAHE(Image image, double percentage)
		{
			/**
			 * Algorithm:
			 * 1. Calcluate grid size based on image dimension
			 * 2. Start from top left corner to find grid points
			 * 3. For each grid point
				* Calcluate histogram with area equal to window size
				* Clip histogram above clip limit and use to find CDF
			 * 4. For each pixel
				* Find 4 neighbor grid point for pixel and, based on CDF, 
				  find mapping of pixel at 4 grid points 
			 * 5. Apply Interpolation among these values to get mapping
			 * 6. Map intensity to output image within min-max range
			 */

			return new List<Pixel>();
		}
	}
}
