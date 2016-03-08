using Assets.Game.BattleScape.Effects;
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace Assets.Game.BattleScape.SpaceObjects.Ships.Components
{
    public class Laser : MonoBehaviour
    {
        public float LaserDPS;
        public Boolean On;
        public SpaceObject Target;
        public float RayMaxDistance;
        public Ship Ship;
        public float Cooldown;
        private float _cooldownDuration;
        public float AttackDuration;
        [HideInInspector]
        public LineRenderer LineRenderer;
        private const float _baseWidth = 0.1f;//The laser's base width. Change to be dynamic if we find a proper way (can't access LineRenderer's width field.
        private float explosionTimer = 0f;
        private float explosionDelay = 0.1f;

        private void Start()
        {
            LineRenderer = GetComponent<LineRenderer>();
            LineRenderer.enabled = false;
            LineRenderer.material = new Material(Shader.Find("Particles/Additive"));
        }

        public void Init(Ship parentShip)
        {
            Ship = parentShip;
            On = false;
            transform.position = Ship.transform.position;
            transform.parent = Ship.transform;
        }

        private void Update()
        {
            LineRenderer.enabled = false;
            if (TurnManager.Instance.GamePaused && BattleScape.Instance.Ships[TurnManager.Instance.CurrentPlayer].Contains(Ship))//shows laser if game is paused, and if this is the current player's turn
            {
                var range = RayMaxDistance;
                RayMaxDistance = float.MaxValue;
                LineRenderer.SetColors(Color.green, Color.blue);
                //LineRenderer.enabled = true;
                LineRenderer.SetWidth(0.1f, 0.1f);
                PaintTarget(Ship.transform.position);
                RayMaxDistance = range;
            }
            else
            {
                LaserShots();
                LineRenderer.SetColors(Color.red, Color.red);
            }
        }

        [SerializeField]
        private float _timeToCooldown = 0f;
        protected float TimeToCooldown
        {
            get { return _timeToCooldown; }
            set { _timeToCooldown = Mathf.Clamp(value, 0f, AttackDuration); }
        }

        private void LaserShots()
        {
            if (TimeToCooldown >= AttackDuration) //start cooldown
            {
                _cooldownDuration = Cooldown;
                TimeToCooldown = 0;
            }
            TimeToCooldown -= Time.deltaTime; //so that if the laser is not attacking, it will start cooling
                                              //used for code cleanup. always reducing time, and when enabled compensate for it
            if (_cooldownDuration > 0)
            {
                _cooldownDuration -= Time.deltaTime;
                TimeToCooldown += Time.deltaTime; //compensating for unneeded reduction
            }
            else
            {
                PaintTarget(Ship.transform.position);
            }

        }

        public void PaintTarget(Vector3 origin)
        {
            if (Target == null || On == false || Ship == null)//no target
            {
                On = false;
            }
            else
            {
                float distanceSqr = (Target.transform.position - origin).sqrMagnitude;
                if (distanceSqr <= RayMaxDistance * RayMaxDistance) //pravents checking if out of range (was bugged if plant in the way)
                {
                    RaycastHit hit;
                    var direction = (Target.transform.position - origin).normalized;
                    var ray = new Ray(origin, direction);
                    if (Physics.Raycast(ray, out hit, RayMaxDistance))
                    {
                        Debug.DrawLine(origin, hit.point, Color.green);

                        var spaceObject = hit.transform.GetComponent<SpaceObject>();
                        if (spaceObject != null)
                        {
                            LineRenderer.SetPosition(0, origin);
                            LineRenderer.SetPosition(1, hit.point);
                            LineRenderer.enabled = true;
                            LineRenderer.SetWidth(_baseWidth * ((AttackDuration - TimeToCooldown) / AttackDuration), _baseWidth * ((AttackDuration - TimeToCooldown) / AttackDuration));
                            if (explosionTimer <= 0 && !TurnManager.Instance.GamePaused)
                            {
                                DamageEffectsFactory.Instance.StartDamageEffect(hit.point, Quaternion.identity,this); //TODO: create a specialized effect for the laser
                                explosionTimer = explosionDelay;
                            }
                            else
                            {
                                explosionTimer -= Time.deltaTime;
                            }
                            spaceObject.ChangeHealth(-Time.deltaTime * LaserDPS);
                            TimeToCooldown += Time.deltaTime * 2;
                        }

                    }

                }
            }
        }
    }
}
