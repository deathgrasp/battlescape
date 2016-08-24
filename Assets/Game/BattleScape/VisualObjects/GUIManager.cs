using UnityEngine;
using Assets.Utils;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.Game.BattleScape.SpaceObjects;
using Assets.Game.BattleScape.VisualObjects.Path;

namespace Assets.Game.BattleScape.VisualObjects
{
    /// <summary>
    /// In charge of the various GUI elements.</summary>
    class GUIManager : UnitySingleton<GUIManager>
    {

        public void Update()
        {
            if (!BattleScape.Instance.Generated)
                return;

            BattleScape.Instance.PlayerHealthBar.value = BattleScape.Instance.Ship.Health;
        }
        public void EndScreen()
        {
            var groupController = BattleScape.Instance.EndGamePanel.GetComponent<CanvasGroup>();
            groupController.interactable = true;
            groupController.GetComponentInParent<GraphicRaycaster>().enabled = true;
            groupController.alpha = 1;
        }
        public void FadeEndScreen()
        {
            var groupController = BattleScape.Instance.EndGamePanel.GetComponent<CanvasGroup>();
            groupController.interactable = false;
            groupController.alpha = 0;
            groupController.GetComponentInParent<GraphicRaycaster>().enabled = false;
        }

        public void UpdateHealthBar(float health)
        {
            BattleScape.Instance.PlayerHealthBar.value = health;
        }


    }
}
