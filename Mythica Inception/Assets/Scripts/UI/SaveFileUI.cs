using System;
using _Core.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SaveFileUI : MonoBehaviour
    {
        public Button button;
        public int buttonNum;
        public TextMeshProUGUI playerName;
        public TextMeshProUGUI saveFileInfo;
        public PlayerSaveData playerSaveData;

        void OnEnable()
        {
            button.onClick.AddListener(ContinueOrNew);
        }

        private void ContinueOrNew()
        {
            var newGamePanel = GameManager.instance.uiManager.newGamePanel;
            
            if (!newGamePanel.continueSelected)
            {
                newGamePanel.DisableTweener(GameManager.instance.uiManager.startButtonsTweener);
                newGamePanel.ShowNewSaveFilePanel();
                newGamePanel.saveFileSelected = this;
                return;
            }

            GameManager.instance.uiManager.startSceneUI.ContinueGame(buttonNum);
        }

        public void SetSaveFileData(PlayerSaveData saveData)
        {
            playerName.text = saveData.name;
            TimeSpan span = (saveData.lastOpened - saveData.lastClosed);
            var timeSpent = string.Format("{00:%h} : {00:%m} : {00:%s}", span);
            string save = saveData.lastOpened.ToShortDateString() + "\n" + timeSpent;
            saveFileInfo.text = save;
            playerSaveData = saveData;
        }
    }
}