using Assets.Game.BattleScape.SpaceObjects;
using Assets.Game.BattleScape.SpaceObjects.Ships;
using Assets.Game.BattleScape.SpaceObjects.WeaponFire;
using Assets.Game.BattleScape.VisualObjects.Path;
using Assets.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.BattleScape.VisualObjects
{    /// <summary>
     /// In charge of inputs and their effects, such as right click.</summary>

    
    public class InputManager : UnitySingleton<InputManager>
    {

        private static TargetIndicator _targetIndicatorPrefab;

        public static TargetIndicator TargetIndicatorPrefab
        {
            get
            {
                return _targetIndicatorPrefab ?? (_targetIndicatorPrefab = Resources.Load<TargetIndicator>("BattleScape/TargetIndicator"));
            }
        }


        public bool DelayFrame = false;
        public SpaceObject LastHitSpaceObject;
        public SpaceObject CurrentTarget;
        private MovementIndicator _movementIndicator;
        public MovementIndicator MovementIndicator { get { return _movementIndicator ?? (_movementIndicator = Instantiate(MovementIndicator.MovementIndicatorPrefab)); } }
        public bool LocationAction = false;
    public bool WaypointAction = false;
    public bool GunShotAction = false;
        public bool SpaceObjectAction = false;
        public Waypoint WaypointForAction;
        private void Update()
        {
            if (!DelayFrame)
            {
                if (TurnManager.Instance.GamePaused)
                {
                    if (Input.GetMouseButtonDown(1))
                    {
                        if (SpaceObjectAction || LocationAction) //cancel action
                        {
                            SpaceObjectAction = false;
                            LocationAction = false;
                            return;
                        }
                    }
                    if (LastHitSpaceObject != null)
                    {
                        LastHitSpaceObject.Highlight(false);
                    }

                    RaycastHit hit;
                    var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (!LocationAction && Physics.Raycast(ray, out hit, 1000f))
                    {
                        var spaceObject = hit.transform.GetComponent<SpaceObject>();
                        if (spaceObject != null)
                        {
                            LastHitSpaceObject = spaceObject;
                            LastHitSpaceObject.Highlight(true);
                        }

                        if (Input.GetMouseButtonDown(0))
                        {
                            Debug.Log("Clicking");
                            if (SpaceObjectAction)
                            {
                                //TODO: change to use ActionManager to perform the actions. Need to see how to do it well.
                                var hitShip = LastHitSpaceObject as Ship;
                                if (hitShip != null && BattleScape.Instance.Ships[TurnManager.Instance.CurrentPlayer].Contains(hitShip))//no attack on friendly target. currently only attack option
                                    return;
                                if (CurrentTarget != false)
                                {
                                    CurrentTarget.SetTarget(false);
                                }
                                CurrentTarget = spaceObject;
                                BattleScape.Instance.Ship.AttackObject(CurrentTarget);
                                CurrentTarget.SetTarget(true);
                                if (!(Input.GetKeyDown(KeyCode.RightShift) || Input.GetKeyDown(KeyCode.LeftShift)))
                                {
                                    BattleScape.Instance.Ship.RestoreState();
                                    PathPlanner.PlanPath();
                                    SpaceObjectAction = false; //one action, unless shift is pressed
                                }
                            }
                            //cases depending on what we selected
                            if (spaceObject is Ship)
                            {
                                var hitShip = spaceObject as Ship;
                                if (BattleScape.Instance.Ships[TurnManager.Instance.CurrentPlayer].Contains(hitShip))
                                {
                                    Ship.changeActiveShip(hitShip);
                                }
                            }
                            else if (hit.transform.GetComponent<ShipVisual>() != null)
                            {
                                var shipVisual = hit.transform.GetComponent<ShipVisual>();
                                //pops the menu up
                                ConfigurationManager.Instance.ActionMenu.gameObject.SetActive(true);
                                ConfigurationManager.Instance.ActionMenu.transform.position = shipVisual.transform.position+new Vector3(0,-1,0);
                                WaypointForAction = shipVisual.GetComponentInParent<Waypoint>();
                                //sets the ship to be active and located at the waypoint
                                var ship = shipVisual.Parent;
                                Ship.changeActiveShip(ship);
                                Debug.Log("[ship][in input]old shooting towards: "+ship.ShipState.GunShotTarget+" at  with state id: "+ship.ShipState.Id);
                                Debug.Log("[BS ship][in input]old shooting towards: " + BattleScape.Instance.Ship.ShipState.GunShotTarget + " at  with state id: " + BattleScape.Instance.Ship.ShipState.Id);

                                ship.ShipState = shipVisual.ShipState;

                                ship.transform.position = shipVisual.GetComponentInParent<Waypoint>().transform.position;
                                ship.transform.rotation = shipVisual.transform.rotation;
                                ship.AttackObject(ship.AttackTarget);
                                BattleScape.Instance.Ship = ship;
                                Debug.Log("[ship][in input]new shooting towards: " + ship.ShipState.GunShotTarget + " at  with state id: " + ship.ShipState.Id);
                                Debug.Log("[BS ship][in input]old shooting towards: " + BattleScape.Instance.Ship.ShipState.GunShotTarget + " at  with state id: " + BattleScape.Instance.Ship.ShipState.Id);

                            }
                        }
                        else if (Input.GetMouseButtonDown(1))
                        {
                            Debug.Log("Right Clicking");
                            //can't target friendly ships
                            if (spaceObject!=null&& !BattleScape.Instance.Ships[TurnManager.Instance.CurrentPlayer].Contains(spaceObject as Ship))
                            {
                                if (CurrentTarget != false)
                                {
                                    CurrentTarget.SetTarget(false);
                                }
                                CurrentTarget = spaceObject;
                                BattleScape.Instance.Ship.AttackObject(CurrentTarget);
                                CurrentTarget.SetTarget(true);
                            }
                            else if (hit.transform.GetComponent<ShipVisual>() != null)
                            {
                                var waypoint = hit.transform.GetComponent<ShipVisual>().GetComponentInParent<Waypoint>();
                                if (!PathPlanner.Legs.ContainsValue(waypoint))
                                {
                                    BattleScape.Instance.Ship.RestoreState();
                                    PathPlanner.ClearPaths();
                                    PathPlanner.ResetWaypointPathTime(waypoint);

                                    waypoint.Destroy();
                                    PathPlanner.PlanPath();
                                }
                            }
                        }

                    }//no raycast hit
                    else
                    {
                        var distanceToPlane = 0f;
                        if (BattleScape.Instance.Plane.Raycast(ray, out distanceToPlane))
                        {
                            var ship = BattleScape.Instance.Ship;
                            if (LocationAction && Input.GetMouseButtonDown(0))
                            {
                                var position = ray.GetPoint(distanceToPlane);
                                if (WaypointAction)
                                {
                                    var waypoint = Waypoint.Create(position, ship);
                                    if (WaypointForAction != null)
                                    {
                                        WaypointForAction.AddWaypoint(waypoint, true);
                                        PathPlanner.ResetWaypointPathTime(WaypointForAction);
                                    }
                                    else
                                    {
                                        PathPlanner.Legs[ship].AddAtEnd(waypoint);
                                    }

                                    MovementIndicator.gameObject.SetActive(true);
                                    MovementIndicator.transform.position = position;
                                    WaypointForAction.ShipVisual.ShipState.MovementTarget = position;

                                    PathPlanner.ClearPaths();
                                    ship.RestoreState();
                                    PathPlanner.PlanPath();
                                    LocationAction = false;
                                    WaypointAction = false;
                                }
                                else if (GunShotAction)
                                {
                                    BattleScape.Instance.Ship.ShootAt(position);
                                    Instantiate(TargetIndicatorPrefab, position,Quaternion.identity);
                                    Debug.Log("shooting towards: "+position.ToString()+ " for state id: "+BattleScape.Instance.Ship.ShipState.Id);
                                    ship.RestoreState();
                                    PathPlanner.PlanPath();
                                    LocationAction = false;
                                    GunShotAction = false;

                                }
                                
                            }
                            if (!LocationAction && Input.GetMouseButtonDown(1))
                            {
                                Debug.Log("right click");
                                const float SAMEPLACE = 0.005f;
                                if ((ship.transform.position - ship.StartState.Position).sqrMagnitude < SAMEPLACE)//right click on empty place=cancel.
                                {
                                    PathPlanner.TotalPathingTime = float.MaxValue;

                                    var waypoint = Waypoint.Create(ray.GetPoint(distanceToPlane), ship);
                                    PathPlanner.Legs[ship].AddAtEnd(waypoint);
                                    MovementIndicator.gameObject.SetActive(true);
                                    var position = ray.GetPoint(distanceToPlane);
                                    MovementIndicator.transform.position = position;
                                    ship.MovementTarget = position;

                                    PathPlanner.ClearPaths();
                                    ship.RestoreState();
                                    PathPlanner.PlanPath();
                                }
                                else
                                {
                                    PathPlanner.TotalPathingTime = 0;
                                    Debug.Log("restore");
                                    ship.RestoreState();

                                }
                            }
                        }
                    }
                }
            }//delayframe
            else
            {
                DelayFrame = false;
            }
            var mouseWheel = Input.GetAxis("Mouse ScrollWheel");
            var panDirectionX = 0f;
            var panDirectionZ = 0f;

            if (Input.GetKey(KeyCode.UpArrow))
            {
                panDirectionZ += 1f;
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                panDirectionZ -= 1f;
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                panDirectionX -= 1f;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                panDirectionX += 1f;
            }
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                NextShipScript.Instance.NextShip();
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                TurnManager.Instance.EndTurn();
            }
            BattleScapeCamera.Instance.CameraPositionChange(panDirectionX, mouseWheel * -1, panDirectionZ);
        }
    }
}