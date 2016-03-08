using Assets.Game.BattleScape.SpaceObjects;
using Assets.Game.BattleScape.SpaceObjects.Ships;
using Assets.Game.BattleScape.SpaceObjects.SolarObjects;
using System;
using UnityEngine;

namespace Assets.Game.BattleScape.Players
{
    public class EnemyAI : MonoBehaviour
    {
        public Ship Ship;
        public float TargetPrioritization = 3f; //requires player to be over 3 times further than a planet to target him.
        private void Update()
        {
			if(!BattleScape.Instance.Generated)
				return;

            var spaceObject = AcquireTarget();
            if (this != null&& spaceObject!=null)
            {
                Ship.MovementTarget = spaceObject.transform.position;
                Ship.AttackObject(spaceObject);
            }
        }

        private SpaceObject AcquireTarget()
        {
            //this function assumes that there will always be a valid player or a none-destroyed planet to target
            var playerShip = BattleScape.Instance.Ship;
            var friendlyShips = BattleScape.Instance.Ships[BattleScape.Instance.Players[0]] ;
            var planets = BattleScape.Instance.SolarSystem.Planets;
            Planet closestPlanet = null;
            if (planets != null && planets.Length >0)//finds the closest planet
            {
                closestPlanet = planets[0];
                foreach (var planet in planets)
                {
                    if (closestPlanet==null || (closestPlanet.PlanetStatus==Planet.Status.Destroyed && planet!=null))
                    {
                        closestPlanet = planet;
                    }
                    else if (planet != null && planet.PlanetStatus!=Planet.Status.Destroyed && (transform.position - planet.transform.position).sqrMagnitude < (transform.position - closestPlanet.transform.position).sqrMagnitude)
                    {
                        closestPlanet = planet;
                    }
                }
            }
            if (friendlyShips != null && friendlyShips.Count >= 0) //finds the closest enemy (friendly ship, as it's an enemy ship) ship
            {
                foreach (var ship in friendlyShips)
                {
                    if (playerShip == null)
                    {
                        playerShip = ship;
                    }
                    else if (ship != null && (transform.position - ship.transform.position).sqrMagnitude < (transform.position - playerShip.transform.position).sqrMagnitude)
                    {
                        playerShip = ship;
                    }
                }
            }
            //checks if either planet or player are null, otherwise checking distance.
            if (closestPlanet == null)
            {
                return playerShip;
            }
            else if (playerShip == null)
            {
                return closestPlanet;
            }
            else if ((playerShip.transform.position - transform.position).sqrMagnitude/ TargetPrioritization < (closestPlanet.transform.position - transform.position).sqrMagnitude)
            {
                return playerShip;
            }
            else
            {
                return closestPlanet;
            }

        }
    }
}