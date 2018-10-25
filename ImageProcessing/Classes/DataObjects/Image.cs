using System.Collections.Generic;
using ImageProcessing.Tools;

namespace ImageProcessing.DataObjects
{
	/// <summary>
	/// Represents image
	/// </summary>
	public class Image
	{
		/// <summary>
		/// Array of pixels in image
		/// </summary>
		public List<Pixel> Pixels { get; set; } = new List<Pixel>();

		/// <summary>
		/// Array of colors in image palette
		/// </summary>
		public List<Color> Palette { get; set; } = new List<Color>();

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
				Color color = new Color(pixelArray[i], pixelArray[i+1], pixelArray[i+2], pixelArray[i+3]);
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
			List<Pixel> pixels = GetPixelsFromArray(pixelArray);
			return new Image(pixels);
		}

		/// <summary>
		/// Set the palette of the image
		/// </summary>
		public void SetPalette()
		{
			this.Palette = PaletteManager.GetPalette(this);
		}

	}
}
