using Assets.Game.BattleScape.SpaceObjects.Ships.Components;
using UnityEngine;

namespace Assets.Game.BattleScape.SpaceObjects.WeaponFire
{
    public class GunShot :Projectile
    {
        //change these in the future to the correct location.
        public static int BASESHOTBURST = 10;
        public static float BASESHOOTDELAY = 0.1f;
        private static  GunShot _gunShotPrefab;

        public static GunShot GunShotPrefab
        {
            get
            {
                return _gunShotPrefab ?? (_gunShotPrefab = Resources.Load<GunShot>("BattleScape/GunShot"));
            }
        }
        private Vector3 _direction;

        public static void Create(Vector3 startLocation, Vector3 direction)
        {
            var gunshot = Instantiate(GunShotPrefab, startLocation, Quaternion.identity) as GunShot;
            gunshot._direction = direction.normalized;
            gunshot.gameObject.transform.position += gunshot._direction;
        }

        private void Update()
        {
            Move(Time.deltaTime);
        }

        private void Move(float step)
        {
            gameObject.transform.position += Speed* step * _direction;
        }
    }
}