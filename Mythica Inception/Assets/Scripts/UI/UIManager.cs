using System.Collections.Generic;
using _Core.Managers;
using Dialogue_System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        
        [HideInInspector] public GameObject startSceneUICanvas;
        [HideInInspector] public GameObject gameplayUICanvas;
        [HideInInspector] public GameObject minimapCamera;
        [HideInInspector] public ProgressBarUI currentPlayerHealth;
        [HideInInspector] public ProgressBarUI currentMonsterExp;
        [HideInInspector] public List<PartySlotUI> partySlots;
        [HideInInspector] public List<Image> currentMonsterSkillImages;
        [HideInInspector] public List<Image> currentMonsterItemImages;
        [HideInInspector] public GameObject dialogueUICanvas;
        [HideInInspector] public DialogueManager dialogueManager;
        [HideInInspector] public GameObject loadingScreen;
        [HideInInspector] public ProgressBarUI loadingBar;
        [HideInInspector] public Camera loadingScreenCamera;
        
        public void InitLoadingUIRef(GameObject canvas, ProgressBarUI loadBar, Camera cam)
        {
            loadingScreen = canvas;
            loadingBar = loadBar;
            loadingScreenCamera = cam;
            GameManager.instance.gameSceneManager.loadingUITweener = loadingScreen.GetComponent<UITweener>();
        }

        public void InitGameplayUIRef(GameObject canvas, GameObject minimapCam, ProgressBarUI playerHealth, ProgressBarUI monsterExp, List<PartySlotUI> party, List<Image> skills, List<Image> items)
        {
            gameplayUICanvas = canvas;
            minimapCamera = minimapCam;
            currentPlayerHealth = playerHealth;
            currentMonsterExp = monsterExp;
            currentMonsterSkillImages = skills;
            currentMonsterItemImages = items;
            partySlots = party;
        }

        public void InitDialogueUIRef(GameObject canvas, DialogueManager manager)
        {
            dialogueUICanvas = canvas;
            dialogueManager = manager;
        }

        public void DeactivateAllUI()
        {
            startSceneUICanvas.SetActive(false);
            gameplayUICanvas.SetActive(false);
            minimapCamera.SetActive(false);
            dialogueUICanvas.SetActive(false);
            loadingScreen.SetActive(false);
        }
    }
}
