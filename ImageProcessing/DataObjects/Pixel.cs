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
		Position Position;
		Color Color;

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
			this.Position = position;
			this.Color = color;
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !this.GetType().Equals(obj.GetType()))
			{
				return false;
			}

			Pixel other = (Pixel)obj;

			if (this.Position.Equals(other.Position)
				&& this.Color.Equals(other.Color))
			{
				return true;
			}

			return false;
		}

	}
}
