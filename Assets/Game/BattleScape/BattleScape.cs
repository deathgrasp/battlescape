using System.Collections.Generic;
using Assets.Utils;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Assets.Game.BattleScape.SpaceObjects.Ships;
using Assets.Game.BattleScape.VisualObjects;
using Assets.Game.BattleScape.SpaceObjects.SolarObjects;
using Assets.Game.BattleScape.Players;
using Assets.Game.BattleScape.VisualObjects.Highlight;

namespace Assets.Game.BattleScape
{
    /// <summary>
    /// Main class.
    /// </summary>
    [ExecuteInEditMode]
    public class BattleScape : UnitySingleton<BattleScape>
    {

        #region Configurations
        public int Seed
        {
            get { return ConfigurationManager.Instance.Seed; }
            set { ConfigurationManager.Instance.Seed = value; }
        }
        public float MinEnemyDistanceFromShip
        {
            get { return ConfigurationManager.Instance.MinEnemyDistanceFromShip; }
            set { ConfigurationManager.Instance.MinEnemyDistanceFromShip = value; }
        }
        public int PlayerStartLives
        {
            get { return ConfigurationManager.Instance.PlayerStartLives; }
            set { ConfigurationManager.Instance.PlayerStartLives = value; }
        }
        public int PlayerMaxHealth
        {
            get { return ConfigurationManager.Instance.PlayerMaxHealth; }
            set { ConfigurationManager.Instance.PlayerMaxHealth = value; }
        }

        public float PlanetMaxHealth
        {
            get { return ConfigurationManager.Instance.PlanetMaxHealth; }
            set { ConfigurationManager.Instance.PlanetMaxHealth = value; }
        }

        public float EnemyMaxHealth
        {
            get { return ConfigurationManager.Instance.EnemyMaxHealth; }
            set { ConfigurationManager.Instance.EnemyMaxHealth = value; }
        }

        #region Enemy Spawn
        public float EnemySpawnRate
        {
            get { return ConfigurationManager.Instance.EnemySpawnRate; }
            set { ConfigurationManager.Instance.EnemySpawnRate = value; }
        }
        public int StartEnemyShips
        {
            get { return ConfigurationManager.Instance.StartEnemyShips; }
            set { ConfigurationManager.Instance.StartEnemyShips = value; }
        }
        public int MaxEnemyShipsActive
        {
            get { return ConfigurationManager.Instance.MaxEnemyShipsActive; }
            set { ConfigurationManager.Instance.MaxEnemyShipsActive = value; }
        }
        [HideInInspector]
        public int ActiveEnemyShips = 0;
        #endregion Enemy Spawn

        [HideInInspector]
        public List<BSPlayer> Players = new List<BSPlayer>();
        [HideInInspector]
        public BSPlayer HumanPlayer;

        public GameObject EndGamePanel
        {
            get { return ConfigurationManager.Instance.EndGamePanel; }
            set { ConfigurationManager.Instance.EndGamePanel = value; }
        }
        public Slider PlayerHealthBar
        {
            get { return ConfigurationManager.Instance.PlayerHealthBar; }
            set { ConfigurationManager.Instance.PlayerHealthBar = value; }
        }
        #endregion Configurations

        [HideInInspector]
        public InputManager InputManager;

        public bool Generated;
        public Canvas WorldCanvas;
        public Canvas MessageCanvas;
        public SolarSystem SolarSystem;
        public Plane Plane;

        public int NumberOfHumanPlayers=0;

        #region Ships
        public Dictionary<BSPlayer,List<Ship>> Ships=new Dictionary<BSPlayer , List<Ship>>();
        public Ship Ship;
        private CurrentShipHighlight _currentShipHighlight;
        public CurrentShipHighlight CurrentShipHighlight { get
            {
                return _currentShipHighlight ?? (_currentShipHighlight=Instantiate(CurrentShipHighlight.CurrentShipHighlightPrefab));
            } }
        #endregion Ships

        public void Generate()
        {
            if (Generated)
                return;


            Random.seed = Seed;

            SolarSystem = SolarSystem.Create();
            SolarSystem.Generate();

            PlayerHealthBar.maxValue = PlayerMaxHealth;

            HumanPlayer = BSPlayer.Create("Human Player", PlayerStartLives, PlayerMaxHealth,0, true);
            Ship.SpawnShips(ConfigurationManager.Instance.StartFriendlyShips +1, HumanPlayer, () => Ship.FriendlyShipPrefab);
            Ship = Ships[HumanPlayer][0];
            TurnManager.Instance.CurrentPlayer = HumanPlayer;


            BSPlayer player2;
            if (ConfigurationManager.Instance.HotSeat)
            {
                player2 = BSPlayer.Create("Human Player 2", PlayerStartLives, PlayerMaxHealth, 1, true);
                Ship.SpawnShips(StartEnemyShips, player2, () => Ship.FriendlyShipPrefab, true);
                NumberOfHumanPlayers += 1;
            }
            else
            {
                player2 = BSPlayer.Create("AI Player", 0, EnemyMaxHealth);
                Ship.SpawnShips(StartEnemyShips, player2, () => Ship.EnemyShipPrefab, true);

            }
            CurrentShipHighlight.transform.position = Ship.transform.position;
            Generated = true;
        }

        private void Start()
        {
            if (!Application.isEditor || (Application.isEditor && Application.isPlaying))
            {
                Plane = new Plane(Vector3.up, 0f);
                InputManager = InputManager.Instance;
                Generate();
            }
            else
            {
                name = "BattleScape";
                transform.position = Vector3.zero;

                CryOnMissingReference(PlayerHealthBar);
            }
        }

        private void CryOnMissingReference(Object it)
        {
            if (it == null)
                Debug.LogError(it.GetType() + " missing from " + name);
        }

        public void GameOver()
        {
            //TODO: while running unity, does not exit game.
            Debug.Log("game ended");
            GUIManager.Instance.EndScreen();
        }
    }
}
