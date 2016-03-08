using UnityEngine;
using System.Collections;
using UnityEngine.UI;
namespace Assets.Game.BattleScape.VisualObjects
{
    public class HPBar : MonoBehaviour
    {

        public Slider slider;
        public void UpdateText(Text text)
        {
            text.text = string.Format("{0}/{1}", slider.value, BattleScape.Instance.PlayerMaxHealth);
        }
    }
}