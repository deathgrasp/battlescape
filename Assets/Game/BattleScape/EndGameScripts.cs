﻿using Assets.Game.BattleScape.VisualObjects;
using Assets.Game.BattleScape.VisualObjects.Path;
using UnityEngine;
namespace Assets.Game.BattleScape
{
    public class EndGameScripts : MonoBehaviour
    {
        public void QuitGame()
        {
            Debug.Log("Quitting");
            GUIManager.Instance.FadeEndScreen();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
		        Application.Quit();
#endif

        }

        public void RestartGame()
        {
            PathDisplay.Instance.ClearAllTrajectories();
            Debug.Log("Restarting");
            GUIManager.Instance.FadeEndScreen();
            Application.LoadLevel(Application.loadedLevel);

        }
    }
}
