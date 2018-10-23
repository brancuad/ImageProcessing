using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageProcessing.DataObjects
{
	/// <summary>
	/// Represents a pixel of an image
	/// </summary>
	public class Pixel
	{
		Position position;
		Color color;

		/// <summary>
		/// Default constructor
		/// </summary>
		public Pixel() : this(new Position(), new Color())
		{

		}

		/// <summary>
		/// Position and color constructor
		/// </summary>
		/// <param name="position"></param>
		/// <param name="color"></param>
		public Pixel(Position position, Color color)
		{
			this.position = position;
			this.color = color;
		}

	}
}
