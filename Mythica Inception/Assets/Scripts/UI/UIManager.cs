using System;
using System.Collections;
using System.Collections.Generic;
using _Core.Managers;
using _Core.Others;
using Dialogue_System;
using Items_and_Barter_System.Scripts;
using Monster_System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        
        [HideInInspector] public GameObject startSceneUICanvas;
        [HideInInspector] public StartSceneUI startSceneUI;
        [HideInInspector] public UITweener startButtonsTweener;
        [HideInInspector] public GameObject gameplayUICanvas;
        [HideInInspector] public GameObject minimapCamera;
        [HideInInspector] public TextMeshProUGUI currentCharacterName;
        [HideInInspector] public TextMeshProUGUI currentCharacterLevel;
        [HideInInspector] public TextMeshProUGUI currentGold;
        [HideInInspector] public ProgressBarUI currentCharacterHealth;
        [HideInInspector] public ProgressBarUI currentCharacterExp;
        [HideInInspector] public List<PartySlotUI> partySlots;
        [HideInInspector] public List<Image> currentMonsterSkillImages;
        [HideInInspector] public List<Image> currentMonsterItemImages;
        [HideInInspector] public GameObject dialogueUICanvas;
        [HideInInspector] public DialogueManager dialogueManager;
        [HideInInspector] public GameObject questUICanvas;
        [HideInInspector] public TextMeshProUGUI questTitle;
        [HideInInspector] public TextMeshProUGUI questDescription;
        [HideInInspector] public TextMeshProUGUI questDecline;
        [HideInInspector] public TextMeshProUGUI questAccept;
        [HideInInspector] public Transform questReward;
        [HideInInspector] public GameObject loadingScreen;
        [HideInInspector] public ProgressBarUI loadingBar;
        [HideInInspector] public Camera loadingScreenCamera;
        [HideInInspector] public Texture2D normalCursor;
        [HideInInspector] public GameObject areaIndicator;
        [HideInInspector] public Texture2D pointIndicator;
        [HideInInspector] public NewGamePanelUI newGamePanel;
        [HideInInspector] public ModalUI modal;

        [Header("Monster Party UI")]
        public Sprite blankSlotSquare;
        public Sprite blankSlotCircle;
        public Color unusedPartyMember;

        #region Initialization

        public void InitStartSceneUIRef(GameObject startScenePanel, UITweener startButtonsTweener)
        {
            startSceneUICanvas = startScenePanel;
            startSceneUI = startSceneUICanvas.GetComponent<StartSceneUI>();
            this.startButtonsTweener = startButtonsTweener;
        }

        public void InitLoadingUIRef(GameObject canvas, ProgressBarUI loadBar, Camera cam)
        {
            loadingScreen = canvas;
            loadingBar = loadBar;
            loadingScreenCamera = cam;
            GameManager.instance.gameSceneManager.loadingUITweener = loadingScreen.GetComponent<UITweener>();
        }

        public void InitCursors(Texture2D normal, GameObject area, Texture2D point)
        {
            normalCursor = normal;
            areaIndicator = area;
            pointIndicator = point;
        }

        public void InitGameplayUIRef(GameObject canvas, GameObject minimapCam, TextMeshProUGUI characterName, TextMeshProUGUI gold, TextMeshProUGUI characterLevel,ProgressBarUI characterHealth, ProgressBarUI characterExp, List<PartySlotUI> party, List<Image> skills, List<Image> items)
        {
            gameplayUICanvas = canvas;
            minimapCamera = minimapCam;
            currentGold = gold;
            currentCharacterName = characterName;
            currentCharacterLevel = characterLevel;
            currentCharacterHealth = characterHealth;
            currentCharacterExp = characterExp;
            currentMonsterSkillImages = skills;
            currentMonsterItemImages = items;
            partySlots = party;
        }

        public void InitDialogueUIRef(GameObject canvas, DialogueManager manager)
        {
            dialogueUICanvas = canvas;
            dialogueManager = manager;
        }

        public void InitQuestUIRef(GameObject questUICanvas, TextMeshProUGUI questTitle, TextMeshProUGUI questDescription, Transform questRewardParent, TextMeshProUGUI accept, TextMeshProUGUI decline)
        {
            this.questUICanvas = questUICanvas;
            this.questTitle = questTitle;
            this.questDescription = questDescription;
            questReward = questRewardParent;
            questAccept = accept;
            questDecline = decline;
        }

        public void InitGameplayUI(string charName, float currentHealth, float maxHealth, List<MonsterSlot> monsterSlots)
        {
            gameplayUICanvas.SetActive(false);
            currentCharacterName.text = charName;
            currentCharacterHealth.currentValue = currentHealth;
            currentCharacterHealth.maxValue = maxHealth;
            currentCharacterLevel.transform.parent.gameObject.SetActive(false);
            var monsterCount = monsterSlots.Count;
            for (var i = 0; i < monsterCount; i++)
            {
                if (monsterSlots[i].monster == null)
                {
                    partySlots[i].memberPortrait.sprite = blankSlotCircle;
                    partySlots[i].memberHealth.transform.parent.gameObject.SetActive(false);
                    continue;
                }

                var fill = (float) monsterSlots[i].currentHealth / GameSettings.Stats(
                    monsterSlots[i].monster.stats.baseHealth, monsterSlots[i].stabilityValue,
                    GameSettings.Level(monsterSlots[i].currentExp));
                partySlots[i].memberHealth.fillAmount = fill;
                partySlots[i].memberPortrait.sprite = monsterSlots[i].monster.monsterPortrait;
                partySlots[i].memberPortrait.color = unusedPartyMember;
            }
            
            gameplayUICanvas.SetActive(true);
        }

        #endregion


        public void UpdateCharSwitchUI(string charName, float currentHealth, float maxHealth, float currentExp, float maxExp, int currentSlotNumber, List<Sprite> skills, List<Sprite> items)
        {
            gameplayUICanvas.SetActive(false);
            currentCharacterName.text = charName;
            currentCharacterHealth.currentValue = currentHealth;
            currentCharacterHealth.maxValue = maxHealth;
            currentCharacterExp.currentValue = currentExp;
            currentCharacterExp.maxValue = maxExp;

            if (currentSlotNumber >= 0)
            {
                currentCharacterLevel.transform.parent.gameObject.SetActive(true);
                currentCharacterLevel.text = GameSettings.Level(GameManager.instance.player.monsterSlots[currentSlotNumber].currentExp).ToString();
            }
            else
            {
                currentCharacterLevel.transform.parent.gameObject.SetActive(false);
            }
            
            var partyCount = partySlots.Count;
            for (var i = 0; i < partyCount; i++)
            {
                if (partySlots[i].memberPortrait.sprite != blankSlotCircle)
                {
                    partySlots[i].memberPortrait.color = unusedPartyMember;
                }
                
                if (i == currentSlotNumber)
                {
                    partySlots[i].memberPortrait.color = Color.white;
                }
            }
            
            var skillCount = currentMonsterSkillImages.Count;
            for (var i = 0; i < skillCount; i++)
            {
                if (skills[i] == null)
                {
                    currentMonsterSkillImages[i].sprite = blankSlotSquare;
                    currentMonsterSkillImages[i].raycastTarget = false;
                    continue;
                }
                currentMonsterSkillImages[i].sprite = skills[i];
                currentMonsterSkillImages[i].raycastTarget = true;
            }
            
            var itemCount = currentMonsterItemImages.Count;
            for (var i = 0; i < itemCount; i++)
            {
                if (items[i] == null)
                {
                    currentMonsterItemImages[i].sprite = blankSlotSquare;
                    currentMonsterItemImages[i].raycastTarget = false;
                    continue;
                }
                currentMonsterItemImages[i].sprite = items[i];
                currentMonsterItemImages[i].raycastTarget = true;
            }
            
            gameplayUICanvas.SetActive(true);
        }

        public void UpdatePartyUI(MonsterSlot slot)
        {
            var num = slot.slotNumber;
            partySlots[slot.slotNumber].memberHealth.transform.parent.gameObject.SetActive(true);
            var fill = (float) slot.currentHealth / GameSettings.Stats(
                slot.monster.stats.baseHealth, slot.stabilityValue,
                GameSettings.Level(slot.currentExp));
            partySlots[num].memberHealth.fillAmount = fill;
            partySlots[num].memberPortrait.sprite = slot.monster.monsterPortrait;
            partySlots[num].memberPortrait.color = unusedPartyMember;
        }

        public void UpdateHealthUI(int currentSlotNumber, float currentHealth)
        {
            currentCharacterHealth.currentValue = currentHealth;
            
            if (currentSlotNumber < 0) return;
            
            var currentMonster = GameManager.instance.player.monsterSlots[currentSlotNumber];
            var maxHealth = GameSettings.Stats(
                currentMonster.monster.stats.baseHealth,
                currentMonster.stabilityValue,
                GameSettings.Level(currentMonster.currentExp));
            UpdatePartyMemberHealth(currentSlotNumber, currentHealth, maxHealth);
        }

        public void UpdatePartyMemberHealth(int slotNumber, float currentHealth, float maxHealth)
        {
            partySlots[slotNumber].memberHealth.fillAmount = currentHealth / maxHealth;
            
            if(currentHealth > 0) return;
            partySlots[slotNumber].memberPortrait.color = Color.red;
        }

        public void UpdateExpUI(int monsterSlotNum, float addedExp)
        {
            var currentExp = currentCharacterExp.currentValue + addedExp;
            currentCharacterExp.currentValue = currentExp;

            if (currentExp < currentCharacterExp.maxValue) return;

            var newCurrent = currentExp - currentCharacterExp.maxValue;
            LevelUp(out var maxExp, monsterSlotNum);

            currentCharacterExp.currentValue = newCurrent;
            currentCharacterExp.maxValue = maxExp;
            
        }

        private void LevelUp(out float maxExp, int slotNum)
        {
            var monsterSlots = GameManager.instance.player.monsterSlots;
            var monsterLevel = GameSettings.Level(monsterSlots[slotNum].currentExp);
            maxExp = (float) GameSettings.Experience( monsterLevel + 1) - GameSettings.Experience(monsterLevel);
            currentCharacterLevel.text = monsterLevel.ToString();
        }

        IEnumerator DelayAction(float delay, Action action)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }
        
        public void DeactivateAllUI()
        {
            startSceneUICanvas.SetActive(false);
            gameplayUICanvas.SetActive(false);
            minimapCamera.SetActive(false);
            dialogueUICanvas.SetActive(false);
            loadingScreen.SetActive(false);
            newGamePanel.gameObject.SetActive(false);
            modal.CloseModal();
        }

        public void UpdateGoldUI()
        {
            var inventory = GameManager.instance.player.inventory;
            var count = inventory.inventorySlots.Count;
            for (var i = 0; i < count; i++)
            {
                if (!(inventory.inventorySlots[i].inventoryItem is Gold)) continue;
                
                currentGold.text = inventory.inventorySlots[i].amountOfItems.ToString();
                break;
            }
        }
    }

    [System.Serializable]
    public class PartyUIData
    {
        public Sprite portrait;
        public float healthFill;
    }
}
