using System;

namespace CellUnity.Utility
{
	public static class Utils {

		private static readonly int[] primes = new int[] { 1, 7, 11, 13, 17, 19, 23, 29, 31 };
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

		public static bool TypeEquals<T>(object me, object obj, out T other) 
			where T:class
		{
			if (obj == null) {
				other = null;
				return false;
			} else {
				if (me.GetType().Equals(obj.GetType())) {
					other = (T)obj;
					return true;
				}
				else {
					other = null;
					return false;
				}
			}
		}
		
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

		public static bool ArrayContains<T>(T[] array, T element)
			where T : class
		{
			foreach (var item in array) {
				if (item == element) { return true; }
			}

			return false;
		}

		public static float ScaleFromNm(float nm)
		{
			return nm;
		}

		public static float ScaleToNm(float unityUnit)
		{
			return unityUnit;
		}

		public static float GetSphereRadius(float volumeNanoliter)
		{
			float V = volumeNanoliter * 1e15f; // nanoliter -> nm³ so we have r in nm in the end
			float r = UnityEngine.Mathf.Pow( 3*V / (4*UnityEngine.Mathf.PI) , 1f/3f );
			return r;
		}
	}
}