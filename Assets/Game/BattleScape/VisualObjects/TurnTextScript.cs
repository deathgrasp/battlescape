using UnityEngine;
using UnityEngine.UI;
namespace Assets.Game.BattleScape.VisualObjects
{
    class TurnTextScript :MonoBehaviour
    {
        public Text StatusText;
        public Text TimeLeftText;
        public Button UnPause=null;
        public void Update()
        {
            if (TurnManager.Instance.GamePaused)
            {
                StatusText.text = "Paused \n Turn "+(TurnManager.Instance.TurnNumber+1);
                UnPause.gameObject.SetActive(true);
            }
            else
            {
                UnPause.gameObject.SetActive(false);
                StatusText.text = Mathf.Clamp(TurnManager.Instance.TimeLeftForTurn, 0f, ConfigurationManager.Instance.TurnTime).ToString();
            }
            TimeLeftText.text = "Turns left: " + (ConfigurationManager.Instance.TotalGameTurns - TurnManager.Instance.TurnNumber);
        }
    }
}
