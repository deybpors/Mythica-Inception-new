﻿using System.Collections.Generic;
using _Core.Managers;
using Assets.Scripts.UI;
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
        public ProgressBarUI playerHealth;
        public ProgressBarUI monsterExp;
        public List<PartySlotUI> partySlots;
        public List<Image> skills;
        public List<Image> items;

        [Header("Dialogue UI")]
        public GameObject dialogueUICanvas;
        public DialogueManager dialogueManager;
        
        [Header("Loading UI")] 
        public GameObject loadingScreen;
        public ProgressBarUI loadingBar;
        private void Start()
        {
            var ui = GameManager.instance.uiManager;
            ui.InitLoadingUIRef(loadingScreen,loadingBar);
            ui.InitGameplayUIRef(gameplayUICanvas, minimapCamera, playerHealth, monsterExp, partySlots, skills, items);
            ui.InitDialogueUIRef(dialogueUICanvas, dialogueManager);
            ui.startSceneUICanvas = startSceneUICanvas;

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