using Assets.Utils;
using UnityEngine;

namespace Assets.Game.BattleScape.SpaceObjects.SolarObjects
{
	public class SolarSystem : MonoBehaviour
	{
        private Star _starPrefab;

        public Star StarPrefab
        {
            get
            {
                return _starPrefab ?? (_starPrefab = Resources.Load<Star>("BattleScape/Star"));
            }
        }

        private Planet _planetPrefab;

        public Planet PlanetPrefab
        {
            get { return _planetPrefab ?? (_planetPrefab = Resources.Load<Planet>("BattleScape/Planet")); }
        }


        public bool Generated;

		public Star Star;
		public Planet[] Planets= new Planet[0];
        public int LivingPlanets;
		internal void Generate()
		{
			if(Generated)
				return;

			Star = Instantiate(StarPrefab, Vector3.zero, Quaternion.identity) as Star;
			if (Star == null)
			{
				Debug.LogError("Could not instantiate a Star");
				return;
			}
			Star.transform.parent = transform;

            if (!ConfigurationManager.Instance.ClearScene) //no planets in the clear scene
            {
                Planets = new Planet[Random.Range(0, 10)];
                LivingPlanets = Planets.Length;
                var distanceFromStar = 0f;
                Range.ReverseFor(Planets.Length, i =>
                {

                    distanceFromStar += 3f;
                    var pos = Random.insideUnitCircle;
                    var position = new Vector3(pos.x, 0f, pos.y) * Random.Range(distanceFromStar, (distanceFromStar = distanceFromStar + 2f));
                    Planets[i] = Instantiate(PlanetPrefab, position, Quaternion.LookRotation(-position)) as Planet;
                    Planets[i].transform.parent = transform;
                    Planets[i].Init(Random.Range(1f, 60f));
                    AssignGravityWell(Planets[i]);
                });
            }

            AssignGravityWell(Star);
			Generated = true;
		}

		public static SolarSystem Create()
		{
			return GameObjectExtensions.CreateNew<SolarSystem>("Solar System");
		}

        public void AssignGravityWell(SolarObject solarObject)
        {
            float gravityStrength;
            float gravityRadius;
            if (solarObject is Star)
            {
                gravityRadius = ConfigurationManager.Instance.StarGravityRadius;
                gravityStrength = ConfigurationManager.Instance.StarGravityStrength;
            }
            else
            {
                gravityRadius = ConfigurationManager.Instance.PlanetGravityRadius;
                gravityStrength = ConfigurationManager.Instance.PlanetGravityStrength;
            }
            solarObject.GravityPullStrangth = gravityStrength;
            solarObject.GravityRadius = gravityRadius;
            solarObject.GravityWell.transform.localScale = new Vector3(gravityRadius, gravityRadius, gravityRadius);
        }
	}
}
