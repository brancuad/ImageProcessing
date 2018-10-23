using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageProcessing.DataObjects
{
	/// <summary>
	/// Represents a pixel position
	/// </summary>
	public class Position
	{
		int X = 0;
		int Y = 0;

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

			Position other = (Position)obj;

			if (this.X.Equals(other.X)
				&& this.Y.Equals(other.Y)) {
				return true;
			}

			return false;
		}
	}
}
