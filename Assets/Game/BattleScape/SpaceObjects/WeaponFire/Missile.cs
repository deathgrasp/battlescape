using System.Runtime.CompilerServices;
using Assets.Game.BattleScape.Effects;
using Assets.Game.BattleScape.SpaceObjects.Ships.Components;
using UnityEngine;

namespace Assets.Game.BattleScape.SpaceObjects.WeaponFire
{
    public class Missile :Projectile
    {
        //change these in the future to the correct location.
        public static int BASESHOTBURST = 2;
        public static float BASESHOOTDELAY = 1f;
        public static float BASEAOE = 2;
        private static  Missile _missilePrefab;

        public static Missile MissilePrefab
        {
            get
            {
                return _missilePrefab ?? (_missilePrefab = Resources.Load<Missile>("BattleScape/Missile"));
            }
        }
        private Vector3 _direction;
        private Vector3 _explosionLocation;
        public static void Create(Vector3 startLocation, Vector3 direction)
        {
            var missile = Instantiate(MissilePrefab, startLocation, Quaternion.identity) as Missile;
            missile._direction = direction.normalized;
            missile._explosionLocation = direction+startLocation;
            missile.transform.LookAt(direction+new Vector3(90,0,0));
            missile.gameObject.transform.position += missile._direction;
        }

        private bool _delayFrame;//delay the destruction by a frame, allowing the object to run a full cycle in order to hit
        private void Update()
        {
            Move(Time.deltaTime);
            if (_delayFrame)
            {
                Destroy(gameObject);
            }
            if ((transform.position-_explosionLocation).sqrMagnitude<0.01)
            {
                gameObject.GetComponent<CapsuleCollider>().radius = BASEAOE;
                DeathEffectsFactory.Instance.StartDeathEffect(transform.position, Quaternion.identity); //Create a custom effect
                _delayFrame = true;
            }
        }

        private void Move(float step)
        {
            gameObject.transform.position += Speed* step * _direction;
        }
    }
}