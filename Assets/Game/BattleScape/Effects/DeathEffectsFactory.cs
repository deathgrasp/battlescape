using Assets.Utils;
using UnityEngine;

namespace Assets.Game.BattleScape.Effects
{
	public class DeathEffectsFactory : UnitySingletonPersistent<DeathEffectsFactory>
	{

		private GameObject _deathEffect;

		public GameObject DeathEffect
		{
			get { return _deathEffect ?? (_deathEffect = Resources.Load<GameObject>("BattleScape/Effects/Explosion")); }
		}

		public void StartDeathEffect(Vector3 position, Quaternion rotation)
		{
			Instantiate(DeathEffect, position, rotation);

		}
	}
}
