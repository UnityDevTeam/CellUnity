using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace CellUnity.Model.Dispensing
{
	public class MoleculeSizeQueue : IEnumerable<Molecule>
	{
		private SortedList<float, Queue<Molecule>> queueList = new SortedList<float, Queue<Molecule>>(new MoleculeSizeComparer());
		
		private class MoleculeSizeComparer : IComparer<float>
		{
			public int Compare(float x, float y)
			{
				if (x == y) { return 0; }
				else if (x > y) { return -1; }
				else { return 1; }
			}
		}
		
		
		private int count = 0;
		public int Count { get { return count; } }
		
		public void Enqueue(Molecule molecule)
		{
			float key = molecule.Species.Size;
			
			Queue<Molecule> queue;
			if (queueList.TryGetValue(key, out queue))
			{
				queue.Enqueue(molecule);
			}
			else
			{
				queue = new Queue<Molecule>();
				queue.Enqueue(molecule);
				queueList.Add(key, queue);
			}
			
			count++;
		}
		
		public Molecule Dequeue()
		{
			if (count > 0)
			{
				Molecule result = null;
				Queue<Molecule> queue = null;
				
				foreach (var item in queueList)
				{
					queue = item.Value;
					result = queue.Dequeue();
					break;
				}
				
				if (queue.Count == 0)
				{
					queueList.RemoveAt(0);
				}
				
				count--;
				
				return result;
			}
			else
			{ return null; }
		}
		
		private class Enumerator : IEnumerator<Molecule>
		{
			public Enumerator(MoleculeSizeQueue queue)
			{
				queueListEnumerator = queue.queueList.GetEnumerator();
				GetQueueEnumerator();
			}
			
			private MoleculeSizeQueue queue;
			private IEnumerator<KeyValuePair<float, Queue<Molecule>>> queueListEnumerator;
			private IEnumerator<Molecule> queueEnumerator;
			private Molecule current;
			
			public Molecule Current
			{
				get { return current; }
			}
			
			public void Dispose()
			{
				current = null;
				queueListEnumerator.Dispose();
				if (queueEnumerator != null) { queueEnumerator.Dispose(); }
			}
			
			object IEnumerator.Current
			{
				get { return Current; }
			}
			
			public bool MoveNext()
			{
				while (queueEnumerator != null)
				{
					if (queueEnumerator.MoveNext())
					{
						current = queueEnumerator.Current;
						return true;
					}
					else
					{
						queueEnumerator.Dispose();
						queueEnumerator = null;
						
						GetQueueEnumerator();
					}
				}
				
				return false;
			}
			
			private void GetQueueEnumerator()
			{
				if (queueEnumerator == null)
				{
					if (queueListEnumerator.MoveNext())
					{
						var item = queueListEnumerator.Current;
						queueEnumerator = item.Value.GetEnumerator();
					}
					else
					{
						Dispose();
					}
				}
			}
			
			public void Reset()
			{
				throw new System.NotImplementedException();
			}
		}
		
		public IEnumerator<Molecule> GetEnumerator()
		{
			return new Enumerator(this);
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Enumerator(this);
		}
	}
}