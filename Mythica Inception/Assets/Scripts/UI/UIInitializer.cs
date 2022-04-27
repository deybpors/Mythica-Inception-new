using System.Collections.Generic;
using _Core.Managers;
using Assets.Scripts.UI;
using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIInitializer : MonoBehaviour
    {
        [Foldout("Start Scene UI", true)]
        public GameObject startSceneUICanvas;
        public UITweener startButtonsTweener;

        [Foldout("Gameplay UI", true)]
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
        public ItemDropUI itemDropUi;

        [Foldout("Credits UI", true)]
        public Button optionsButton;
        public Button optionsMinimizeButton;
        public OptionsUI generalOptionsUI;

        [Foldout("Dialogue UI", true)]
        public DialogueUI dialogueUI;

        [Foldout("Quest UI", true)]
        public QuestUI questUI;

        [Foldout("Loading UI", true)]
        public LoadingScreenUI loadingScreen;

        [Foldout("New Game Panel UI")] public NewGamePanelUI newGamePanel;
        
        [Foldout("Merchant UI")] public MerchantUI merchantUi;


        [Foldout("Modal UI")] public ModalUI modal;

        [Foldout("Monster Tamed", true)] public MonsterTamedUI monsterTamed;

        [Foldout("Debug Console")] public DebugConsoleUI debugConsole;


        [Foldout("Tooltip UI")] public TooltipUI tooltip;

        [Foldout("Cursors and Indicators", true)]
        public Texture2D normalCursor;
        public GameObject areaIndicator;
        public Texture2D pointIndicator;

        private void Awake()
        {
            if(GameManager.instance == null) return;
            
            var ui = GameManager.instance.uiManager;
            GameManager.instance.inputHandler.SwitchActionMap("UI");
            ui.loadingScreen = loadingScreen;
            ui.InitGameplayUIRef(gameplayUICanvas, minimapCamera, characterName, currentGold,characterLevel,characterHealth, characterExp, partySlots, skills, items, optionsButton);
            ui.questUI = questUI;
            ui.InitStartSceneUIRef(startSceneUICanvas, startButtonsTweener);
            ui.InitCursors(normalCursor, areaIndicator, pointIndicator);
            ui.dialogueUI = dialogueUI;
            ui.newGamePanel = newGamePanel;
            ui.debugConsole = debugConsole;
            ui.modal = modal;
            ui.tooltip = tooltip;
            ui.optionsMinimizeButton = optionsMinimizeButton;
            ui.generalOptionsUi = generalOptionsUI;
            ui.monsterTamedUi = monsterTamed;
            ui.merchantUi = merchantUi;
            ui.itemDropUi = itemDropUi;
            Cursor.SetCursor(normalCursor, Vector2.zero, CursorMode.Auto);
        }
    }
}