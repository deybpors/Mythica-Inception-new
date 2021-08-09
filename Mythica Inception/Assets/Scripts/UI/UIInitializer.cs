using System.Collections.Generic;
using _Core.Managers;
using Dialogue_System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIInitializer : MonoBehaviour
    {
        [Header("Start Scene UI")]
        public GameObject startSceneUICanvas;
        
        [Header("Gameplay UI")] 
        public GameObject gameplayUICanvas;
        public GameObject minimapCamera;
        public TextMeshProUGUI characterName;
        public ProgressBarUI characterHealth;
        public ProgressBarUI characterExp;
        public TextMeshProUGUI characterLevel;
        public TextMeshProUGUI currentGold;
        public List<PartySlotUI> partySlots;
        public List<Image> skills;
        public List<Image> items;

        [Header("Dialogue UI")]
        public GameObject dialogueUICanvas;
        public DialogueManager dialogueManager;

        [Header("Quest UI")] 
        public GameObject questUICanvas;
        public TextMeshProUGUI questTitle;
        public TextMeshProUGUI questDescription;
        public Transform questRewardParent;
        public TextMeshProUGUI accept;
        public TextMeshProUGUI decline;
        
        [Header("Loading UI")] 
        public GameObject loadingScreen;
        public ProgressBarUI loadingBar;
        public Camera loadingScreenCam;
        
        [Header("Cursors and Indicators")]
        public Texture2D normalCursor;
        public GameObject areaIndicator;
        public Texture2D pointIndicator;
        
        private void Awake()
        {
            if(GameManager.instance == null) return;
            
            var ui = GameManager.instance.uiManager;
            ui.InitLoadingUIRef(loadingScreen,loadingBar, loadingScreenCam);
            ui.InitGameplayUIRef(gameplayUICanvas, minimapCamera, characterName, currentGold,characterLevel,characterHealth, characterExp, partySlots, skills, items);
            ui.InitDialogueUIRef(dialogueUICanvas, dialogueManager);
            ui.InitQuestUIRef(questUICanvas, questTitle, questDescription, questRewardParent, accept, decline);
            ui.startSceneUICanvas = startSceneUICanvas;
            ui.InitCursors(normalCursor, areaIndicator, pointIndicator);
            
            Cursor.SetCursor(normalCursor, Vector2.zero, CursorMode.Auto);
            
            if (!GameManager.instance.gameplayActive)
            {
                ui.startSceneUICanvas.SetActive(true);
                return;
            }
            
            ui.gameplayUICanvas.SetActive(true);
            ui.minimapCamera.SetActive(true);
        }
    }
}