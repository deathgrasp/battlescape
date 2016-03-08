using UnityEngine;

namespace Assets.Game.BattleScape.SpaceObjects.Ships
{
    public class ShipVisual : MonoBehaviour
    {
        public ShipState ShipState { get; set; }
        public Ship Parent;
        private static ShipVisual _shipVisualPrefab;


        private static ShipVisual ShipVisualPrefab
        {
            get
            {
                return (_shipVisualPrefab ?? (_shipVisualPrefab = Resources.Load<ShipVisual>("BattleScape/UI/ShipVis")));
            }
        }
        public static ShipVisual Create()
        {
            var shipVisual = Instantiate(ShipVisualPrefab);
            shipVisual.ShipState = new ShipState();
            shipVisual.transform.position = Vector3.zero;
            shipVisual.transform.rotation = Quaternion.identity;
            return shipVisual;
        }
        public static ShipVisual Create(Vector3 position, Quaternion rotation)
        {
            var shipVisual = Instantiate(ShipVisualPrefab, position, rotation) as ShipVisual;
            shipVisual.ShipState = new ShipState();

            return shipVisual;
        }


    }
}
