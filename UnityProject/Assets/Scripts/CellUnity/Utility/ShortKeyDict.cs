using System;

namespace CellUnity.Utility
{
	public class ShortKeyDict<TKey, TValue>
		where TKey : class
	{
		public ShortKeyDict ()
		{

		}

		private Entry root = null;

		public Entry Find(TKey key)
		{
			Entry entry = root;
			while (entry != null)
			{
				if (entry.Key == key)
				{
					return entry;
				}

				entry = entry.Next;
			}

			return null;
		}

		public Entry Set(TKey key, TValue value)
		{
			Entry entry = Find (key);
			if (entry != null)
			{
				entry.Value = value;
				return entry;
			}
			else
			{
				Entry oldRoot = root;

				root = new Entry();
				root.Value = value;
				root.Key = key;
				root.Next = oldRoot;
				return root;
			}
		}

		public bool Remove(TKey key)
		{
			Entry entry = root;
			Entry last = null;
			while (entry != null)
			{
				if (entry.Key == key)
				{
					if (last == null)
					{
						root = entry.Next;
					}
					else
					{
						last.Next = entry.Next;
					}

					return true;
				}

				last = entry;
				entry = entry.Next;
			}

			return false;
		}

		public bool GetNext(Entry entry, out Entry next)
		{
			if (entry == null)
			{
				next = root;
				return (next != null);
			}
			else
			{
				next = entry.Next;
				return (next != null);
			}
		}

		public class Entry
		{
			public TKey Key;
			public TValue Value;
			public Entry Next;
		}
	}
}

