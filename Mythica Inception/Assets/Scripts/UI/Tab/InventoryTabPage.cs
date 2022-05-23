using System.Collections.Generic;
using _Core.Managers;
using Assets.Scripts.UI;
using Items_and_Barter_System.Scripts;
using Monster_System;
using MyBox;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class InventoryTabPage : TabPage
{
    [Foldout("For Mythica Items", true)]
    [InitializationField] [SerializeField] private Image[] _mythicaItemImages;
    [InitializationField] [SerializeField] private TextMeshProUGUI[] _mythicaItemAmounts;
    [InitializationField] [SerializeField] private TooltipTrigger[] _mythicaItemTooltipTriggers;
    [Foldout("For Inventory Items", true)]
    [InitializationField] [SerializeField] private Image[] _itemImages;
    [InitializationField] [SerializeField] private TextMeshProUGUI[] _itemAmounts;
    [InitializationField] [SerializeField] private TooltipTrigger[] _itemTooltipTriggers;

    [Foldout("For Mythica Button", true)]
    [InitializationField] [SerializeField] private Button[] _monsterButtons;

    [Foldout("For Other Info", true)]
    [InitializationField][SerializeField] private TextMeshProUGUI _description;
    [InitializationField][SerializeField] private Image _logoImage;
    [InitializationField][SerializeField] private GameObject[] _patterns;

    private PlayerInventory _inventory;
    private Color _white = Color.white;
    private UIManager _ui;
    private Dictionary<Button, Image> _monsterImages = new Dictionary<Button, Image>();
    private List<MonsterSlot> _playerMonsters = new List<MonsterSlot>();
    private MonsterSlot _currentMonsterSlotSelected;


    protected override void OnActive()
    {
        Initialize();
        if (GameManager.instance.player == null) return;
        _playerMonsters = GameManager.instance.player.monsterSlots;
        ChangeMonsterUI();
        var monsterCount = _playerMonsters.Count;

        for (var i = 0; i < monsterCount; i++)
        {
            if (_playerMonsters[i].monster == null) continue;

            _monsterButtons[i].onClick.Invoke();
            break;
        }

        UpdatePlayerInventory();
    }

    private void UpdatePlayerInventory()
    {
        if (_inventory == null)
        {
            _inventory = GameManager.instance.player.playerInventory;
        }

        var itemCount = _inventory.inventorySlots.Count;

        for (var i = 0; i < itemCount; i++)
        {
            var tooltip = _itemTooltipTriggers[i];
            if (tooltip == null) continue;

            tooltip.SetTitleContent(string.Empty, string.Empty);

            var item = _inventory.inventorySlots[i].inventoryItem;
            if (item == null)
            {
                _itemAmounts[i].text = string.Empty;
                _itemImages[i].sprite = _ui.blankSlotSquare;
                continue;
            }

            var title = "<b>" + item.itemName + "</b>";
            var content = item.itemDescription;
            tooltip.enabled = true;
            tooltip.SetTitleContent(title, content);
            var amount = _inventory.inventorySlots[i].amountOfItems;
            _itemAmounts[i].text = amount <= 0 ? string.Empty : amount.ToString();
            _itemImages[i].sprite = item.itemIcon;
        }
    }

    private void ChangeMonsterUI()
    {
        var monsterCount = _playerMonsters.Count;

        for (var i = 0; i < monsterCount; i++)
        {
            _monsterImages.TryGetValue(_monsterButtons[i], out var image);

            if (image == null) continue;
            if (_playerMonsters[i].monster == null)
            {
                _monsterButtons[i].interactable = false;
                image.sprite = _ui.blankSlotSquare;
                continue;
            }

            _monsterButtons[i].interactable = true;
            _monsterButtons[i].onClick.RemoveAllListeners();
            var slotNum = i;
            _monsterButtons[i].onClick.AddListener(() => UpdatePartyPanel(slotNum));
            image.sprite = _playerMonsters[i].monster.monsterPortrait;
        }
    }

    private void UpdatePartyPanel(int slotNum)
    {
        _currentMonsterSlotSelected = _playerMonsters[slotNum];
        UpdateOtherInfo();
        UpdateMonsterItems();
    }

    private void Initialize()
    {
        _ui = GameManager.instance.uiManager;
        foreach (var button in _monsterButtons)
        {
            try
            {
                _monsterImages.Add(button, button.GetComponent<Image>());
            }
            catch
            {
                //ignored
            }
        }
    }

    private void UpdateOtherInfo()
    {
        var monster = _currentMonsterSlotSelected.monster;

        if (monster == null) return;

        foreach (var pattern in _patterns)
        {
            pattern.SetActive(true);
        }

        var ui = GameManager.instance.uiManager;
        var text = "<b><size=200%>" + monster.monsterName + "</size></b>\n" +
                   "<size=90%>Type: <b>" + monster.type + "</b>\n" +
                   GetTypeAdvantage(monster.type) +
                   "Basic Attack: <b>" + monster.basicAttackType + "</b>\n" +
                   "Stability Value: <b>" + _currentMonsterSlotSelected.stabilityValue.ToString("#.00") + "</b></size>\n" +
                   monster.description + "\n\n" +
                   "<size=120%><b>Base Stats</b></size>\n" +
                   "<size=90%>HP: <b>" + monster.stats.baseHealth + "</b>\n" +
                   "Max Lives: <b>" + monster.stats.maxLives + "</b>\n" +
                   "Physical Attack: <b>" + monster.stats.physicalAttack + "</b>\n" +
                   "Physical Defense: <b>" + monster.stats.physicalDefense + "</b>\n" +
                   "Special Attack: <b>" + monster.stats.specialAttack + "</b>\n" +
                   "Special Defense: <b>" + monster.stats.specialDefense + "</b>\n" +
                   "Experience Yield: <b>" + monster.stats.baseExpYield + "</b>\n" +
                   "Tame Resistance: <b>" + monster.stats.tameResistance + "</b>\n" +
                   "Critical Chance: <b>" + (monster.stats.criticalChance * 100).ToString("#.00") + "%</b>\n";
        _description.text = text;
        _logoImage.color = _white;

        _logoImage.sprite = monster.type switch
        {
            MonsterType.Piercer => ui.piercer,
            MonsterType.Brawler => ui.brawler,
            MonsterType.Slasher => ui.slasher,
            MonsterType.Charger => ui.charger,
            MonsterType.Emitter => ui.emitter,
            MonsterType.Keeper => ui.keeper,
            _ => _logoImage.sprite
        };
    }

    private string GetTypeAdvantage(MonsterType type)
    {
        var text = string.Empty;

        switch (type)
        {
            case MonsterType.Piercer:
                text = "Strong to: <color=#ffc880>Chargers</color>\n" +
                       "Resistant to: <color=#ffc880>Chargers</color>\n" +
                       "Weak to:  <color=#b3f47a>Slashers</color>, <color=#97e4ff>Emitters</color>\n\n";
                break;
            case MonsterType.Brawler:
                text = "Strong to: None\n" +
                       "Resistant to: <color=#ffef7d>Piercers</color>, <color=#d8a1ff>Brawlers</color>, <color=#97e4ff>Emitters</color>\n" +
                       "Weak to:  <color=#ffc880>Chargers</color>\n\n";
                break;
            case MonsterType.Slasher:
                text = "Strong to: <color=#ffef7d>Piercers</color>, <color=#b3f47a>Slashers</color>, <color=#97e4ff>Emitters</color>\n" +
                       "Resistant to: None\n" +
                       "Weak to:  <color=#ffef7d>Piercers</color>, <color=#97e4ff>Emitters</color>\n\n";
                break;
            case MonsterType.Charger:
                text = "Strong to: <color=#d8a1ff>Brawlers</color>, <color=#b3f47a>Slashers</color>, <color=#97e4ff>Emitters</color>\n" +
                       "Resistant to: None\n" +
                       "Weak to:  <color=#b3f47a>Slashers</color>, <color=#ffc880>Chargers</color>, <color=#97e4ff>Emitters</color>\n\n";
                break;
            case MonsterType.Emitter:
                text = "Strong to: <color=#ffef7d>Piercers</color>, <color=#b3f47a>Slashers</color>, <color=#ffc880>Chargers</color>, <color=#97e4ff>Emitters</color>\n" +
                       "Resistant to: <color=#ffc880>Chargers</color>\n" +
                       "Weak to:  <color=#b3f47a>Slashers</color>, <color=#ffc880>Chargers</color>, <color=#97e4ff>Emitters</color>\n\n";
                break;
            case MonsterType.Keeper:
                text = "Strong to: <color=#f48989>Keepers</color>\n" +
                       "Resistant to: <color=#ffef7d>Piercers</color>, <color=#b3f47a>Slashers</color>, <color=#ffc880>Chargers</color>, <color=#d8a1ff>Brawlers</color>, <color=#97e4ff>Emitters</color>\n" +
                       "Weak to:  <color=#f48989>Keepers</color>\n\n";
                break;
        }

        return text;
    }

    private void UpdateMonsterItems()
    {
        var mythicaItemLength = _mythicaItemImages.Length;

        for (var i = 0; i < mythicaItemLength; i++)
        {
            var tooltip = _mythicaItemTooltipTriggers[i];
            if (tooltip == null) continue;

            tooltip.SetTitleContent(string.Empty, string.Empty);

            ItemObject item = null;
            try
            {
                item = _currentMonsterSlotSelected.inventory[i].inventoryItem;
                if (item == null)
                {
                    _mythicaItemAmounts[i].text = string.Empty;
                    _mythicaItemImages[i].sprite = _ui.blankSlotSquare;
                    continue;
                }
            }
            catch
            {
                _mythicaItemAmounts[i].text = string.Empty;
                _mythicaItemImages[i].sprite = _ui.blankSlotSquare;
                continue;
            }

            var title = "<b>" + item.itemName + "</b>";
            var content = item.itemDescription;
            tooltip.enabled = true;
            tooltip.SetTitleContent(title, content);
            var amount = _currentMonsterSlotSelected.inventory[i].amountOfItems;
            _mythicaItemAmounts[i].text = amount <= 0 ? string.Empty : amount.ToString();
            _mythicaItemImages[i].sprite = item.itemIcon;
        }
    }
}
