using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageProcessing.DataObjects
{
	/// <summary>
	/// Represents the color of a pixel
	/// </summary>
	public class Color : IEquatable<Color>
	{
		public int R = 0;
		public int G = 0;
		public int B = 0;
		public int A = 255;

		public double L = 0;
		public double a = 0;
		public double b = 0;

		/// <summary>
		/// Initialize with default (black) color
		/// </summary>
		public Color() : this(0, 0, 0, 255)
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
			SetLab();
		}

		/// <summary>
		/// Calculate Color in Lab color space
		/// </summary>
		/// <param name="color"></param>
		private static Tuple<double, double, double> CalcluateLab(Color color)
		{
			// TODO
			return new Tuple<double, double, double>(0,0,0);
		}

		/// <summary>
		/// Set values for Lab color space
		/// </summary>
		private void SetLab()
		{
			Tuple<double, double, double> lab = Color.CalcluateLab(this);

			this.L = lab.Item1;
			this.a = lab.Item2;
			this.b = lab.Item3;
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

			return this.Equals(obj as Color);
		}

		/// <summary>
		/// Get object hash
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return HashCode.Combine(R, G, B, A);
		}

		/// <summary>
		/// Determine equality
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(Color other)
		{
			if (this.R.Equals(other.R)
				&& this.G.Equals(other.G)
				&& this.B.Equals(other.B))
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Override equals operator
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static bool operator ==(Color left, Color right)
		{
			return left.Equals(right);
		}

		/// <summary>
		/// Override not equals operator
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		public static bool operator !=(Color left, Color right)
		{
			return !left.Equals(right);
		}
	}
}
