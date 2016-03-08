
using UnityEngine;
namespace Assets.Game.BattleScape.SpaceObjects.Ships
{
    class ParticleSystemAutoDestroy : MonoBehaviour
    {

        private ParticleSystem ps;


        public void Start()
        {
            ps = GetComponent<ParticleSystem>();
        }

        public void Update()
        {
            if (ps)
            {
                if (!ps.IsAlive())
                {
                    Destroy(gameObject);
                }
            }
        }
    }

}
