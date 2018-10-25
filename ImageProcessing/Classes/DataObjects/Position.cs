using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageProcessing.DataObjects
{
	/// <summary>
	/// Represents a pixel position
	/// </summary>
	public class Position : IEquatable<Position>
	{
		public int X { get; set; } = 0;
		public int Y { get; set; } = 0;

		/// <summary>
		/// Initialize position with default values
		/// </summary>
		public Position() : this(0, 0)
		{

		}

		/// <summary>
		/// Initialize position with specified position
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public Position(int x, int y)
		{
			this.X = x;
			this.Y = y;
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

			return this.Equals(obj as Position);
		}

		/// <summary>
		/// Equals override
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(Position other)
		{
			if (this.X.Equals(other.X)
				&& this.Y.Equals(other.Y))
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
			return HashCode.Combine(X, Y);
		}

		/// <summary>
		/// Equals operator
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static bool operator ==(Position left, Position right)
		{
			return left.Equals(right);
		}

		/// <summary>
		/// Not equals operator
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static bool operator !=(Position left, Position right)
		{
			return !left.Equals(right);
		}
	}
}
