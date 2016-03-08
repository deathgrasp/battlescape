using System;
using Assets.Game.BattleScape.SpaceObjects;
using Assets.Game.BattleScape.SpaceObjects.Ships;
using UnityEngine;
namespace Assets.Game.BattleScape.VisualObjects.Path
{
    public class Waypoint : MonoBehaviour
    {
        public ShipVisual ShipVisual;
        public Waypoint NextAction;
        public Waypoint PreviousAction;
        public float Time = float.MaxValue;//invariant on time: nextaction.time>this.time>previousAction.time
        private static Waypoint _waypointPrefab;
        private static Waypoint WaypointPrefab
        {
            get
            {
                return (_waypointPrefab ?? (_waypointPrefab = Resources.Load<Waypoint>("BattleScape/UI/Waypoint")));
            }
        }
        public static Waypoint Create(Ship parent)
        {
            var waypoint = Instantiate(WaypointPrefab);
            waypoint.transform.SetParent(BattleScape.Instance.WorldCanvas.transform);
            waypoint.transform.position = Vector3.zero;
            waypoint.transform.rotation = Quaternion.identity;
            waypoint.ShipVisual.ShipState = new ShipState();
            waypoint.ShipVisual.Parent = parent;
            return waypoint;
        }
        public static Waypoint Create(Vector3 position, Ship parent)
        {
            var waypoint = Create(parent);
            waypoint.transform.position = position;

            return waypoint;
        }
        public void Destroy()
        {
            Remove();
            Destroy(gameObject);
        }

        public void AddAtEnd(Waypoint last)
        {
            var point = GetLast(this);

            point.NextAction = last;
            last.PreviousAction = point;
            last.transform.SetParent(point.transform.parent);

        }

        public Waypoint GetLast(Waypoint chain)
        {
            while (chain.NextAction != null)
            {
                chain = chain.NextAction;
            }
            return chain;
        }
        public void Remove()
        {
            if (NextAction != null)
            {
                NextAction.PreviousAction = PreviousAction;
            }
            if (PreviousAction != null)
            {
                PreviousAction.NextAction = NextAction;
                PreviousAction.ShipVisual.ShipState.MovementTarget = ShipVisual.ShipState.MovementTarget;
            }
        }
        public void AddWaypoint(Waypoint next, bool afterThis = false)//If the added waypoint already has a time, afterthis should be false. if you want to add after this point regardless of time, it should be true.
        {
            if (afterThis)
            {
                next.NextAction = NextAction;
                next.PreviousAction = this;
                NextAction = next;
                next.ShipVisual.ShipState.MovementTarget = ShipVisual.ShipState.MovementTarget;
            }
            else
            {
                AddWaypoint(next, next.Time);
            }
            transform.SetParent(PreviousAction.transform.parent);
        }

        private void ResolveAddWaypoint(Waypoint next)
        {
            next.NextAction = NextAction;
            NextAction = next;
        }
        private void AddWaypoint(Waypoint next, float time)
        {
            if (time < Time)
            {
                if (PreviousAction != null)
                {
                    PreviousAction.AddWaypoint(next, time);
                }
                else
                {
                    next.ResolveAddWaypoint(this);
                }
            }
            else if (NextAction != null && NextAction.Time < time)
            {
                NextAction.AddWaypoint(next, time);
            }
            else
            {
                ResolveAddWaypoint(next);
            }
        }
        public void Clear()
        {
            var action = GetFirst(this);
            
            action.ClearTail();
        }
        public void ClearTail()
        {
            if (NextAction != null)
            {
                NextAction.ClearTail();
            }
            Destroy(gameObject);

        }

        public Waypoint GetFirst(Waypoint chain)
        {
            while (chain.PreviousAction != null)
            {
                chain = chain.PreviousAction;
            }
            return chain;
        }
        public void Turn(float turnTime,Waypoint first) //reduce time by turn time, removes negative timed waypoints, and connects them to waypoint.
        {
            var point = GetFirst(this);
            Waypoint second=null;
            first.transform.SetParent(point.transform.parent);
            while (point!=null)
            {
                point.Time -= turnTime;
                if (point.Time <= 0)
                {
                    point.Destroy();
                    if (point.NextAction != null&&point.NextAction.Time-turnTime>=0)//connects the chain (if any left) to  "first" waypoint
                    {
                        second = point.NextAction;
                        first.ShipVisual.ShipState = point.ShipVisual.ShipState;
                    }
                }
                point = point.NextAction;
            }
            if (second!=null)
            {
                second.PreviousAction = first;
                first.NextAction = second;
                
            }
        }
    }
}
