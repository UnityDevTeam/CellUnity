
namespace CellUnity.Utility
{
	static class Utils {

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
	}
}