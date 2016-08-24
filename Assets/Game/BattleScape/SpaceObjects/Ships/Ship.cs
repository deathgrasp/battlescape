using System;
using Assets.Game.BattleScape.Effects;
using Assets.Game.BattleScape.Players;
using Assets.Game.BattleScape.SpaceObjects.Ships.Components;
using Assets.Game.BattleScape.VisualObjects;
using UnityEngine;
using System.Collections.Generic;
using Assets.Game.BattleScape.SpaceObjects.SolarObjects;
using Assets.Game.BattleScape.SpaceObjects.WeaponFire;
using Assets.Game.BattleScape.VisualObjects.Path;


/*TODO: fix UI Fixed- the endgame canvas blocked the raycast.
 * make sure commands are always excuted
 * make space ships continue moving forward (instead of staying still)
 * new ships. shotgun ship, exploding (aoe) missile
 * 
 */


namespace Assets.Game.BattleScape.SpaceObjects.Ships
{
    public partial class Ship : SpaceObject
    {
        #region Ship State
        public Vector3? MovementTarget
        {
            get { return ShipState.MovementTarget; }
            set { ShipState.MovementTarget = value; }
        }
        public float RotationSpeed
        {
            get { return ShipState.RotationSpeed; }
            set { ShipState.RotationSpeed = value; }
        }
        public float MovementSpeed
        {
            get { return ShipState.MovementSpeed; }
            set { ShipState.MovementSpeed = value; }
        }
        public SpaceObject AttackTarget
        {
            get { return ShipState.AttackTarget; }
            set { ShipState.AttackTarget = value; }
        }

        public Vector3 GunShotTarget
        {
            get { return ShipState.GunShotTarget; }
            set { ShipState.GunShotTarget = value; }
        }

        public float ShotTimer
        {
            get { return ShipState.ShotTimer; }
            set { ShipState.ShotTimer = value; }
        }

        private int _bulletCounter = 0;
        #endregion Ship State
        public ShipState ShipState {
            get; set; }
        private float _stopMovementThreshold = 0.1f;

        public float OriginalTrailRendererTime
        {
            get;
            private set;
        }

        public bool IsInvincible;

            public BSPlayer Player;

            private TrailRenderer _trailRenderer;

            public event Action<Ship> OnMove;

        public TrailRenderer TrailRenderer
        {
            get
            {
                if (_trailRenderer == null)
                {
                    _trailRenderer = GetComponent<TrailRenderer>();
                    OriginalTrailRendererTime = TrailRenderer.time;
                }

                return _trailRenderer;
            }
        }
        public int ActionsIndex=0;
        private void Update()
        {
            //checks if the game met the requirements to change to the next action. if it does, switch.
            PathPlanner.NextShipAction(this);
            if (IsInvincible)
                Player.PlayerLives = 2;


            Move(Time.deltaTime);

            if (Health <= 0)
            {
                ShipDestruction();
            }
            GravityPull(Time.deltaTime);
            var gs = GunShotTarget;

            Shoot();
            
        }

        private void Shoot()
        {
            if (ShotTimer <= 0 && _bulletCounter<=GunShot.BASESHOTBURST)
            {                
                    if (GunShotTarget != Vector3.zero && TurnManager.Instance.GamePaused == false)
                    {
                        GunShot.Create(transform.position, GunShotTarget - transform.position, GunShot.BASEDAMAGE,
                            GunShot.BASESPEED);
                        Debug.Log("shot fired!");
                    _bulletCounter += 1;
                    ShotTimer += GunShot.BASESHOOTDELAY;
                    if(_bulletCounter>=GunShot.BASESHOTBURST)
                        GunShotTarget = Vector3.zero;

                }
            }
                ShotTimer -= Time.deltaTime;
        }

        public void ShootAt(Vector3 position)
        {
            GunShotTarget = position;
            if (_bulletCounter >= GunShot.BASESHOTBURST)
            {
                _bulletCounter = 0;
            }
            if (ShotTimer <= 0)
            {
                ShotTimer = 0;
            }
        }
        private void Move(float step)
        {
            if (MovementTarget.HasValue)
            {
                if (Mathf.Approximately(TrailRenderer.time, 0f))
                    TrailRenderer.time = OriginalTrailRendererTime;

                var direction = (MovementTarget.Value - transform.position);

                var rotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, step * RotationSpeed);
                if ((MovementTarget.Value - transform.position).sqrMagnitude <= _stopMovementThreshold * _stopMovementThreshold)
                {
                    MovementTarget = null;
                    InputManager.Instance.MovementIndicator.gameObject.SetActive(false);
                }
                else
                {
                    transform.position += transform.forward * step * MovementSpeed;
                }
            }
        }

        private void GravityPull(float step)
        {
            foreach (var solarObject in PulledToward)
            {
                var direction = (transform.position - solarObject.transform.position).normalized;
                transform.position -= direction * solarObject.GravityPullStrangth * AffectedByGravityPercent * step;
            }
        }


        new public void TakeStep(float step)
        {
            PathPlanner.NextShipAction(this);
            Move(step);
            GravityPull(step);
            trajectory.Add(transform.position);
            trajectoryRotation.Add(transform.rotation);
        }

