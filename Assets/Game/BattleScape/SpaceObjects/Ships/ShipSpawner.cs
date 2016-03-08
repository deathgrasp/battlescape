using Assets.Game.BattleScape.Players;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Game.BattleScape.SpaceObjects.Ships
{

    public partial class Ship
    {
        private static Func<Ship> _shipFunc;
        public static void SpawnShips(int amount, BSPlayer player, Func<Ship> shipFuncPrefab,bool enemy=false)
        {
            _shipFunc = shipFuncPrefab;
            if (player.IsHuman && !enemy)
            {
                if (ConfigurationManager.Instance.ClearScene)
                {
                    spawnClear(amount, player, true);
                }
                else
                {
                    SpawnFriendlyShip(amount, player);
                }
            }
            else
            {
                if (ConfigurationManager.Instance.ClearScene)
                {
                    spawnClear(amount, player, false);
                }
                else
                {
                    BattleScape.Instance.StartCoroutine(EnemyShipSpawner(amount, player));
                }
            }
        }
        private static void spawnClear(int amount, BSPlayer player, bool left)
        {
            var vecPos = new Vector3(20, 0, -amount);
            if (left)
            {
                vecPos += new Vector3(-40, 0, 0);
            }
            var spawnLocationIndicator = new GameObject();
            spawnLocationIndicator.transform.position = vecPos;
            for (int i = 0; i < amount; i++)
            {
                spawnLocationIndicator.transform.position = spawnLocationIndicator.transform.position + (new Vector3(0, 0, 2f));
                var directionToSun = -spawnLocationIndicator.transform.position.normalized;
                var rotation = Quaternion.LookRotation(directionToSun);
                var ship = Instantiate(_shipFunc(), spawnLocationIndicator.transform.position, rotation) as Ship;
                if (ship == null)
                {
                    Debug.LogError("Could not instantiate FriendlyShipPrefab");
                    return;
                }
                ship.Init(player, player.PlayerStartHealth);
                ship.RestoreHealthToFull();
                if (player.IsHuman)
                {
                BattleScape.Instance.Ships[player].Add(ship);
                }
                else
                {
                    BattleScape.Instance.ActiveEnemyShips += 1;
                }
            }
            Destroy(spawnLocationIndicator);
        }
        private static void SpawnFriendlyShip(int amount, BSPlayer player)
        {
            var vecPos = new Vector3(0, 0, 4);
            var spawnLocationIndicator = new GameObject();
            spawnLocationIndicator.transform.position = vecPos;
            spawnLocationIndicator.transform.RotateAround(BattleScape.Instance.SolarSystem.Star.transform.position, Vector3.up, -360 / amount);
            for (int i = 0; i < amount; i++)
            {
                spawnLocationIndicator.transform.RotateAround(BattleScape.Instance.SolarSystem.Star.transform.position, Vector3.up, 360 / amount);//position for creation of the ship

                var directionToSun = spawnLocationIndicator.transform.position.normalized;
                var rotation = Quaternion.LookRotation(directionToSun);
                var ship = Instantiate(_shipFunc(), spawnLocationIndicator.transform.position, rotation) as Ship;
                if (ship == null)
                {
                    Debug.LogError("Could not instantiate FriendlyShipPrefab");
                    return;
                }
                ship.Init(player, player.PlayerStartHealth);
                ship.RestoreHealthToFull();
                BattleScape.Instance.Ships[player].Add(ship);
            }
            Destroy(spawnLocationIndicator);
        }

        private static IEnumerator EnemyShipSpawner(int amount, BSPlayer aiPlayer)
        {
            const int maxSpawnTries = 20;
            for (int i = 0; i < amount; i++)
            {
                while (BattleScape.Instance.MaxEnemyShipsActive <= BattleScape.Instance.ActiveEnemyShips)
                {
                    yield return new WaitForSeconds(1);
                }

                SpawnEnemyShip(maxSpawnTries, aiPlayer);
                yield return new WaitForSeconds(BattleScape.Instance.EnemySpawnRate);
            }
        }

        private static void SpawnEnemyShip(int numTriesLeft, BSPlayer aiPlayer)
        {
            if (BattleScape.Instance.Ship == null)//game ended, no reason to spawn more ships.
                return;

            var startTries = numTriesLeft;
            var vecPos = Vector3.zero;
            do
            {
                var position = UnityEngine.Random.insideUnitCircle * 30f;
                vecPos = new Vector3(position.x, 0, position.y);

                numTriesLeft--;

                if (numTriesLeft == 0)
                {
                    Debug.LogError("Could not find a place for enemy spawn after " + startTries + " tries");
                    return;
                }
            } while ((vecPos - BattleScape.Instance.Ship.transform.position).sqrMagnitude < BattleScape.Instance.MinEnemyDistanceFromShip * BattleScape.Instance.MinEnemyDistanceFromShip);

            var directionToSun = -vecPos.normalized;
            var rotation = Quaternion.LookRotation(directionToSun);
            var ship = Instantiate(_shipFunc(), vecPos, rotation) as Ship;
            if (ship == null)
            {
                Debug.LogError("Could not instantiate EnemyShipPrefab");
                return;
            }

            ship.Init(aiPlayer, aiPlayer.PlayerStartHealth);

            ship.RestoreHealthToFull();
            BattleScape.Instance.Ships[aiPlayer].Add(ship);
            BattleScape.Instance.ActiveEnemyShips += 1;
        }

    }
}
