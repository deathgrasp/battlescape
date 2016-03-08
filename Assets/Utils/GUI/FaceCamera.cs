using System.Linq;
using Assets.Game;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Utils.GUI
{
	public class FaceCamera : MonoBehaviour
	{
		private Transform _cameraTransfrom;
		public Vector3 Offset;
		public Transform Follow;
		private bool _isSetup;

		private void Update()
		{
			if(!_isSetup && Follow != null)
				Setup(Follow);
		}

		public void Setup(Transform follow, Camera targetCamera = null)
		{
			_cameraTransfrom = targetCamera != null ? targetCamera.transform : Camera.main.transform;
			Follow = follow;
			transform.position = Follow.position + Offset;
			transform.SetParent(Follow);
			_isSetup = true;
		}

		private void LateUpdate()
		{
			transform.LookAt(transform.position + _cameraTransfrom.rotation * Vector3.forward,
						 _cameraTransfrom.rotation * Vector3.up);
		} 
	}
}