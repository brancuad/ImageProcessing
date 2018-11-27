using ImageProcessing.DataObjects;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImageProcessing.Tools
{
	/// <summary>
	/// Handles Palette-based Color Transfer
	/// </summary>
	public static class ColorTransfer
	{
		/// <summary>
		/// Perform Palette-based Color Transfer
		/// </summary>
		/// <param name="image"></param>
		/// <param name="newPalette"></param>
		public static Image Transfer(Image image, List<Color> newPalette)
		{
			// Iterate through all pixels and transfer each color
			for (int i = 0; i < image.Pixels.Count; i++)
			{
				Color color = image.Pixels[i].Color;

				Color newColor = TransferColor(image, color, newPalette);

				image.Pixels[i].Color = color;
			}

			return image;
		}

		/// <summary>
		/// Transfer color to its new color
		/// </summary>
		/// <param name="image"></param>
		/// <param name="color"></param>
		/// <returns></returns>
		private static Color TransferColor(Image image, Color color, List<Color> newPalette)
		{
			// Get the weights for each member of the palette
			double[] weights = new double[PaletteManager.PaletteSize];

			for (int i = 0; i < PaletteManager.PaletteSize; i++)
			{
				weights[i] = alglib.rbfcalc3(image.RBFs[i], color.L, color.a, color.b);

				if (weights[i] < 0)
				{
					weights[i] = 0;
				}
			}

			// Reweigh results
			double weightSum = weights.Sum();
			for (int i = 0; i < PaletteManager.PaletteSize; i++)
			{
				weights[i] = weights[i] / weightSum;
			}

			Vector<double> total = DenseVector.OfArray(new double[] { 0, 0 });

			// Apply weights to result of transfer function
			for (int i = 0; i < PaletteManager.PaletteSize; i++)
			{
				if (weights[i] == 0)
				{
					continue;
				}

				// Transfer ab value
				Vector<double> result = Transfer_AB(image, i, color, newPalette);

				total = total.Add(result.Multiply(weights[i]));
			}

			total = DenseVector.OfArray(new double[] { color.L, total[1], total[2] });

			Color newColor = Color.GetColorFromLab(color.L, total[1], total[2]);
			return newColor;
		}

		/// <summary>
		/// Transfer color's ab value in Lab color space
		/// </summary>
		/// <param name="image"></param>
		/// <param name="paletteIndex"></param>
		/// <param name="color"></param>
		/// <returns></returns>
		private static Vector<double> Transfer_AB(Image image, int paletteIndex, Color color, List<Color> newPalette)
		{
			Color C = image.Palette[paletteIndex];
			Color C_prime = newPalette[paletteIndex];

			// If palette color didn't change, use original color
			if (C.Equals(C_prime))
			{
				return color.ToVector(Lab: true);
			}

			// Initialize values
			Vector<double> x_ab = DenseVector.OfArray(new double[] { color.a, color.b });
			Vector<double> C_ab = DenseVector.OfArray(new double[] { C.a, C.b });
			Vector<double> C_prime_ab = DenseVector.OfArray(new double[] { C_prime.a, C_prime.b });

			double L = color.L;

			Vector<double> C_b = GamutIntersect(C_ab, C_prime_ab, L);

			Vector<double> x_0 = DenseVector.OfArray(new double[] { 0, 0 });
			x_0 = x_ab.Add(C_prime_ab).Subtract(C_ab);

			Vector<double> x_b = DenseVector.OfArray(new double[] { 0, 0 });
			// near case
			if (Color.GetColorFromLab(L, x_0[0], x_0[1]).OutOfGamut())
			{
				// Find where the line from C_prime to x_0
				// intersects with the gamut boundary
				x_b = GamutIntersect(C_prime_ab, x_0, L);
			}

			// far case
			else
			{
				// Get x_b where x intersects the gamut boundary
				// parallel to C - C_prime
				x_b = GamutIntersect(C_ab, C_prime_ab, L, x_ab);
			}

			// Calculate x'
			double min_numerator = x_b.Subtract(x_ab).Norm(2);
			double min_denom = C_b.Subtract(C_ab).Norm(2);

			double min_result = Math.Min(1, min_numerator / min_denom);

			// Find x' such that 
			// ||x'-x|| = ||C'-C||*(min_result)
			double dist = C_prime_ab.Subtract(C_ab).Norm(2);

			Vector<double> v = x_b.Subtract(x_ab);
			Vector<double> u = v.Divide(v.Norm(2));

			Vector<double> x_prime = x_ab.Add(u.Multiply(dist * min_result));

			// Return x', using the new color's luminance
			Color result = Color.GetColorFromLab(L, x_prime[0], x_prime[1]);

			if (result.OutOfGamut())
			{
				Vector<double> result_ab = GamutIntersect(x_ab, x_prime, L);
				result = Color.GetColorFromLab(L, result_ab[0], result_ab[1]);
			}

			return result.ToVector(Lab: true);
		}

		/// <summary>
		/// Find the gamut intersect point between the given colors in the given luminance
		/// </summary>
		/// <param name="C_ab">Start palette color</param>
		/// <param name="C_prime_ab">End palette color</param>
		/// <param name="L">Luminance</param>
		/// <param name="x">Original color</param>
		/// <returns></returns>
		private static Vector<double> GamutIntersect(Vector<double> C_ab, Vector<double> C_prime_ab, double L, Vector<double> x = null)
		{
			// Get slope from C to C_prime
			// (Cp_b - C_b) / (Cp_a - C_a)

			double db = C_prime_ab[1] - C_ab[1];
			double da = C_prime_ab[0] - C_ab[0];

			double divisor = 10;
			da = da / divisor;
			db = db / divisor;

			bool outOfBounds = false;
			Vector<double> prevColor = DenseVector.OfArray(new double[] { 0, 0 });
			Vector<double> currentColor = null;
			C_ab.CopyTo(currentColor);

			if (x != null)
			{
				x.CopyTo(currentColor);
			}

			while (!outOfBounds)
			{
				// Store the previous color
				// This will be the intersect
				// when the current color is out of bounds
				currentColor.CopyTo(prevColor);

				// Get next color
				currentColor[0] = prevColor[0] + da;
				currentColor[1] = prevColor[1] + db;

				Color color = Color.GetColorFromLab((int)Math.Round(L), (int)Math.Round(currentColor[0]), (int)Math.Round(currentColor[1]));

				if (color.OutOfGamut())
				{
					outOfBounds = true;
				}
			}

			// Last in-gamut color will be used
			Vector<double> C_b = prevColor;

			return C_b;
		}
	}
}
