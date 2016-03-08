using Assets.Game.BattleScape.SpaceObjects.Ships.Components;
using Assets.Game.BattleScape.VisualObjects;
using Assets.Game.BattleScape.VisualObjects.Path;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.BattleScape.SpaceObjects.SolarObjects
{
    public class Planet : SolarObject
    {
        private float _turnRate;
        public Status PlanetStatus;

        [HideInInspector]
        public MeshRenderer MeshRenderer;
        public void Init(float turnRate)
        {
            _turnRate = turnRate;
            MaxHealth = BattleScape.Instance.PlanetMaxHealth;
            RestoreHealthToFull();
            PlanetStatus = Status.Healthy;
            MeshRenderer = GetComponent<MeshRenderer>();
            _oldHealth = Health;
            PathDisplay.Instance.DrawPartialTrajectory(trajectory,trajectoryRotation,this,0f);
        }

        private void Update()
        {

            if (!BattleScape.Instance.Generated)
                return;

            Move(Time.deltaTime);
            if (Health <= 0 && PlanetStatus != Status.Destroyed)
            {
                PlanetStatus = Status.Destroyed;
                BattleScape.Instance.SolarSystem.LivingPlanets -= 1;
                if (BattleScape.Instance.SolarSystem.LivingPlanets == 0)
                {
                    BattleScape.Instance.HumanPlayer.PlayerLives = 0;
                    BattleScape.Instance.Ship.ChangeHealth(-BattleScape.Instance.Ship.MaxHealth);
                }
            }
            else if (Health <= MaxHealth / 2 && PlanetStatus != Status.Destroyed)
            {
                PlanetStatus = Status.Damaged;
            }

            MeshRenderer.material = StatusMaterial();
            StartCoroutine(ShowUnderAttack());
        }

        private void Move(float step)
        {
            transform.RotateAround(transform.parent.position, Vector3.up, _turnRate * step);
        }
        new public void TakeStep(float step)
        {
            Move(step);
            trajectory.Add(transform.position);
            trajectoryRotation.Add(transform.rotation);
        }
        new public void ClearPath()
        {
            PathDisplay.Instance.ClearTrajectory(trajectory);
            trajectory.Clear();
            trajectoryRotation.Clear();
        }
        #region oldPlanPath
        //public void PlanPath()
        //{
        //var startingPos = transform.position;

        //var PathingMaxSeconds = 5f;
        //var step = Time.fixedDeltaTime; //1.0 / PathResolution;//seconds
        //step = step * 20; //space it up to reduce clutter
        //var totalTime = 0.0;
        //while (true)
        //{
        //    totalTime += step;
        //    if (totalTime > PathingMaxSeconds) //exit after a certain time to prevent infinate loops)
        //        break;
        //    transform.RotateAround(transform.parent.position, Vector3.up, _turnRate * step);
        //    trajectory.Enqueue(transform.position);
        //}
        //transform.position = startingPos;
        //GUIManager.DrawPartialTrajectory(trajectory);
        //}
        #endregion oldPlanPath
        private float _oldHealth;
        private IEnumerator ShowUnderAttack()
        {
            if (Health < _oldHealth)
            {
                UnderAttack(true);
                yield return new WaitForSeconds(0.1f);
                UnderAttack(false);
                yield return new WaitForSeconds(0.1f);
                _oldHealth = Health;
            }
        }


        private void OnCollisionEnter(Collision collision)
        {
            var spaceObject = collision.gameObject.GetComponent<SpaceObject>();
            if (spaceObject != null)
            {
                spaceObject.ChangeHealth(spaceObject.Health*ConfigurationManager.Instance.RammingDamageModifier+ConfigurationManager.Instance.RammingExtraDamage);
            }
            else if (spaceObject is Projectile)
            {
                var projectile = spaceObject as Projectile;
                ChangeHealth(-projectile.Damage);
            }
        }
        #region PlanetStatus
        private Material StatusMaterial()
        {
            switch (PlanetStatus)
            {
                case Status.Healthy:
                    return HealthyPlanetMaterialPrefab;
                case Status.Damaged:
                    return DamagedPlanetMaterialPrefab;
                case Status.Destroyed:
                    return DestroyedPlanetMaterialPrefab;
                default:
                    return HealthyPlanetMaterialPrefab;
            }
        }
        public enum Status
        {
            Healthy, Damaged, Destroyed
        }

        private Material _healthyPlanetMaterialPrefab;

        public Material HealthyPlanetMaterialPrefab
        {
            get { return _healthyPlanetMaterialPrefab ?? (_healthyPlanetMaterialPrefab = Resources.Load<Material>("BattleScape/Planet")); }
        }

        private Material _damagedPlanetMaterialPrefab;

        public Material DamagedPlanetMaterialPrefab
        {
            get { return _damagedPlanetMaterialPrefab ?? (_damagedPlanetMaterialPrefab = Resources.Load<Material>("BattleScape/DamagedPlanet")); }
        }

        private Material _destroyedPlanetMaterialPrefab;

        public Material DestroyedPlanetMaterialPrefab
        {
            get { return _destroyedPlanetMaterialPrefab ?? (_destroyedPlanetMaterialPrefab = Resources.Load<Material>("BattleScape/DestroyedPlanet")); }
        }

        private bool _underAttack = false;
        private PlanetUnderAttack _planetUnderAttackPrefab;
        private PlanetUnderAttack _planetUnderAttack;

        public PlanetUnderAttack PlanetUnderAttack
        {
            get
            {
                if (_planetUnderAttack == null)
                {
                    _planetUnderAttack = Instantiate(_planetUnderAttackPrefab ??
                                (_planetUnderAttackPrefab = Resources.Load<PlanetUnderAttack>("BattleScape/PlanetUnderAttack")));

                    _planetUnderAttack.transform.position = transform.position;
                    _planetUnderAttack.transform.SetParent(transform);
                }

                return _planetUnderAttack;
            }
        }

        public virtual void UnderAttack(bool underAttack)
        {
            _underAttack = underAttack;
            PlanetUnderAttack.gameObject.SetActive(_underAttack);
        }
        #endregion PlanetStatus
    }
}
