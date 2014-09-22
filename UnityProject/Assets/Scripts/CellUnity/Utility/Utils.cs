using System;

namespace CellUnity.Utility
{
	/// <summary>
	/// Provides static methods.
	/// </summary>
	public static class Utils {

		/// <summary>
		/// Prime numbers for hash code generation
		/// </summary>
		private static readonly int[] primes = new int[] { 1, 7, 11, 13, 17, 19, 23, 29, 31 };

		/// <summary>
		/// Generate a hashcode by combining the hashcode of multiple objects
		/// using prime numbers.
		/// </summary>
		/// <returns>Hashcode</returns>
		/// <param name="objs">Objects.</param>
		public static int Hash(params object[] objs)
		{
			int i = 0;
			int hash = 1;
			foreach (object item in objs)
			{
				int p = primes[i++ % primes.Length];
				if (item != null)
				{ hash += p * item.GetHashCode(); }
				else
				{ hash += p * -1; }
			}
			
			return hash;
		}

		/// <summary>
		/// Checks if a the type of an object equals a given type.
		/// If it equals, true is returned and the object is casted and
		/// returned by the output parameter other.
		/// If the types do not equal or obj is null, false is returned
		/// </summary>
		/// <returns><c>true</c>, if obj has type T, <c>false</c> otherwise.</returns>
		/// <param name="obj">object</param>
		/// <param name="other">casted object</param>
		/// <typeparam name="T">Type to check</typeparam>
		public static bool TypeEquals<T>(object obj, out T other) 
			where T:class
		{
			if (obj == null) {
				other = null;
				return false;
			} else {
				if (typeof(T).Equals(obj.GetType())) {
					other = (T)obj;
					return true;
				}
				else {
					other = null;
					return false;
				}
			}
		}

		/// <summary>
		/// Checks if the items of two arrays equal.
		/// </summary>
		/// <returns><c>true</c>, if all items equal, <c>false</c> otherwise.</returns>
		/// <param name="a">first array, must not be null</param>
		/// <param name="b">second array, must not be null</param>
		/// <typeparam name="T">Type</typeparam>
		public static bool ArrayEquals<T>(T[] a, T[] b)
		{
			if (a.Length == b.Length)
			{
				for (int i = 0; i < a.Length; i++) {
					if (!Object.Equals(a[i], b[i]))
					{
						return false;
					}
				}
				
				return true;
			}
			else { return false; }
		}

		/// <summary>
		/// Checks if an array contains a element.
		/// Looks for the identical element (== Operator)
		/// </summary>
		/// <returns><c>true</c>, if array contains element, <c>false</c> otherwise.</returns>
		/// <param name="array">Array.</param>
		/// <param name="element">Element.</param>
		/// <typeparam name="T">Type of the array elements</typeparam>
		public static bool ArrayContains<T>(T[] array, T element)
			where T : class
		{
			foreach (var item in array) {
				if (item == element) { return true; }
			}

			return false;
		}

		/// <summary>
		/// Returns the Unity units of a length given in nm.
		/// </summary>
		/// <returns>Unity units</returns>
		/// <param name="nm">nm</param>
		public static float ScaleFromNm(float nm)
		{
			return nm;
		}

		/// <summary>
		/// Returns the nm
		/// </summary>
		/// <returns>The to nm.</returns>
		/// <param name="unityUnit">Unity unit.</param>
		public static float ScaleToNm(float unityUnit)
		{
			return unityUnit;
		}

		/// <summary>
		/// Calculates the radius of a sphere with a given volume
		/// </summary>
		/// <returns>Radius in nm</returns>
		/// <param name="volumeNanoliter">Volume in nl</param>
		public static float GetSphereRadius(float volumeNanoliter)
		{
			float V = volumeNanoliter * 1e15f; // nanoliter -> nm³ so we have r in nm in the end
			float r = UnityEngine.Mathf.Pow( 3*V / (4*UnityEngine.Mathf.PI) , 1f/3f );
			return r;
		}
	}
}