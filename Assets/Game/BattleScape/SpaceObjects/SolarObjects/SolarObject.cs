using UnityEngine;
namespace Assets.Game.BattleScape.SpaceObjects.SolarObjects
{
    public class SolarObject:SpaceObject
    {
        public float GravityRadius;
        public float GravityPullStrangth;
        public GameObject GravityWell;
        public override void Start()
        {
            base.Start();
            AffectedByGravityPercent = 0;
        }
    }
}
