using Assets.Game.BattleScape.SpaceObjects;
using Assets.Game.BattleScape.SpaceObjects.Ships;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace Assets.Game.BattleScape.VisualObjects.Path
{
    public class TrailButtonTimeComparer : IComparer<TrailButton>
    {

        public int Compare(TrailButton x, TrailButton y)
        {
            if (x.Time > y.Time)
            {
                return 1;
            }
            else if (x.Time == y.Time)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }

    }
    public class TrailButton : MonoBehaviour
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public SpaceObject Parent;
        public float Time = 0;
        public ShipVisual ShipVisual;
        private static TrailButton _trailButtonPrefab;

        private static TrailButton TrailButtonPrefab
        {
            get
            {
                return (_trailButtonPrefab ?? (_trailButtonPrefab = Resources.Load<TrailButton>("BattleScape/UI/TrailButton")));
            }
        }
        public static TrailButton Create()
        {
            var gameObject = new GameObject("Trail");
            var button = Instantiate(TrailButtonPrefab);
            gameObject.transform.SetParent(BattleScape.Instance.WorldCanvas.transform);
            button.transform.SetParent(gameObject.transform);

            return button;
        }
        public static TrailButton Create(GameObject path)
        {
            var gameObject = new GameObject("Trail");
            var button = Instantiate(TrailButtonPrefab);
            gameObject.transform.SetParent(path.transform);
            button.transform.SetParent(gameObject.transform);

            return button;
        }
        public void Click()
        {
            GetComponent<Image>().color = Color.red;
            Debug.Log("trail click");
            if (Parent is Ship)
            {
                var ship = Parent as Ship;
                ship.TrailRenderer.time = 0;

                //if (!PathPlanner.Legs.ContainsKey(ship))
                //{
                //    PathPlanner.Legs[ship] = new List<TrailButton>();
                //}
                if (ShipVisual == null)
                {
                    ShipVisual = ShipVisual.Create(Position, Rotation);
                }
                else
                {
                    ShipVisual.gameObject.SetActive(true);
                }
                ShipVisual.ShipState.MovementTarget = ship.MovementTarget;


                ShipVisual.transform.parent = transform.parent;
                ShipVisual.ShipState.Position = Position;
                ShipVisual.ShipState.Rotation = Rotation;
                BattleScape.Instance.Ship.RestoreState();

                //PathPlanner.Legs[ship].Add(this);
                SetShipState(ship);
                
                ship.TrailRenderer.time = ship.OriginalTrailRendererTime;

                ConfigurationManager.Instance.ActionMenu.gameObject.SetActive(true);
                ConfigurationManager.Instance.ActionMenu.transform.position = transform.position;

            }
        }
        public void SetShipState(Ship ship)
        {
            ship.ShipState = ShipVisual.ShipState;
            ship.transform.position = ShipVisual.ShipState.Position;
            ship.transform.rotation = ShipVisual.ShipState.Rotation;
            if (BattleScape.Instance.Ship.MovementTarget != null)
            {
                InputManager.Instance.MovementIndicator.transform.position = ship.MovementTarget.Value;
            }
            BattleScape.Instance.Ship = ship;
        }
/*
        //checks if anything has actually been changed, and acts accordingly
        //currently only move target  and attack targets are change-able
        public bool CheckDirty(ShipState state)
        {
            if (Parent is Ship &&
                ShipVisual.ShipState.AttackTarget == state.AttackTarget &&
               ShipVisual.ShipState.MovementTarget == state.MovementTarget)
            {
                GetComponent<Image>().color = Color.white;
                ShipVisual.gameObject.SetActive(false);
                return false;
            }
            return true;
        }
        */
    }
}
