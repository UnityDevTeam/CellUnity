using UnityEngine;

namespace CellUnity.Utility
{
	public class ScriptManager
	{
		public ScriptManager ()
		{

		}

		private static readonly string GOName = "CellUnityScript";
		private GameObject pGO = null;
		private GameObject GO
		{
			get
			{
				if (pGO == null)
				{
					pGO = GameObject.Find(GOName);
					if (pGO == null)
					{
						pGO = new GameObject(GOName);
					}
				}

				return pGO;
			}
		}

		public T AddScript<T>()
			where T : Component
		{
			return GO.AddComponent<T>();
		}

		public T GetScript<T>()
			where T : Component
		{
			return GO.GetComponent<T>();
		}

		public T GetOrAddScript<T>()
			where T : Component
		{
			T script = GetScript<T> ();
			if (script == null)
			{
				script = AddScript<T>();
			}

			return script;
		}

		public bool HasScript<T>()
			where T : Component
		{
			return (GetScript<T> () != null);
		}

		public void RemoveScript<T>()
			where T : Component
		{
			T script = GetScript<T> ();
			if (Application.isEditor)
			{ MonoBehaviour.DestroyImmediate (script); }
			else
			{ MonoBehaviour.Destroy (script); }
		}
	}
}

