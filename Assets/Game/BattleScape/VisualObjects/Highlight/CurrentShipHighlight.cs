using UnityEngine;

namespace Assets.Game.BattleScape.VisualObjects.Highlight
{
    public class CurrentShipHighlight : MonoBehaviour
    {
        private static CurrentShipHighlight _currentShipHighlightPrefab;

        public static CurrentShipHighlight CurrentShipHighlightPrefab
        {
            get
            {
                return _currentShipHighlightPrefab
                                           ?? (_currentShipHighlightPrefab = Resources.Load<CurrentShipHighlight>("BattleScape/CurrentShipHighlight"));
            }
        }
        public GameObject VisualObject;
        public void Update()
        {
            VisualObject.SetActive(true);
            if (BattleScape.Instance.Ship != null&&TurnManager.Instance.GamePaused)
            {
                transform.position = BattleScape.Instance.Ship.transform.position;
            }
            else
            {
                VisualObject.SetActive(false);
            }
        }
    }
}
