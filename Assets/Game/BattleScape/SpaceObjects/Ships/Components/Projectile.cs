using UnityEngine;

namespace Assets.Game.BattleScape.SpaceObjects.Ships.Components
{
    public class Projectile : SpaceObject
    {
        public float Lifetime;
        public float Damage;
        public float Speed;

        private void OnCollisionEnter(Collision collision)
        {
            Destroy(gameObject);
        }
    }
}
