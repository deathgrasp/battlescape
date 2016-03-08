using UnityEngine;

namespace Assets.Utils
{
	public class ImprovedMonoBehaviour : MonoBehaviour
	{
		[HideInInspector]
		[SerializeField]
		private Transform _transform;

		public Transform Transform
		{
			get { return _transform == null ? (_transform = transform) : _transform; }
		}
	}
}
