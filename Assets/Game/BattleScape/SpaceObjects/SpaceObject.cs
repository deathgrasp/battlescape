using Assets.Game.BattleScape.SpaceObjects.SolarObjects;
using Assets.Game.BattleScape.VisualObjects;
using Assets.Game.BattleScape.VisualObjects.Highlight;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.BattleScape.SpaceObjects
{
    public class SpaceObject : MonoBehaviour
    {
        [HideInInspector]
        public GameObject Path=null;
        [HideInInspector]
        public List<Vector3> trajectory = new List<Vector3>();
        public List<Quaternion> trajectoryRotation = new List<Quaternion>();
        public bool ActiveHiglight;
        public bool ActiveTarget;
        public float MaxHealth;
        public float Health;
        public float AffectedByGravityPercent = 1;
        public List<SolarObject> PulledToward;

        public virtual void ChangeHealth(float addHealth)
        {
            Health = Mathf.Clamp(Health + addHealth, 0, MaxHealth);
        }

        public void RestoreHealthToFull()
        {
            ChangeHealth(MaxHealth);
        }

        #region Highlights
        private SelectionHighlight _selectionHighlight;
        public SelectionHighlight SelectionHighlight
        {
            get
            {
                return _selectionHighlight ?? (_selectionHighlight=SelectionHighlight.Create(transform));
            }
        }
        public virtual void Highlight(bool active)
        {
            ActiveHiglight = active;
            SelectionHighlight.gameObject.SetActive(ActiveHiglight);
        }

        private TargetHighlight _targetnHighlight;
        public TargetHighlight TargetHighlight
        {
            get
            {
                return _targetnHighlight ?? (_targetnHighlight=TargetHighlight.Create(transform));
            }
        }
        #endregion Highlights

        public virtual void SetTarget(bool active)
        {
            ActiveTarget = active;
            TargetHighlight.gameObject.SetActive(ActiveTarget);
        }

        public virtual void Start()
        {
            Highlight(false);
            SetTarget(false);
        }

        public void OnTriggerEnter(Collider collider)
        {
            
            var solarObject = collider.gameObject.GetComponentInParent<SolarObject>();
            if (solarObject != null)
            {
                PulledToward.Add(solarObject);
            }
        }
        public void OnTriggerExit(Collider collider)
        {
            var solarObject = collider.gameObject.GetComponentInParent<SolarObject>();
            if (solarObject != null)
            {
                PulledToward.Remove(solarObject);
            }
        }
        public void TakeStep(float step)//acts as interface
        {
        }

        public void ClearPath()//acts as interface
        {
        }
    }
}
