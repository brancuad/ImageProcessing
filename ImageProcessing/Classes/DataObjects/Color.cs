using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System;

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

		public static Color Black = new Color();

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
			if (this == Color.Black)
				SetLab();
		}

		/// <summary>
		/// Calculate Color in Lab color space. Derived from color.js.
		/// </summary>
		/// <param name="color"></param>
		private static Tuple<double, double, double> CalcluateLab(Color color)
		{
			double r = color.R / 255;
			double g = color.G / 255;
			double b = color.B / 255;

			double x, y, z;

			r = (r > 0.04045) ? Math.Pow((r + 0.055) / 1.055, 2.4) : r / 12.92;
			g = (g > 0.04045) ? Math.Pow((g + 0.055) / 1.055, 2.4) : g / 12.92;
			b = (b > 0.04045) ? Math.Pow((b + 0.055) / 1.055, 2.4) : b / 12.92;

			x = (r * 0.4124 + g * 0.3576 + b * 0.1805) / 0.95047;
			y = (r * 0.2126 + g * 0.7152 + b * 0.0722) / 1.00000;
			z = (r * 0.0193 + g * 0.1192 + b * 0.9505) / 1.08883;

			x = (x > 0.008856) ? Math.Pow(x, 1 / 3) : (7.787 * x) + 16 / 116;
			y = (y > 0.008856) ? Math.Pow(y, 1 / 3) : (7.787 * y) + 16 / 116;
			z = (z > 0.008856) ? Math.Pow(z, 1 / 3) : (7.787 * z) + 16 / 116;

			return new Tuple<double, double, double>((116 * y) - 16, 500 * (x - y), 200 * (y - z));
		}

		/// <summary>
		/// Get a Color object from Lab values
		/// </summary>
		/// <param name="L"></param>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Color GetColorFromLab(double L, double a, double b)
		{
			double y = (L + 16) / 116;
			double x = a / 500 + y;
			double z = y - b / 200;
			double R, G, B;

			x = 0.95047 * ((x * x * x > 0.008856) ? x * x * x : (x - 16 / 116) / 7.787);
			y = 1.00000 * ((y * y * y > 0.008856) ? y * y * y : (y - 16 / 116) / 7.787);
			z = 1.08883 * ((z * z * z > 0.008856) ? z * z * z : (z - 16 / 116) / 7.787);

			R = x * 3.2406 + y * -1.5372 + z * -0.4986;
			G = x * -0.9689 + y * 1.8758 + z * 0.0415;
			B = x * 0.0557 + y * -0.2040 + z * 1.0570;

			R = (R > 0.0031308) ? (1.055 * Math.Pow(R, 1 / 2.4) - 0.055) : 12.92 * R;
			G = (G > 0.0031308) ? (1.055 * Math.Pow(G, 1 / 2.4) - 0.055) : 12.92 * G;
			B = (B > 0.0031308) ? (1.055 * Math.Pow(B, 1 / 2.4) - 0.055) : 12.92 * B;

			R = Math.Max(0, Math.Min(1, R)) * 255;
			G = Math.Max(0, Math.Min(1, G)) * 255;
			B = Math.Max(0, Math.Min(1, B)) * 255;

			return new Color((int)Math.Round(R), (int)Math.Round(G), (int)Math.Round(B));
		}

		/// <summary>
		/// Check if Color is out of Lab color gamut
		/// </summary>
		/// <returns></returns>
		public bool OutOfGamut()
		{
			if ((L >= 100 || L <= 0) ||
				(a >= 128 || a <= -128) ||
				(b >= 128 || b <= -128))
			{
				return true;
			}

			// Check if all bounds have been reached
			if ((R >= 255 || R <= 0) &&
				(G >= 255 || G <= 0) &&
				(B >= 255 || B <= 0))
			{
				return true;
			}

			bool outOfBounds = false;

			for (int i = 0; i < 3; i++)
			{
				int val = 0;

				switch (i)
				{
					case 0:
						val = R;
						break;
					case 1:
						val = G;
						break;
					case 2:
						val = B;
						break;
					default:
						val = R;
						break;
				}

				if (val < 0 || val > 255)
				{
					outOfBounds = true;
					break;
				}
			}

			return outOfBounds;
		}

		/// <summary>
		/// Return Vector in RGB.
		/// </summary>
		/// <param name="lab">True to return Lab vector</param>
		/// <returns></returns>
		public Vector<double> ToVector(bool Lab = false)
		{
			Vector<double> vec = DenseVector.OfArray(new double[] { R, G, B });

			if (Lab)
			{
				vec = DenseVector.OfArray(new double[] { L, a, b });
			}

			return vec;
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
