using System.Collections.Generic;
using Assets.Utils;
using UnityEngine;
using Assets.Game.BattleScape;
using Assets.Game.BattleScape.SpaceObjects.Ships;

namespace Assets.Game.BattleScape.Players
{
	public class BSPlayer : MonoBehaviour
	{
		//public List<PlayerAction> Actions;

		public bool IsHuman { get; private set; }

		public float PlayerStartHealth;
		public int PlayerLives;
        public int PlayerNumber;
        //playernumber represents which player is playing. used for hotseat, multiplayer, or multiple sides with same player.
		public static BSPlayer Create(string name, int playerStartLives, float playerStartHealth,int playerNumber=0, bool isHuman = false)
		{
			var player = GameObjectExtensions.CreateNew<BSPlayer>(name);
			player.PlayerLives = playerStartLives;
			player.PlayerStartHealth = playerStartHealth;
			player.IsHuman = isHuman;
            player.PlayerNumber = playerNumber;
            BattleScape.Instance.Players.Add(player);
            BattleScape.Instance.Ships.Add(player, new List<Ship>());
			return player;
		}
	}
}
