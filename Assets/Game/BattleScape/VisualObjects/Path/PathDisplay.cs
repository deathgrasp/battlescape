using UnityEngine;
using Assets.Utils;
using System.Collections.Generic;
using Assets.Game.BattleScape.SpaceObjects;
using UnityEngine.UI;
namespace Assets.Game.BattleScape.VisualObjects.Path
{
    public class PathDisplay : UnitySingleton<PathDisplay>
    {
        public GameObject TajectoryContainer;
        public Dictionary<List<Vector3>, List<List<TrailButton>>> TrajectoryIndicators = new Dictionary<List<Vector3>, List<List<TrailButton>>>();

        public TrailButton GetLastTrailPoint(List<Vector3> trajectory)
        {
            if (TrajectoryIndicators[trajectory].Count >= 1 && TrajectoryIndicators[trajectory][0].Count >= 1)
            {
                return TrajectoryIndicators[trajectory][0][(TrajectoryIndicators[trajectory][0].Count) / ConfigurationManager.Instance.TotalGameTurns];//dictionary-->list's list--> first list(always only one)-->last object
            }
            else
            {
                return null;
            }
        }

        public void DrawPartialTrajectory(List<Vector3> trajectory, List<Quaternion> trajectoryRotation, SpaceObject parent, float step)
        {
            var indicatorSize = new Vector3(0.1f, 0.1f, 0.1f);

            if (!TajectoryContainer)
                TajectoryContainer = new GameObject("TajectoryContainer");
            if (!TrajectoryIndicators.ContainsKey(trajectory))
                TrajectoryIndicators.Add(trajectory, new List<List<TrailButton>>());

            var currentList = new List<TrailButton>();
            TrajectoryIndicators[trajectory].Add(currentList);
            var path = new GameObject("Path");
            var canvasGroup=path.AddComponent<CanvasGroup>();
            canvasGroup.alpha = ConfigurationManager.Instance.NonSelectedUIVisibility;
            parent.Path = path;
            TrailButton trailButton = null;
            for (int i = 0; i < trajectoryRotation.Count; i++)
            {
                trailButton = TrailButton.Create(path);
                path.transform.SetParent(BattleScape.Instance.WorldCanvas.transform);
                trailButton.transform.position = trajectory[i];
                trailButton.transform.LookAt(transform.position + BattleScapeCamera.Instance.transform.rotation * Vector3.forward, BattleScapeCamera.Instance.transform.rotation * Vector3.up);
                trailButton.transform.rotation *= Quaternion.Euler(0, 0, trajectoryRotation[i].eulerAngles.y);
                trailButton.Parent = parent;
                trailButton.Position = trajectory[i];
                trailButton.Rotation = trajectoryRotation[i];
                trailButton.name = "TrailButton " + _trailID;
                trailButton.Time = i * step;
                _trailID++;
                currentList.Add(trailButton);
            }
            if (trailButton != null && step > 0)
            {

                var steps = (int)(ConfigurationManager.Instance.TurnTime / step);
                if (step > 0)
                {
                }
                for (int i = steps; i < currentList.Count; i++)
                {
                    currentList[i].GetComponent<Image>().color = Color.blue;
                }

                for (int i = steps - 1; i < currentList.Count; i += steps)
                {
                    currentList[i].GetComponent<Image>().color = Color.yellow;
                }

            }
        }
        private int _trailID = 0;
        public void ClearTrajectory(List<Vector3> trajectory)
        {
            foreach (var list in TrajectoryIndicators[trajectory])
            {
                if (list.Count > 0)
                {
                    Destroy(list[0].transform.parent.parent.gameObject);//trailbutton-->trail container-->path container
                }
                list.Clear();
            }
            TrajectoryIndicators[trajectory].Clear();
        }

        public void ClearAllTrajectories()
        {
            foreach (var trajectory in TrajectoryIndicators)
            {
                foreach (var list in trajectory.Value)
                {
                    if (list.Count > 0)
                    {
                        Destroy(list[0].transform.parent.parent.gameObject);//trailbutton-->trail container-->path container
                    }
                    list.Clear();
                }
                trajectory.Value.Clear();
                trajectory.Key.Clear();
            }
            TrajectoryIndicators.Clear();
        }
    }
}
