using UnityEngine;
namespace Assets.Game.BattleScape.VisualObjects
{
    public class ActionMenuScripts : MonoBehaviour
    {
        public void CreateWaypoint()
        {
            InputManager.Instance.DelayFrame=true;
            InputManager.Instance.LocationAction = true;
            ConfigurationManager.Instance.ActionMenu.gameObject.SetActive(false);
        }
        public void AttackTarget()
        {
            InputManager.Instance.SpaceObjectAction = true;
            ConfigurationManager.Instance.ActionMenu.gameObject.SetActive(false);
        }
        public void Update()
        {
            transform.position = transform.position+new Vector3(0,1-transform.position.y,0);
        }
    }
}
