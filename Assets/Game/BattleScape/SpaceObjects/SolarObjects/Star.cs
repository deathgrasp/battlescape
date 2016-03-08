namespace Assets.Game.BattleScape.SpaceObjects.SolarObjects
{
	public class Star : SolarObject
	{
        public new  void Start()
        {
            MaxHealth = float.MaxValue;
            RestoreHealthToFull();
        }
	}
}
