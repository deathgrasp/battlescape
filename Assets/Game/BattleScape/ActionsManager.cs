using UnityEngine;
using Assets.Utils;
using System;
using System.Collections.Generic;
using Assets.Game.BattleScape.SpaceObjects;
using Assets.Game.BattleScape.SpaceObjects.Ships;

namespace Assets.Game.BattleScape
{
    /// <summary>
    /// INCOMPLETE
    /// Manager for various actions, such as attacking a target.
    /// Expected use: actions loaded into the actions lists, then related PerformActions function is called.
    /// </summary>
    public class ActionsManager : UnitySingleton<ActionsManager>
    {
        public List<Action<SpaceObject>> SpaceObjectActions; //such as attack target
        public List<Action<Vector3>> LocationActions; //such as create waypoint
        public ShipState ActiveShip; 
        public void PerformActions(SpaceObject spaceObject)
        {
            foreach (var action in SpaceObjectActions)
            {
                action(spaceObject);
            }
        }
        public void PerformActions(Vector3 location)
        {
            foreach (var action in LocationActions)
            {
                action(location);
            }
        }
        public void AttackTarget(SpaceObject spaceObject)
        {
        }
        public void CreateWayPoint(Vector3 location)
        {
        }
        public void ClearLists()
        {
            SpaceObjectActions.Clear();
            LocationActions.Clear();
        }
    }
}
