using UnityEngine;
namespace Assets.Game.BattleScape.VisualObjects
{
    public class ActionMenuScripts : MonoBehaviour
    {
        public void CreateWaypoint()
        {
            InputManager.Instance.DelayFrame=true;
            InputManager.Instance.WaypointAction = true;
            InputManager.Instance.LocationAction = true;
            ConfigurationManager.Instance.ActionMenu.gameObject.SetActive(false);
        }
        public void AttackTarget()
        {
            InputManager.Instance.SpaceObjectAction = true;
            ConfigurationManager.Instance.ActionMenu.gameObject.SetActive(false);
        }

        public void ShootAtLocation()
        {
            Debug.Log("shoot at location clicked");
            InputManager.Instance.DelayFrame = true;

            InputManager.Instance.LocationAction = true;
            InputManager.Instance.GunShotAction = true;
            ConfigurationManager.Instance.ActionMenu.gameObject.SetActive(false);
        }
        public void Update()
        {
            transform.position = transform.position+new Vector3(0,1-transform.position.y,0);
        }
    }
}
