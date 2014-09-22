using UnityEngine;

namespace CellUnity.Utility
{
	/// <summary>
	/// Script manager.
	/// The Script Manager finds the GameObject named "CellUnityScript" or
	/// creates it if it does not exist.
	/// The Script Manager can be used to easily add and remove scripts that
	/// do not have to be applied to specific game objects.
	/// </summary>
	public class ScriptManager
	{
		public ScriptManager ()
		{

		}

		/// <summary>
		/// Name of the GameObject
		/// </summary>
		private static readonly string GOName = "CellUnityScript";
		private GameObject pGO = null;
		/// <summary>
		/// GameObject to which the scripts are applied.
		/// </summary>
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

		/// <summary>
		/// Adds a script of the type T.
		/// </summary>
		/// <returns>The newly created script.</returns>
		/// <typeparam name="T">Type of the script</typeparam>
		public T AddScript<T>()
			where T : Component
		{
			return GO.AddComponent<T>();
		}

		/// <summary>
		/// Gets the script of a specific type.
		/// </summary>
		/// <returns>The script, null if such a script is not applied</returns>
		/// <typeparam name="T">Type of the script.</typeparam>
		public T GetScript<T>()
			where T : Component
		{
			return GO.GetComponent<T>();
		}

		/// <summary>
		/// Returns the script of a specic type.
		/// If it does not exist yet, it is created.
		/// </summary>
		/// <returns>The script</returns>
		/// <typeparam name="T">Type of the script</typeparam>
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

		/// <summary>
		/// Determines whether this instance has a script.
		/// </summary>
		/// <returns><c>true</c> if this instance has the script; otherwise, <c>false</c>.</returns>
		/// <typeparam name="T">Type of the script</typeparam>
		public bool HasScript<T>()
			where T : Component
		{
			return (GetScript<T> () != null);
		}

		/// <summary>
		/// Removes a script.
		/// </summary>
		/// <typeparam name="T">Type of the script</typeparam>
		public void RemoveScript<T>()
			where T : Component
		{
			T script = GetScript<T> ();
			if (script != null)
			{
				RemoveComponent (script);
			}
		}

		public static void RemoveComponent(Component component)
		{
			if (Application.isEditor)
			{ Component.DestroyImmediate (component); }
			else
			{ Component.Destroy (component); }
		}
	}
}

