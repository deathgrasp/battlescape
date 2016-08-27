using Assets.Game.BattleScape.SpaceObjects;
using Assets.Game.BattleScape.SpaceObjects.Ships;
using Assets.Game.BattleScape.SpaceObjects.SolarObjects;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Game.BattleScape.VisualObjects.Path
{
    public static class PathPlanner
    {
        public static Dictionary<Ship, Waypoint> Legs = new Dictionary<Ship, Waypoint>(); //Rename?
        public static Dictionary<Ship, CollisionMarker> Collisions = new Dictionary<Ship, CollisionMarker>();

        private static Planet[] Planets
        {
            get { return BattleScape.Instance.SolarSystem.Planets; }
        }

        private static List<Ship> Ships
        {
            get { return BattleScape.Instance.Ships[TurnManager.Instance.CurrentPlayer]; }
        }

        public static float TotalPathingTime;

        public static void ResetWaypoints()
        {
            foreach (var ship in Legs.Keys)
            {
                ship.ShipState = Legs[ship].ShipVisual.ShipState;
                ship.ShipState.NextWaypoint = Legs[ship];
            }
        }

        public static void ResetWaypointPathTime(Waypoint waypoint)
        {
            while (waypoint.PreviousAction!=null)
            {
                waypoint = waypoint.PreviousAction;
            }
            waypoint.Time = 0.01f;
            while (waypoint.NextAction!=null)
            {
                waypoint=waypoint.NextAction;
                waypoint.Time = float.MaxValue;
            }
        }
        public static void PlanPath()
        {
            ResetWaypoints();
            TotalPathingTime = 0;
            var stepNumber = 0;
            var step = Time.fixedDeltaTime; //1.0 / PathResolution;//seconds
            step = step * ConfigurationManager.Instance.StepSizeMultiplier; //space it up to reduce clutter
            var totalTime = 0.0f;
            var pathingMaxSeconds = ConfigurationManager.Instance.TurnTime;
            var ship = BattleScape.Instance.Ship;
            ship.ClearPath();
            if (!Collisions.ContainsKey(ship))
            {
                Collisions.Add(ship, CollisionMarker.Create());
            }
            Collisions[ship].gameObject.SetActive(false);
            var pos = ship.transform.position;
            var rotation = ship.transform.rotation;

            for (var i = 0; i < ConfigurationManager.Instance.TurnsToCalculate; i++)
            {
                while (true)
                {
                    stepNumber++;
                    totalTime += step;
                    TotalPathingTime += step;
                    if (totalTime >pathingMaxSeconds) //exit after a certain time to prevent infinate loops)
                        break;
                    CheckIfWaypointReached(ship, TotalPathingTime);

                    ship.TakeStep(step);

                    if (DetectCollision(ship, stepNumber))
                    {
                        Collisions[ship].gameObject.SetActive(true);
                        Collisions[ship].transform.position = ship.transform.position;
                        break;
                    }

                }
                totalTime = 0;
            }
            if (ship.ShipState.NextWaypoint!=null&&Mathf.Approximately(ship.ShipState.NextWaypoint.Time,float.MaxValue)) //makes sure that unreachable shipstates are auto-deleted
            {
               ship.ShipState.NextWaypoint.ClearTail();
            }
            PathDisplay.Instance.DrawPartialTrajectory(ship.trajectory, ship.trajectoryRotation, ship, step);
            ship.transform.position = pos;
            ship.transform.rotation = rotation;
            ship.MovementTarget = null; //last step always stops
            BattleScape.Instance.Ship.Path.GetComponent<CanvasGroup>().alpha = 1;
            TotalPathingTime = 0;
        }

        private static Waypoint CreateShipLegWaypoint(Ship ship)
        {
            var waypoint = Waypoint.Create(ship.transform.position, ship);
            waypoint.ShipVisual.ShipState = ship.StartState;
            waypoint.ShipVisual.transform.rotation = ship.transform.rotation;
            waypoint.Time = 0;
            ship.StartState.NextWaypoint = waypoint;
            return waypoint;
        }

        private static void WrapWaypointContainer(Waypoint waypoint)
        {
            var container = new GameObject(waypoint.ShipVisual.Parent.name + "WaypointContainer");
            waypoint.transform.SetParent(container.transform);
            container.transform.parent = BattleScape.Instance.WorldCanvas.transform;
        }
        public static void PlanPathStart()//should be ran once at the start of the turn
        {
            if (!BattleScape.Instance.Generated)
                return;

            foreach (var list in BattleScape.Instance.Ships.Values)
            {
                foreach (var ship in list)
                {
                    if (!Collisions.ContainsKey(ship))
                    {
                        Collisions.Add(ship, CollisionMarker.Create());
                    }
                    Collisions[ship].gameObject.SetActive(false);
                    if (!Legs.ContainsKey(ship))
                    {
                        var waypoint = CreateShipLegWaypoint(ship);
                        WrapWaypointContainer(waypoint);
                        Legs.Add(ship, waypoint);
                    }
                    else if (Legs[ship] == null)
                    {
                        var waypoint = CreateShipLegWaypoint(ship);
                        Legs[ship] = CreateShipLegWaypoint(ship);
                        WrapWaypointContainer(waypoint);
                    }
                }

            }
            TotalPathingTime = 0;

            var stepNumber = 0;
            var step = Time.fixedDeltaTime; //1.0 / PathResolution;//seconds
            step = step * ConfigurationManager.Instance.StepSizeMultiplier; //space it up to reduce clutter
            var totalTime = 0.0f;
            var pathingMaxSeconds = ConfigurationManager.Instance.TurnTime;
            foreach (var planet in Planets) //save location
            {
                totalTime = 0;
                var pos = planet.transform.position;
                for (int i = 0; i < ConfigurationManager.Instance.TurnsToCalculate; i++)
                {
                    while (true)
                    {
                        totalTime += step;
                        if (totalTime > pathingMaxSeconds) //exit after a certain time to prevent infinate loops)
                            break;
                        planet.TakeStep(step);
                    }
                    totalTime = 0;
                }
                PathDisplay.Instance.DrawPartialTrajectory(planet.trajectory, planet.trajectoryRotation, planet, step);
                planet.transform.position = pos;
            }
            foreach (var ship in Ships)
            {
                TotalPathingTime = 0;
                var wp = ship.ShipState.NextWaypoint;
                while (wp!=null)
                {
                    foreach (var mesh in wp.GetComponentsInChildren<MeshRenderer>())
                    {
                        mesh.enabled = true;
                    }
                    wp = wp.NextAction;
                }
                var pos = ship.transform.position;
                var rotation = ship.transform.rotation;
                var oldMoveTarget = ship.MovementTarget;
                totalTime = 0;
                for (int i = 0; i < ConfigurationManager.Instance.TurnsToCalculate; i++)
                {
                    while (true)
                    {
                        stepNumber++;
                        totalTime += step;
                        TotalPathingTime += step;
                        if (totalTime > pathingMaxSeconds) //exit after a certain time to prevent infinate loops)
                            break;
                        ship.TakeStep(step);
                        if (DetectCollision(ship, stepNumber))
                        {
                            Collisions[ship].gameObject.SetActive(true);
                            Collisions[ship].transform.position = ship.transform.position;
                            break;
                        }
                        CheckIfWaypointReached(ship, TotalPathingTime);
                    }
                    totalTime = 0;
                }
                PathDisplay.Instance.DrawPartialTrajectory(ship.trajectory, ship.trajectoryRotation, ship, step);
                ship.transform.position = pos;
                ship.transform.rotation = rotation; ship.MovementTarget = oldMoveTarget;
            }
            BattleScape.Instance.Ship.Path.GetComponent<CanvasGroup>().alpha = 1;
            TotalPathingTime = 0;
        }
        private static void CheckIfWaypointReached(Ship ship, float time)
        {
            var waypoint = ship.ShipState.NextWaypoint;
            var SAMEPLACE = 0.05f;
            if (waypoint != null && (ship.transform.position - waypoint.transform.position).sqrMagnitude < SAMEPLACE)//same place
            {
                waypoint.Time = time;
                waypoint.ShipVisual.transform.rotation = ship.transform.rotation;
            }
        }
        /*
                public static void CheckDirt(Ship ship)
                {
                    if (Legs.ContainsKey(ship))
                    {
                        var removeList = new List<TrailButton>(); //used to avoid changing the list while iterating over it
                        foreach (var trailButtom in Legs[ship])
                        {
                            if (trailButtom.CheckDirty(ship.ShipState))
                            {
                                removeList.Add(trailButtom);
                            }
                        }
                        for (int i = 0; i < removeList.Count; i++)
                        {
                            removeList.RemoveAt(i);
                        }
                    }
                }
                */
        private static bool DetectCollision(SpaceObject spaceObject, int stepNumber)
        {
            //TODO: improvements to algorithm- if distance is large, put to sleep for a number of steps.
            const float DETECTIONRADIUS = 0.8f; //TODO: change according to the object
            foreach (var ship in Ships)
            {
                if (ship.trajectory.Count >= stepNumber && (spaceObject.transform.position - ship.trajectory[stepNumber - 1]).sqrMagnitude < DETECTIONRADIUS * DETECTIONRADIUS && ship != spaceObject)
                {

                    return true;
                }
            }
            foreach (var planet in Planets)
            {
                if (planet.trajectory.Count >= stepNumber && (spaceObject.transform.position - planet.trajectory[stepNumber - 1]).sqrMagnitude < DETECTIONRADIUS * DETECTIONRADIUS && planet != spaceObject)
                {
                    return true;
                }
            }
            if ((spaceObject.transform.position - BattleScape.Instance.SolarSystem.Star.transform.position).sqrMagnitude < (DETECTIONRADIUS * DETECTIONRADIUS) * 1.5)
            {
                return true;
            }
            return false;
        }
        public static void ClearPaths()
        {
            BattleScape.Instance.Ship.ClearPath();
        }
        public static void Clear()
        {
            foreach (var planet in Planets)
            {
                planet.ClearPath();
            }
            ClearCollisions();
            foreach (var list in BattleScape.Instance.Ships.Values)
            {
                foreach (var ship in list)
                {
                    ship.ClearPath();
                }
            }
            foreach (var wp in Legs.Values)
            {
                var waypoint = wp;
                while (waypoint != null)
                {
                    foreach (var mesh in waypoint.GetComponentsInChildren<MeshRenderer>())
                    {
                        mesh.enabled = false;
                    }
                    waypoint = waypoint.NextAction;
                }
            }
            InputManager.Instance.MovementIndicator.gameObject.SetActive(false);
        }
        public static void OnPause()
        {
            var legKeys=Legs.Keys.ToList();
            foreach (var ship in legKeys)
            {
                if (ship == null)
                {
                    Legs.Remove(ship);
                }
                else
                {
                    var waypoint = CreateShipLegWaypoint(ship);
                    Legs[ship].Turn(ConfigurationManager.Instance.TurnTime, waypoint);
                    Legs[ship] = waypoint;
                    ship.ShipState.NextWaypoint = waypoint;
                }
            }
            foreach (var list in BattleScape.Instance.Ships.Values)
            {
                foreach (var ship in list)
                {
                    ship.ShipState.Position = ship.transform.position;
                    ship.ShipState.Rotation = ship.transform.rotation;
                    ship.StartState = ship.ShipState;
                }
            }
        }
        //public static void SortPaths()
        //{
        //    foreach (var list in Legs.Values)
        //    {
        //        if (list.Count > 1)
        //        {
        //            list.Sort(new TrailButtonTimeComparer()); //sorts list according to Time
        //        }
        //    }
        //}

        public static bool NextShipAction(Ship ship)
        {
            var waypoint = ship.ShipState.NextWaypoint;
            if (!TurnManager.Instance.GamePaused && Legs.ContainsKey(ship) && waypoint != null &&
    TurnManager.Instance.TimeLeftForTurn <= TurnManager.Instance.TurnTime - waypoint.Time)
            {
                ship.ShipState = waypoint.ShipVisual.ShipState;
                ship.ShipState.NextWaypoint = waypoint.NextAction;
                ship.AttackObject(ship.AttackTarget);
                return true;
            }
            else if
                (TurnManager.Instance.GamePaused && Legs.ContainsKey(ship) && waypoint != null &&
                 TotalPathingTime >= waypoint.Time)
            {
                Debug.Log("switching to next ship action, from stateid: "+ship.ShipState.Id +" to stateid: "+waypoint.ShipVisual.ShipState.Id);
                ship.ShipState = waypoint.ShipVisual.ShipState;
                ship.ShipState.NextWaypoint = waypoint.NextAction;
                ship.AttackObject(ship.AttackTarget);
                return true;
            }
            return false;
        }

        private static void ClearCollisions()
        {
            foreach (var collision in Collisions.Values)
            {
                if (collision != null)
                {
                    collision.gameObject.SetActive(false);
                }
            }
        }
    }
}
