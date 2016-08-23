using UnityEngine;
using System.Collections;
namespace Assets.Game.BattleScape.VisualObjects.Highlight
{
    public class TargetHighlight : MonoBehaviour
    {
        private static TargetHighlight _targetHighlightPrefab;
        public static TargetHighlight Create(Transform transform)
        {
            if (_targetHighlightPrefab == null)
            {
                _targetHighlightPrefab = Resources.Load<TargetHighlight>("BattleScape/TargetHighlight");
            }
            var targetnHighlight = Instantiate(_targetHighlightPrefab);

            targetnHighlight.transform.position = transform.position;
            targetnHighlight.transform.SetParent(transform);

                return targetnHighlight;
        }

    }
}
