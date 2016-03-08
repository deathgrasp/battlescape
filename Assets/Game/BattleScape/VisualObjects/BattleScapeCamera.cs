using Assets.Utils;
using UnityEngine;

namespace Assets.Game.BattleScape.VisualObjects
{
	public class BattleScapeCamera : UnitySingleton<BattleScapeCamera>
	{
		public float MinDistance;
		public float MaxDistance;
		public float ZoomSpeed;
		public float ZoomAmount;
		[HideInInspector]
		public Vector3 TargetPosition;

		public float MaxDistanceFromCenter = 30f;
		public float PanSpeed;
		public float PanAmount;

		public void Zoom(float direction)
		{
			TargetPosition += Vector3.up * direction*ZoomAmount;
			TargetPosition = TargetPosition.y > MaxDistance
				? new Vector3(TargetPosition.x, MaxDistance, TargetPosition.z)
				: (TargetPosition.y < MinDistance
					? new Vector3(TargetPosition.x, MinDistance, TargetPosition.z)
					: TargetPosition);
		}

		public void CameraXZ(float x, float z)
		{
			var targetPositionOnPlane = new Vector3(TargetPosition.x, 0f, TargetPosition.z);
			var tempPosition = new Vector3(x * PanAmount, 0f, z * PanAmount) + targetPositionOnPlane;
			if (tempPosition.sqrMagnitude > MaxDistanceFromCenter * MaxDistanceFromCenter)
			{
				var direction = (tempPosition - targetPositionOnPlane).normalized;
				tempPosition = TargetPosition + direction * (MaxDistanceFromCenter - targetPositionOnPlane.magnitude);

			}
			tempPosition.y = TargetPosition.y;

			TargetPosition = tempPosition;
		}
		public void CameraPositionChange(float xDirection, float yDirection, float zDirection)
		{
			Zoom(yDirection);
			CameraXZ(xDirection, zDirection);
		}
		private void LateUpdate()
		{
            const float STEP = 0.2f;
			transform.position = Vector3.Lerp(transform.position, TargetPosition, STEP*ZoomSpeed);
		}
	}
}