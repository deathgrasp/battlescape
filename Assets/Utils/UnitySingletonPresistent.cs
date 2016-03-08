using UnityEngine;

namespace Assets.Utils
{
	/// <summary>
	/// A generic singleton with persistence in unity
	/// <see cref="http://redframe-game.com/blog/global-managers-with-generic-singletons/"/>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class UnitySingletonPersistent<T> : MonoBehaviour
		where T : Component
	{
		private static T _instance;
		public static T Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = FindObjectOfType(typeof(T)) as T;
					if (_instance == null)
					{
						var obj = new GameObject
						{
                            name = typeof(T).Name,
                            //hideFlags = HideFlags.HideAndDontSave
						};
						_instance = obj.AddComponent(typeof(T)) as T;
					}
				}
				return _instance;
			}
		}

		protected virtual void Awake()
		{
			DontDestroyOnLoad(gameObject);
			if (_instance == null)
			{
				_instance = this as T;
				_instance.transform.position = Vector3.zero;
			}
			else
			{
				Destroy(gameObject);
			}
		}
	}
}