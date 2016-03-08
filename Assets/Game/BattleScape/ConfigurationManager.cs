using Assets.Game.BattleScape.VisualObjects;
using Assets.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.BattleScape
{
    /// <summary>
    /// A script class for unity and the game, to set the various configurations. For example, the player's starting lives.
    /// </summary>
    class ConfigurationManager : UnitySingleton<ConfigurationManager>
    {
        public int TurnsToCalculate=2;
        public int Seed;
        public bool ClearScene;
        public bool HotSeat;
        public float MinEnemyDistanceFromShip;
        #region Player Ships
        public int PlayerStartLives = 1;
        public int PlayerMaxHealth = 200;
        public int StartFriendlyShips = 0;
        #endregion Player Ships 
        public float PlanetGravityRadius = 5f;
        public float PlanetGravityStrength = 0.2f;
        public float StarGravityRadius = 5f;
        public float StarGravityStrength = 0.2f;

        public float PlanetMaxHealth=1f;
        public float RammingExtraDamage = 0f;
        public float RammingDamageModifier=1f;
        public float EnemyMaxHealth;
        public float RotationSpeed = 50f;
        public float MovementSpeed = 1f;
        #region Enemy Spawn
        public float EnemySpawnRate;
        public int StartEnemyShips;
        public int MaxEnemyShipsActive;

        #endregion Enemy Spawn

        public GameObject EndGamePanel;
        public Slider PlayerHealthBar;
        public ActionMenuScripts ActionMenu;
        public int TotalGameTurns=50;
        public float TurnTime=5f;
        public float StepSizeMultiplier=2;
        public float NonSelectedUIVisibility;
    }
}
