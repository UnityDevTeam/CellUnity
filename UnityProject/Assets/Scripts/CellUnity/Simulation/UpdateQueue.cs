using System;
using CellUnity.Simulation.Update;

namespace CellUnity.Simulation
{
	public class UpdateQueue
	{
		public UpdateQueue ()
		{

		}

		private readonly object lockObject = new object();
		private QueueElement root = null;

		private class QueueElement
		{
			public QueueElement(CueUpdate update)
			{
				this.Update = update;
				this.Next = null;
			}

			public CueUpdate Update;
			public QueueElement Next;
		}

		public void Enqueue(CueUpdate update)
		{
			lock (lockObject)
			{
				if (root == null)
				{
					root = new QueueElement(update);
				}
				else
				{
					QueueElement element = root;
					QueueElement last = null;

					// check for each queue element if it can be replaced by the current item
					do
					{
						CueUpdate newUpdate;
						if (update.CanReplace(element.Update, out newUpdate))
						{
							element.Update = newUpdate;
							return;
						}

						last = element;
						element = element.Next;
					}
					while (element != null);

					// insert new element if it couldn't be replaced
					last.Next = new QueueElement(update);
				}
			}
		}

		public bool Dequeue(out CueUpdate update)
		{
			lock (lockObject)
			{
				if (root == null)
				{
					update = null;
					return false;
				}
				else
				{
					update = root.Update;
					root = root.Next;

					return true;
				}
			}
		}
	}
}

