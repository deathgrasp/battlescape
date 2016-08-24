using Assets.Game.BattleScape.SpaceObjects.WeaponFire;
using Assets.Game.BattleScape.VisualObjects.Path;
using UnityEngine;

namespace Assets.Game.BattleScape.SpaceObjects.Ships
{/// <summary>
/// A class used to remember the ships state, such as movement's target or attack's target.
/// </summary>
   public class ShipState
    {
        public Vector3? MovementTarget { get; set; }
        public Waypoint NextWaypoint;
        public float RotationSpeed = ConfigurationManager.Instance.RotationSpeed;
        public float MovementSpeed = ConfigurationManager.Instance.MovementSpeed;
        public SpaceObject AttackTarget;
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 GunShotTarget;
    public float ShotTimer=0f;
    }
}
