using ImageProcessing.Tools;
using System.Collections.Generic;

namespace ImageProcessing.DataObjects
{
	/// <summary>
	/// Represents image
	/// </summary>
	public class Image
	{
		public static Dictionary<string, Image> SavedImages = new Dictionary<string, Image>();

		public static Image CurrentImage;

		/// <summary>
		/// List of editted versions of this image
		/// </summary>
		public List<Image> Edits { get; set; } = new List<Image>();

		/// <summary>
		/// Keeps copy of original pixels in image
		/// </summary>
		public List<Pixel> OriginalPixels { get; set; } = new List<Pixel>();

		/// <summary>
		/// Array of pixels in image
		/// </summary>
		public List<Pixel> Pixels { get; set; } = new List<Pixel>();

		/// <summary>
		/// Array of colors in image palette
		/// </summary>
		public List<Color> Palette { get; set; } = new List<Color>();

		/// <summary>
		/// List of RBF models for the image palette
		/// </summary>
		public List<alglib.rbfmodel> RBFs { get; set; } = new List<alglib.rbfmodel>();

		/// <summary>
		/// List of Segments in the image
		/// </summary>
		public List<Segment> Segments { get; set; } = new List<Segment>();

		/// <summary>
		/// Default constructor
		/// </summary>
		public Image() : this(new List<Pixel>())
		{

		}

		/// <summary>
		/// Construct Image object from list of Pixels
		/// </summary>
		/// <param name="pixels"></param>
		public Image(List<Pixel> pixels)
		{
			this.Pixels = pixels;
			OriginalPixels = new List<Pixel>(this.Pixels);
		}

		/// <summary>
		/// Get pixels using array from ajax
		/// </summary>
		/// <param name="pixelArray"></param>
		/// <returns></returns>
		public static List<Pixel> GetPixelsFromArray(List<int> pixelArray, int height = -1, int width = -1)
		{
			List<Pixel> pixels = new List<Pixel>();

			Position positionCounter = new Position();

			for (int i = 0; i < pixelArray.Count; i += 4)
			{
				Color color = new Color(pixelArray[i], pixelArray[i + 1], pixelArray[i + 2], pixelArray[i + 3]);
				Position position = null;

				if (height >= 0 && width >= 0)
				{
					position = new Position(positionCounter.X, positionCounter.Y);
				}
				else
				{
					position = new Position();
				}

				// Add pixel to array
				pixels.Add(new Pixel(position, color));

				// increment position counter
				positionCounter.X++;

				if (positionCounter.X >= width)
				{
					// move to next row
					positionCounter.Y++;
					positionCounter.X = 0;
				}

				// Out of image bounds
				if (positionCounter.Y >= height)
				{
					break;
				}
			}

			return pixels;
		}

		/// <summary>
		/// Get Image object using array from ajax
		/// </summary>
		/// <param name="pixelArray"></param>
		/// <returns></returns>
		public static Image GetImageFromArray(List<int> pixelArray, int height = -1, int width = -1)
		{
			List<Pixel> pixels = GetPixelsFromArray(pixelArray, height, width);
			return new Image(pixels);
		}

		/// <summary>
		/// Set the palette of the image
		/// </summary>
		public void SetPalette(int paletteSize)
		{
			this.Palette = PaletteManager.GetPalette(this, paletteSize);
			this.RBFs = PaletteManager.GetWeights(this.Palette);
		}

		/// <summary>
		/// Perform Palette-based Color Transfer
		/// </summary>
		/// <param name="newPalette"></param>
		public void TransferPalette(List<Color> newPalette)
		{
			this.Pixels = ColorTransfer.Transfer(this, newPalette);
			this.Edits.Add(new Image(this.Pixels));
		}

		/// <summary>
		/// Perform Image Equalization
		/// </summary>
		/// <param name="weight"></param>
		public void Enhance(double weight)
		{
			this.Pixels = HistogramEqualization.Enhance(this, weight);
			this.Edits.Add(new Image(this.Pixels));
		}

		/// <summary>
		/// Get the array for the canvas from the image
		/// </summary>
		/// <returns></returns>
		public List<int> GetPixelArray(bool Original = false)
		{
			List<int> pixelArray = new List<int>();

			var Pixels = this.Pixels;

			if (Original)
			{
				Pixels = this.OriginalPixels;
			}

			foreach (Pixel pixel in Pixels)
			{
				pixelArray.Add(pixel.Color.R);
				pixelArray.Add(pixel.Color.G);
				pixelArray.Add(pixel.Color.B);
				pixelArray.Add(pixel.Color.A);
			}

			return pixelArray;
		}

	}
}
