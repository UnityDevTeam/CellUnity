using System;

namespace CellUnity.Utility
{
	/// <summary>
	/// Dictionary that is implemented as linked list. Fast for a small amount of keys.
	/// </summary>
	public class ShortKeyDict<TKey, TValue>
		where TKey : class
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CellUnity.Utility.ShortKeyDict`2"/> class.
		/// </summary>
		public ShortKeyDict ()
		{

		}

		/// <summary>
		/// First element
		/// </summary>
		private Entry root = null;

		/// <summary>
		/// Find an entry by a specified key.
		/// </summary>
		/// <returns>true when entry was found, false otherwise</returns>
		/// <param name="key">Key</param>
		/// <param name="foundEntry">Found entry.</param>
		public bool Find(TKey key, out Entry foundEntry)
 		{
			Entry entry = root;
			while (entry != null)
			{
				if (entry.Key == key)
				{
					foundEntry = entry;
					return true;
				}
				
				entry = entry.Next;
			}
			
			foundEntry = null;
			return false;
		}

		/// <summary>
		/// Set the value of a key
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="value">Value.</param>
		public Entry Set(TKey key, TValue value)
		{
			Entry entry;
			if (Find(key, out entry))
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

		/// <summary>
		/// Remove a key and its value
		/// </summary>
		/// <param name="key">Key.</param>
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

		/// <summary>
		/// Gets the entry after a given entry
		/// </summary>
		/// <returns><c>true</c>, if there is a next entry, <c>false</c> otherwise.</returns>
		/// <param name="entry">Entry.</param>
		/// <param name="next">Next entry if found.</param>
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

		/// <summary>
		/// A Dictionary Entry
		/// </summary>
		public class Entry
		{
			/// <summary>
			/// Key
			/// </summary>
			public TKey Key;
			/// <summary>
			/// Value
			/// </summary>
			public TValue Value;
			/// <summary>
			/// Next Entry
			/// </summary>
			public Entry Next;
		}
	}
}

