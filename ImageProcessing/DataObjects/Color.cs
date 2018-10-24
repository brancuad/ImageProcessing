using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageProcessing.DataObjects
{
	/// <summary>
	/// Represents the color of a pixel
	/// </summary>
	public class Color
	{
		int R = 0;
		int G = 0;
		int B = 0;
		int A = 255;

		double L = 0;
		double a = 0;
		double b = 0;

		/// <summary>
		/// Initialize with default (black) color
		/// </summary>
		public Color() : this(0, 0, 0, 0)
		{
		}

		/// <summary>
		/// Initialize with specific RGB color
		/// </summary>
		/// <param name="R"></param>
		/// <param name="G"></param>
		/// <param name="B"></param>
		/// <param name="A"></param>
		public Color(int R, int G, int B, int A = 255)
		{
			this.R = R;
			this.G = G;
			this.B = B;
			this.A = A;

			// Calcluate Lab
		}

		/// <summary>
		/// Calculate Color in Lab color space
		/// </summary>
		/// <param name="color"></param>
		public static Tuple<double, double, double> CalcluateLab(Color color)
		{
			// TODO
			return new Tuple<double, double, double>(0,0,0);
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

			Color other = (Color)obj;

			if (this.R.Equals(other.R)
				&& this.G.Equals(other.G)
				&& this.B.Equals(other.B))
			{
				return true;
			}

			return false;
		}
	}
}
