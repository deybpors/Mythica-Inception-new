using System;
using System.Collections;
using System.Collections.Generic;
using _Core.Managers;
using _Core.Others;
using Assets.Scripts.UI;
using Items_and_Barter_System.Scripts;
using Monster_System;
using MyBox;
using Skill_System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        #region Hidden Fields
            [HideInInspector] public GameObject startSceneUICanvas;
            [HideInInspector] public StartSceneUI startSceneUI;
            [HideInInspector] public UITweener startButtonsTweener;
            [HideInInspector] public GameObject gameplayUICanvas;
            [HideInInspector] public ItemDropUI itemDropUi;
            [HideInInspector] public Button optionsButton;
            [HideInInspector] public UITweener gameplayTweener;
            [HideInInspector] public GameObject minimapCamera;
            [HideInInspector] public TextMeshProUGUI currentCharacterName;
            [HideInInspector] public TextMeshProUGUI currentCharacterLevel;
            [HideInInspector] public TextMeshProUGUI currentGold;
            [HideInInspector] public ProgressBarUI currentCharacterHealth;
            [HideInInspector] public ProgressBarUI currentCharacterExp;
            [HideInInspector] public List<PartySlotUI> partySlots;
            [HideInInspector] public List<Image> currentMonsterSkillImages;
            [HideInInspector] public List<Image> currentMonsterItemImages;
            [HideInInspector] public List<TextMeshProUGUI> currentMonsterItemsAmount;
            [HideInInspector] public UITweener skillsTweener;
            [HideInInspector] public DialogueUI dialogueUI;
            [HideInInspector] public QuestUI questUI;
            [HideInInspector] public LoadingScreenUI loadingScreen;
            [HideInInspector] public Texture2D normalCursor;
            [HideInInspector] public GameObject areaIndicator;
            [HideInInspector] public Texture2D pointIndicator;
            [HideInInspector] public NewGamePanelUI newGamePanel;
            [HideInInspector] public MerchantUI merchantUi;
            [HideInInspector] public ModalUI modal;
            [HideInInspector] public DebugConsoleUI debugConsole;
            [HideInInspector] public TooltipUI tooltip;
            [HideInInspector] public Button optionsMinimizeButton;
            [HideInInspector] public OptionsUI generalOptionsUi;
            [HideInInspector] public MonsterTamedUI monsterTamedUi;
            private Dictionary<Image, Button> _itemButtons = new Dictionary<Image, Button>();
            private GameObject _skillsObject;
        #endregion


        [Foldout("Monster Party UI", true)]
        public Sprite blankSlotSquare;
        public Sprite blankSlotCircle;
        public Color unusedPartyMember;

        [Foldout("Mythica Type Icons", true)]
        public Sprite piercer;
        public Sprite brawler;
        public Sprite slasher;
        public Sprite charger;
        public Sprite emitter;
        public Sprite keeper;

        #region Initialization

        public void InitStartSceneUIRef(GameObject startScenePanel, UITweener startButtonsTweener)
        {
            startSceneUICanvas = startScenePanel;
            startSceneUI = startSceneUICanvas.GetComponent<StartSceneUI>();
            this.startButtonsTweener = startButtonsTweener;
        }

        public void InitCursors(Texture2D normal, GameObject area, Texture2D point)
        {
            normalCursor = normal;
            areaIndicator = area;
            pointIndicator = point;
        }

        public void InitGameplayUIRef(GameObject canvas, GameObject minimapCam, TextMeshProUGUI characterName, TextMeshProUGUI gold, TextMeshProUGUI characterLevel,ProgressBarUI characterHealth, ProgressBarUI characterExp, List<PartySlotUI> party, List<Image> skills, List<Image> items, List<TextMeshProUGUI> itemsAmount, Button optionsButton)
        {
            gameplayUICanvas = canvas;
            gameplayTweener = canvas.GetComponent<UITweener>();
            minimapCamera = minimapCam;
            currentGold = gold;
            currentCharacterName = characterName;
            currentCharacterLevel = characterLevel;
            currentCharacterHealth = characterHealth;
            currentCharacterExp = characterExp;
            currentMonsterSkillImages = skills;
            currentMonsterItemImages = items;
            currentMonsterItemsAmount = itemsAmount;
            partySlots = party;
            this.optionsButton = optionsButton;
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

        #region Update
        public void UpdateCharSwitchUI(string charName, float currentHealth, float maxHealth, float currentExp, float maxExp, int currentSlotNumber)
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

            gameplayUICanvas.SetActive(true);
        }

        public void UpdateItemsUI(MonsterManager manager, int slot, List<MonsterSlot> monsterSlots)
        {
            var player = GameManager.instance.player;

            var items = new List<Sprite>();
            var itemsAmount = new List<string>();
            var itemActions = new List<UnityAction>();

            if (slot >= 0)
            {
                var monsterSlot = monsterSlots[slot];
                var inventoryLength = monsterSlot.inventory.Length;

                for (var i = 0; i < inventoryLength; i++)
                {
                    if (monsterSlot.inventory[i] == null || monsterSlot.inventory[i].inventoryItem == null)
                    {
                        items.Add(null);
                        itemsAmount.Add(string.Empty);
                        itemActions.Add((() => { }));
                        continue;
                    }
                    items.Add(monsterSlot.inventory[i].inventoryItem.itemIcon);
                    itemsAmount.Add(monsterSlot.inventory[i].amountOfItems.ToString());
                    var index = i;
                    itemActions.Add(() => manager.UseUsableItems(monsterSlot.inventory[index], slot));
                }
            }
            else
            {
                for (var i = 0; i < 6; i++)
                {
                    if (player.playerInventory.inventorySlots[i].inventoryItem == null)
                    {
                        items.Add(null);
                        itemsAmount.Add(string.Empty);
                        itemActions.Add((() => { }));
                        continue;
                    }

                    items.Add(player.playerInventory.inventorySlots[i].inventoryItem.itemIcon);
                    itemsAmount.Add(player.playerInventory.inventorySlots[i].amountOfItems.ToString());
                    var index = i;
                    itemActions.Add(() => manager.UseUsableItems(player.playerInventory.inventorySlots[index], slot));
                }
            }

            var itemCount = items.Count;

            for (var i = 0; i < itemCount; i++)
            {
                currentMonsterItemsAmount[i].text = itemsAmount[i];
                if (items[i] == null)
                {
                    currentMonsterItemImages[i].sprite = blankSlotSquare;
                    currentMonsterItemImages[i].raycastTarget = false;
                    continue;
                }

                currentMonsterItemImages[i].sprite = items[i];
                currentMonsterItemImages[i].raycastTarget = true;

                if (!_itemButtons.TryGetValue(currentMonsterItemImages[i], out var btn))
                {
                    btn = currentMonsterItemImages[i].GetComponent<Button>();
                    _itemButtons.Add(currentMonsterItemImages[i], btn);
                }

                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(itemActions[i]);
            }
        }

        public void UpdateSkillUI(int slot, List<MonsterSlot> monsterSlots)
        {
            var skills = new List<Sprite>();

            if (slot >= 0)
            {
                var skillSlotsLength = monsterSlots[slot].skillSlots.Length;
                var noOfSkills = 0;
                for (var i = 0; i < skillSlotsLength; i++)
                {
                    if (monsterSlots[slot].skillSlots[i] == null || monsterSlots[slot].skillSlots[i].skill == null)
                    {
                        skills.Add(null);
                        continue;
                    }

                    skills.Add(monsterSlots[slot].skillSlots[i].skill.skillIcon);
                    noOfSkills++;
                }

                if (noOfSkills <= 0)
                {
                    if (!skillsTweener.disabled)
                    {
                        skillsTweener.Disable();
                    }
                    return;
                }

                if (_skillsObject == null)
                {
                    _skillsObject = skillsTweener.gameObject;
                }
                _skillsObject.SetActive(true);
            }
            else
            {
                if (!skillsTweener.disabled)
                {
                    skillsTweener.Disable();
                }
                return;
            }

            var skillImagesCount = currentMonsterSkillImages.Count;
            for (var i = 0; i < skillImagesCount; i++)
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
        }

        public void UpdatePartyUI(MonsterSlot slot)
        {
            var num = slot.slotNumber;
            partySlots[num].memberHealth.transform.parent.gameObject.SetActive(true);
            var fill = (float)slot.currentHealth / GameSettings.Stats(
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

            if (currentHealth > 0) return;
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
            maxExp = (float)GameSettings.Experience(monsterLevel + 1) - GameSettings.Experience(monsterLevel);
            currentCharacterLevel.text = monsterLevel.ToString();
        }

        public void DeactivateAllUI()
        {
            startSceneUICanvas.SetActive(false);
            gameplayUICanvas.SetActive(false);
            minimapCamera.SetActive(false);
            dialogueUI.gameObject.SetActive(false);

            if (generalOptionsUi.thisObject == null)
            {
                generalOptionsUi.thisObject = generalOptionsUi.gameObject;
            }
            generalOptionsUi.thisObject.SetActive(false);
            newGamePanel.gameObject.SetActive(false);
            modal.CloseModal();
        }

        public void UpdateGoldUI()
        {
            var inventory = GameManager.instance.player.playerInventory;
            var count = inventory.inventorySlots.Count;

            //find Gold in inventory
            for (var i = 0; i < count; i++)
            {
                if (!(inventory.inventorySlots[i].inventoryItem is Gold)) continue;

                currentGold.text = inventory.inventorySlots[i].amountOfItems.ToString();
                break;
            }
        }

        #endregion

        public IEnumerator Delay(float seconds, UnityAction action)
        {
            yield return new WaitForSecondsRealtime(seconds);
            action?.Invoke();
        }
    }

    [System.Serializable]
    public class PartyUIData
    {
        public Sprite portrait;
        public float healthFill;
    }
}