        new public void ClearPath()
        {
            PathDisplay.Instance.ClearTrajectory(trajectory);
            trajectory.Clear();
            trajectoryRotation.Clear();
        }

        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log("collision");
            var spaceObject = collision.gameObject.GetComponent<SpaceObject>();
            if (spaceObject != null)
            {
                if (spaceObject is SolarObject)
                {
                    ShipDestruction();
                }
                else if (spaceObject is Projectile)
                {
                    var projectile = spaceObject as Projectile;
                    ChangeHealth(-projectile.Damage);
                }
                else
                {
                    Debug.Log(spaceObject.Health);
                    DamageEffectsFactory.Instance.StartDamageEffect(collision.contacts[0].point, Quaternion.identity, this);
                    ChangeHealth(-( spaceObject.Health * ConfigurationManager.Instance.RammingDamageModifier + ConfigurationManager.Instance.RammingExtraDamage));
                }
            }
        }

        private void ShipDestruction()
        {
            var destroy = false;
            DeathEffectsFactory.Instance.StartDeathEffect(transform.position, Quaternion.identity);
            if (Player.IsHuman)
            {
                PlayerDestroyedCleanup();
                Player.PlayerLives -= 1;
                TurnManager.Instance.Pause();
                if (Player.PlayerLives > 0)
                {
                    Respawn();
                }
                else
                {
                    destroy = true;
                    BattleScape.Instance.Ships[Player].Remove(this);
                    if (BattleScape.Instance.Ships[Player].Count == 0)
                    {
                        BattleScape.Instance.GameOver();
                    }
                }
            }
            else
            {
                destroy = true;
                BattleScape.Instance.ActiveEnemyShips -= 1;
            }

            if (destroy)
                Destroy(gameObject);
        }



        private Laser _laserPrefab;
        private Laser _laser;

        public Laser Laser
        {
            get
            {
                return _laser
                       ?? (_laser = Instantiate(_laserPrefab
                                           ?? (_laserPrefab = Resources.Load<Laser>("BattleScape/Laser"))));
            }
        }

        public ShipState StartState { get; set; }

        public void Init(BSPlayer player, float startingHealth)
        {
            ShipState = new ShipState();
            StartState = ShipState;
            PathDisplay.Instance.DrawPartialTrajectory(trajectory, trajectoryRotation, this,0f);
            Player = player;
            MaxHealth = startingHealth;
            ShipState.Position = transform.position;
            ShipState.Rotation = transform.rotation;
        }

        public override void Start()
        {
            base.Start();
            Laser.Init(this);
            //Laser.gameObject.SetActive(false);
        }
        public void RestoreState()
        {
            TrailRenderer.time = 0;
            ShipState = StartState;
            transform.position = ShipState.Position;
            transform.rotation = ShipState.Rotation;
            AttackObject(AttackTarget);
            TrailRenderer.time = OriginalTrailRendererTime;
           // PathPlanner.CheckDirt(this); //removes clean buttoms and actions from the list
           // ShipState.NextWaypoint = PathPlanner.Legs[this];
        }

        public void AttackObject(SpaceObject target)
        {
            AttackTarget = target;
            Laser.gameObject.SetActive(true);
            Laser.Target = target;
            Laser.On = true;
        }
        public SpaceObject CurrentTarget()
        {
            return Laser.Target;
        }

        public void Respawn()
        {
            RestoreHealthToFull();
            //TODO: maybe respawn at spawn point or respawn at same place with safeguards
            TrailRenderer.time = 0f;
            transform.position = new Vector3(2f, 0f, 2f);

        }
        private void PlayerDestroyedCleanup()
        {
            //TODO: clear targeting indicator
            AttackObject(null);
            MovementTarget = null;
            if (InputManager.Instance.CurrentTarget != null)
            {
                InputManager.Instance.CurrentTarget.SetTarget(false);
            }
            if (InputManager.Instance.MovementIndicator != null)
            {
                InputManager.Instance.MovementIndicator.gameObject.SetActive(false);
            }

        }
        public override void ChangeHealth(float addHealth)
        {
            base.ChangeHealth(addHealth);

            if (Player.IsHuman)
                GUIManager.Instance.UpdateHealthBar(Health);
        }
        public static void changeActiveShip(Ship ship)
        {
            BattleScape.Instance.Ship.Path.GetComponent<CanvasGroup>().alpha = ConfigurationManager.Instance.NonSelectedUIVisibility;

            ship.Path.GetComponent<CanvasGroup>().alpha = 1;
            BattleScape.Instance.Ship.RestoreState();
            BattleScape.Instance.Ship = ship;
            if (ship.MovementTarget != null)
            {
                InputManager.Instance.MovementIndicator.transform.position = ship.MovementTarget.Value;
            }

            InputManager.Instance.CurrentTarget = ship.CurrentTarget();


        }

        private static Ship _friendlyShipPrefab;
        public static Ship FriendlyShipPrefab
        {
            get
            {
                return (_friendlyShipPrefab ?? (_friendlyShipPrefab = Resources.Load<Ship>("BattleScape/Ship")));
            }
        }
        private Ship _enemyShipPrefab;

        public Ship EnemyShipPrefab
        {
            get { return _enemyShipPrefab ?? (_enemyShipPrefab = Resources.Load<Ship>("BattleScape/EnemyShip")); }
        }


    }
}