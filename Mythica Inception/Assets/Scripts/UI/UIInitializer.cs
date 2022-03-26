using System.Collections.Generic;
using _Core.Managers;
using Assets.Scripts.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIInitializer : MonoBehaviour
    {
        [Header("Start Scene UI")]
        public GameObject startSceneUICanvas;
        public UITweener startButtonsTweener;

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
        public DialogueUI dialogueUI;

        [Header("Quest UI")] 
        public QuestUI questUI;

        [Header("Loading UI")]
        public LoadingScreenUI loadingScreen;
        
        [Header("Cursors and Indicators")]
        public Texture2D normalCursor;
        public GameObject areaIndicator;
        public Texture2D pointIndicator;

        [Header("New Game Panel")] public NewGamePanelUI newGamePanel;

        [Header("Modal")] public ModalUI modal;

        [Header("Debug Console")] public DebugConsoleUI debugConsole;

        [Header("Tooltip UI")] public TooltipUI tooltip;

        private void Awake()
        {
            if(GameManager.instance == null) return;
            
            var ui = GameManager.instance.uiManager;
            GameManager.instance.inputHandler.SwitchActionMap("UI");
            ui.loadingScreen = loadingScreen;
            ui.InitGameplayUIRef(gameplayUICanvas, minimapCamera, characterName, currentGold,characterLevel,characterHealth, characterExp, partySlots, skills, items);
            ui.questUI = questUI;
            ui.InitStartSceneUIRef(startSceneUICanvas, startButtonsTweener);
            ui.InitCursors(normalCursor, areaIndicator, pointIndicator);
            ui.dialogueUI = dialogueUI;
            ui.newGamePanel = newGamePanel;
            ui.debugConsole = debugConsole;
            ui.modal = modal;
            ui.tooltip = tooltip;
            
            Cursor.SetCursor(normalCursor, Vector2.zero, CursorMode.Auto);
        }
    }
}