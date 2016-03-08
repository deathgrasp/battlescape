using UnityEngine;

namespace Assets.Game.BattleScape.VisualObjects.Highlight
{
    public class SelectionHighlight : MonoBehaviour
    {
        private static SelectionHighlight _selectionHighlightPrefab = Resources.Load<SelectionHighlight>("BattleScape/PlanetSelectionHighlight");
        public static SelectionHighlight Create(Transform transform)
        {
            var selectionHighlight = Instantiate(_selectionHighlightPrefab);

            selectionHighlight.transform.position = transform.position;
            selectionHighlight.transform.SetParent(transform);

            return selectionHighlight;
        }

    }
}