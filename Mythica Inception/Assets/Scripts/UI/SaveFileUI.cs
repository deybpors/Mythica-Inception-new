using System;
using _Core.Managers;
using TMPro;
using ToolBox.Serialization;
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
        public GameObject trashButton;
        public Sprite trashIcon;
        public PlayerSaveData playerSaveData;

        private Color _white;
        void OnEnable()
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(ContinueOrNew);
            if (playerSaveData == null)
            {
                trashButton.SetActive(false);
            }
        }

        private void ContinueOrNew()
        {
            var newGamePanel = GameManager.instance.uiManager.newGamePanel;

            if (!newGamePanel.continueSelected)
            {
                if (playerSaveData != null)
                {
                    GameManager.instance.audioManager.PlaySFX("Error");
                    return;
                } 

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
            var timeSpent = string.Format("{00:%h} : {00:%m} : {00:%s}", saveData.timeSpent);
            var save = saveData.lastOpened.ToShortDateString() + "\n" + timeSpent;
            saveFileInfo.text = save;

            playerSaveData = saveData;
        }

        public void ConfirmDelete()
        {
            var message = "Are you sure you want to delete Player " + playerSaveData.name + " in your files?";
            GameManager.instance.uiManager.modal.OpenModal(message, trashIcon, _white, DeleteSaveFile);
        }

        private void DeleteSaveFile()
        {
            DataSerializer.DeleteProfileIndex(buttonNum, GameManager.instance.saveManager.playerSaveKey);
            GameManager.instance.uiManager.startSceneUI.FillUpSaveFiles();
            GameManager.instance.uiManager.modal.CloseModal();
            GameManager.instance.uiManager.startSceneUI.continueButton.gameObject.SetActive(false);
            GameManager.instance.uiManager.newGamePanel.savePanelTweener.Disable();
            GameManager.instance.uiManager.newGamePanel.continueSelected = false;
        }
    }
}