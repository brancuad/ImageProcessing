using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageProcessing.DataObjects
{
	/// <summary>
	/// Represents a pixel of an image
	/// </summary>
	public class Pixel : IEquatable<Pixel>
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

		/// <summary>
		/// Equals override
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if (obj == null || !this.GetType().Equals(obj.GetType()))
			{
				return false;
			}

			return this.Equals(obj as Pixel);
		}

		/// <summary>
		/// Equals override
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(Pixel other)
		{
			if (this.Position.Equals(other.Position)
				&& this.Color.Equals(other.Color))
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Get hash of object
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return HashCode.Combine(Position, Color);
		}

		/// <summary>
		/// Equals operator
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static bool operator ==(Pixel left, Pixel right)
		{
			return left.Equals(right);
		}

		/// <summary>
		/// Not equals operator
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static bool operator !=(Pixel left, Pixel right)
		{
			return !left.Equals(right);
		}
	}
}
