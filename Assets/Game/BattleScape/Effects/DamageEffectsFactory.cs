using Assets.Game.BattleScape.SpaceObjects;
using Assets.Game.BattleScape.SpaceObjects.Ships.Components;
using Assets.Utils;
using UnityEngine;

namespace Assets.Game.BattleScape.Effects
{
    class DamageEffectsFactory: UnitySingletonPersistent<DamageEffectsFactory>
    {

        private GameObject _laserDamageEffect;

        private GameObject LaserDamageEffect
        {
            get { return _laserDamageEffect ?? (_laserDamageEffect = Resources.Load<GameObject>("BattleScape/Effects/LaserDamageEffect")); }
        }
        private GameObject _collisionEffect;

        private GameObject CollisionEffect
        {
            get { return _collisionEffect ?? (_collisionEffect = Resources.Load<GameObject>("BattleScape/Effects/CollisionEffect")); }
        }
        private GameObject _defaultExplosion;

        private GameObject DefaultExplosion
        {
            get { return _defaultExplosion ?? (_defaultExplosion = Resources.Load<GameObject>("BattleScape/Effects/SmallExplosion")); }
        }
        public void StartDamageEffect(Vector3 position, Quaternion rotation, MonoBehaviour DamagingSource)//TODO: check pattern to improve this, such as visitor pattern or similar
        {
            if (DamagingSource is Laser)
            {
                Instantiate(LaserDamageEffect, position, rotation);
            }
            else if (DamagingSource is SpaceObject)//collision
            {
                Instantiate(CollisionEffect, position, rotation);
            }
            else //default explosion
            {
                Instantiate(DefaultExplosion, position, rotation);
            }

        }
    }
}
