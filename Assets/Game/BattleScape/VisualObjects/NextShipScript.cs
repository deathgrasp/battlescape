using UnityEngine;
using Assets.Utils;
using Assets.Game.BattleScape.SpaceObjects.Ships;

namespace Assets.Game.BattleScape.VisualObjects
{
    class NextShipScript : UnitySingleton<NextShipScript>
    {
        public void NextShip()
        {
            var ship = BattleScape.Instance.Ship;
            var list = BattleScape.Instance.Ships[TurnManager.Instance.CurrentPlayer];

            if (list.IndexOf(ship) >= list.Count-1)
            {
                Ship.changeActiveShip(list[0]);
            }
            else
            {
                Ship.changeActiveShip(list[list.IndexOf(ship) + 1]);
            }
        }
    }
}
