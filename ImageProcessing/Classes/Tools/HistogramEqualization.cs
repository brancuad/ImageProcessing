using ImageProcessing.DataObjects;

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
		}

		/// <summary>
		/// Return the pixels making an enhanced image
		/// </summary>
		/// <param name="image"></param>
		/// <param name="percentage"></param>
		/// <returns></returns>
		public static Image Enhance(Image image, double percentage, EqualizationMethod method = EqualizationMethod.Dynamic)
		{
			Image output = new Image();

			switch (method)
			{
				case EqualizationMethod.Dynamic:
					output = DHE(image, percentage);
					break;
				case EqualizationMethod.ContrastLimitedAdaptive:
					output = CLAHE(image, percentage);
					break;
				default:
					output = DHE(image, percentage);
					break;
			}

			return output;
		}

		/// <summary>
		/// Perform Dynamic Histogram Equalization
		/// </summary>
		/// <param name="image"></param>
		/// <param name="percentage"></param>
		/// <returns></returns>
		private static Image DHE(Image image, double percentage)
		{
			/**
			 * Algorithm:
			 * 1. Get image histogram
			 * 2. Find local minima in histogram
			 * 3. Divide image histogram based on local minima
			 * 4. Assign specific gray levels to each partition
			 * 5. On each partition, HE is applied
			 */

			return new Image();

		}

		/// <summary>
		/// Perform Contrast Limited Adaptive Histogram Equalization
		/// </summary>
		/// <param name="image"></param>
		/// <param name="percentage"></param>
		/// <returns></returns>
		private static Image CLAHE(Image image, double percentage)
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

			return new Image();

		}
	}
}
