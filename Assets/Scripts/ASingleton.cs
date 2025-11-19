using UnityEngine;

namespace WiDiD.Extension
{
	/// <summary>
	/// Abstract class to implement more easily the Singleton design pattern.
	/// </summary>
	public abstract class ASingleton<T> : MonoBehaviour where T : ASingleton<T>
	{
		private static T _instance;

		/// <summary>
		/// Behaviour of the singleton if a new one is instanced
		/// </summary>
		//public enum SingletonBehaviour { ReplaceOldInstance, KeepOldInstance }

		//public SingletonBehaviour _SingletonBehaviour = SingletonBehaviour.ReplaceOldInstance;

		/// <summary>
		/// Singleton instance
		/// </summary>
		public static T Instance
		{
			get
			{
				if (_instance == null)
				{
#if UNITY_EDITOR
					if (!Application.isPlaying)
					{
						var script = GameObject.FindObjectOfType<ASingleton<T>>();
						if (script == null)
						{
							Debug.LogWarning(typeof(T).ToString() + " is missing.");
							return null;
						}
						script.InitializeSingleton();
						return _instance;
					}
#endif
					//Debug.LogWarning(typeof(T).ToString() + " is missing.");
				}

				return _instance;
			}
		}


		protected void Awake()
		{
			InitializeSingleton();
		}

		public void InitializeSingleton()
		{
			if (_instance != null)
			{
				Debug.LogWarning("Second instance of " + typeof(T) + " created. Automatic self-destruct triggered.");
				// Destroy this instance if in play mode
				if (Application.isPlaying) Destroy(this);
				return;
			}
			_instance = this as T;

			AwakeSpecific();
		}

		protected void OnDestroy()
		{
			if (_instance == this)
			{
				_instance = null;
				OnDestroySpecific();
			}
		}

		/// <summary>
		/// This function is called OnAwake, on the singleton
		/// </summary>
		protected virtual void AwakeSpecific()
		{
		}
		/// <summary>
		/// This function is called OnDestroy, on the singleton
		/// </summary>
		protected virtual void OnDestroySpecific()
		{
		}

	}
}