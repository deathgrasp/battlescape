using System;
using UnityEngine;

namespace Assets.Utils
{
	public static class GameObjectExtensions
	{
		public static GameObject CreateNew(this Transform parent, string name, Vector3 position, params Type[] componenets)
		{
			var go = new GameObject(name, componenets);
			go.transform.position = position;
			go.transform.parent = parent;
			return go;
		}

		public static GameObject CreateNew(string name, Vector3 position, Transform parent, params Type[] componenets)
		{
			var go = new GameObject(name, componenets);
			go.transform.position = position;
			go.transform.parent = parent;
			return go;
		}

		public static T CreateNew<T>(string name, Vector3? position = null, Transform parent = null, params Type[] components) where T : Component
		{
			var go = new GameObject(name, components);
			if(position.HasValue)
				go.transform.position = position.Value;

			go.transform.parent = parent;
			var t = go.AddComponent<T>();
			return t;
		}

		public static T CreateNew<T>(this Transform parent, string name, Vector3? position = null, params Type[] components) where T : Component
		{
			var go = new GameObject(name, components);
			if (position.HasValue)
				go.transform.position = position.Value;

			go.transform.parent = parent;
			var t = go.AddComponent<T>();
			return t;
		}
	}
}
