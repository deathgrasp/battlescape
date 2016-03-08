using UnityEngine;

namespace Assets.Game.BattleScape.VisualObjects.Path
{
    public class CollisionMarker : MonoBehaviour
    {
        private  static CollisionMarker _collisionMarkerPrefab;
        private static  CollisionMarker CollisionMarkerPrefab
        {
            get
            {
                return _collisionMarkerPrefab ?? (_collisionMarkerPrefab = Resources.Load<CollisionMarker>("BattleScape/CollisionMarker"));
            }
        }
        
        public static CollisionMarker Create()
        {
            return Instantiate(CollisionMarkerPrefab);
        }
    }
}
