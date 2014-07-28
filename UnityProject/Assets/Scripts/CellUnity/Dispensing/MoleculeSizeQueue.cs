using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace CellUnity.Dispensing
{
	public class MoleculeSizeQueue : IEnumerable<MoleculeSpecies>
	{
		private SortedList<float, Queue<MoleculeSpecies>> queueList = new SortedList<float, Queue<MoleculeSpecies>>(new MoleculeSizeComparer());
		
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
		
		public void Enqueue(MoleculeSpecies species)
		{
			float key = species.Size;
			
			Queue<MoleculeSpecies> queue;
			if (queueList.TryGetValue(key, out queue))
			{
				queue.Enqueue(species);
			}
			else
			{
				queue = new Queue<MoleculeSpecies>();
				queue.Enqueue(species);
				queueList.Add(key, queue);
			}
			
			count++;
		}
		
		public MoleculeSpecies Dequeue()
		{
			if (count > 0)
			{
				MoleculeSpecies result = null;
				Queue<MoleculeSpecies> queue = null;
				
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
		
		private class Enumerator : IEnumerator<MoleculeSpecies>
		{
			public Enumerator(MoleculeSizeQueue queue)
			{
				queueListEnumerator = queue.queueList.GetEnumerator();
				GetQueueEnumerator();
			}
			
			private MoleculeSizeQueue queue;
			private IEnumerator<KeyValuePair<float, Queue<MoleculeSpecies>>> queueListEnumerator;
			private IEnumerator<MoleculeSpecies> queueEnumerator;
			private MoleculeSpecies current;
			
			public MoleculeSpecies Current
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
		
		public IEnumerator<MoleculeSpecies> GetEnumerator()
		{
			return new Enumerator(this);
		}
		
		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Enumerator(this);
		}
	}
}