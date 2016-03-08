using Assets.Game.BattleScape.Players;
using UnityEngine;
using Assets.Utils;
using Assets.Game.BattleScape.VisualObjects;
using Assets.Game.BattleScape.VisualObjects.Path;
using System;
/// <summary>
/// Manager for turns. In charge of pausing and unpausing the game.
/// </summary>
namespace Assets.Game.BattleScape
{

    public class TurnManager : UnitySingleton<TurnManager>
    {
        public int TurnNumber { get; set; }
        public BSPlayer CurrentPlayer { get; set; }
        public bool GamePaused { get; private set; }
        public int CurrentPlayerTurn;
        public float TurnTime
        {
            get
            {
                return ConfigurationManager.Instance.TurnTime;
            }
        }

        public void BeginTurn()
        {
            PathPlanner.PlanPathStart();

        }
        public float TimeLeftForTurn { get; set; }
        //Assumes that pause enables change of action. If we add a pause/unpause that doesn't reset time, or doesn't allow to change commands, it needs to be implemented.
        public void Pause()
        {
            TurnAllTrailShipRenderers(false);
            PathPlanner.OnPause();
            Time.timeScale = 0;
            GamePaused = true;
            if (ConfigurationManager.Instance.TotalGameTurns <= TurnNumber)
            {
                BattleScape.Instance.GameOver();
            }
            else
            {
                BeginTurn();
            }
        }

        public void TurnAllTrailShipRenderers(bool on)
        {
            foreach (var list in BattleScape.Instance.Ships.Values)
            {
                foreach (var ship in list)
                {
                    if (on)
                    {
                        ship.TrailRenderer.time = ship.OriginalTrailRendererTime;
                    }
                    else
                    {
                        ship.TrailRenderer.time = 0;
                    }
                    ship.TrailRenderer.enabled = on;
                }
            }
        }

        public void EndTurn()
        {
            if (InputManager.Instance.CurrentTarget != false)
            {
                InputManager.Instance.CurrentTarget.SetTarget(false);
            }
            if (CurrentPlayerTurn == BattleScape.Instance.NumberOfHumanPlayers)
            {
                BattleScape.Instance.Ship.RestoreState();

                PathPlanner.Clear();
                CurrentPlayer = BattleScape.Instance.Players[0];
                CurrentPlayerTurn = 0;
                BattleScape.Instance.Ship = BattleScape.Instance.Ships[CurrentPlayer][0];

                Time.timeScale = 1; //TODO: check if using Time is the correct way, or I should create a TimeManager class
                //PathPlanner.SortPaths();
                TurnAllTrailShipRenderers(true);

                GamePaused = false;
                TurnNumber++;
                PathPlanner.ResetWaypoints();
            }
            else
            {
                BattleScape.Instance.Ship.RestoreState();
                PathPlanner.Clear();
                CurrentPlayerTurn += 1;
                CurrentPlayer = BattleScape.Instance.Players[CurrentPlayerTurn];

                BattleScape.Instance.Ship = BattleScape.Instance.Ships[CurrentPlayer][0];
                BeginTurn();
            }
        }
        public void Start()
        {
            TurnNumber = 0;
        }
        public void Update()
        {

            if (BattleScape.Instance.Generated == false)
                return;

            //TODO: on game end- continue running, or pause?
            if (TimeLeftForTurn > 0)
            {
                TimeLeftForTurn -= Time.deltaTime;
            }
            else
            {
                TimeLeftForTurn = TurnTime - TimeLeftForTurn; //so that if we did a bit more for this turn, we will do a little less for the next
                Pause();
            }
        }

    }
}
