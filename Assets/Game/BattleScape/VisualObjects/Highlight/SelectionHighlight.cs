using UnityEngine;

namespace Assets.Game.BattleScape.VisualObjects.Highlight
{
    public class SelectionHighlight : MonoBehaviour
    {
        private static SelectionHighlight _selectionHighlightPrefab;
        public static SelectionHighlight Create(Transform transform)
        {
            if (_selectionHighlightPrefab == null)
            {
                _selectionHighlightPrefab = Resources.Load<SelectionHighlight>("BattleScape/PlanetSelectionHighlight");
            }
            var selectionHighlight = Instantiate(_selectionHighlightPrefab);

            selectionHighlight.transform.position = transform.position;
            selectionHighlight.transform.SetParent(transform);

            return selectionHighlight;
        }

    }
}