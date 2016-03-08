using UnityEngine;

namespace Assets.Game.BattleScape.VisualObjects.Path
{
	public class MovementIndicator : MonoBehaviour
	{
        private static MovementIndicator _movementIndicatorPrefab;

        public static MovementIndicator MovementIndicatorPrefab
        {
            get
            {
                return _movementIndicatorPrefab
                                           ?? (_movementIndicatorPrefab = Resources.Load<MovementIndicator>("BattleScape/MovementIndicator"));
            }
        }
    }
}
