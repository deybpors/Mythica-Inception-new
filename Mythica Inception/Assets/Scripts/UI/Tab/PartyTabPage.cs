using System;
using System.Collections.Generic;
using _Core.Managers;
using Assets.Scripts.UI;
using Items_and_Barter_System.Scripts;
using Monster_System;
using MyBox;
using Skill_System;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class PartyTabPage : TabPage
{
    [Foldout("For Main Party", true)]
    [InitializationField] [SerializeField] private Button[] _monsterButtons;
    [InitializationField] [SerializeField] private Image[] _skillImages;
    [InitializationField] [SerializeField] private Image[] _itemImages;
    [InitializationField] [SerializeField] private TextMeshProUGUI[] _itemAmount;
    [InitializationField] [SerializeField] private TooltipTrigger[] _skillTooltipTriggers;
    [InitializationField] [SerializeField] private TooltipTrigger[] _itemTooltipTriggers;

    [Foldout("For Other Info", true)]
    [InitializationField][SerializeField] private TextMeshProUGUI _description;
    [InitializationField][SerializeField] private Image _logoImage;
    [InitializationField][SerializeField] private GameObject[] _patterns;

    private Dictionary<Button, Image> _monsterImages = new Dictionary<Button, Image>();
    private List<MonsterSlot> _playerMonsters;
    private MonsterSlot _currentMonsterSlotSelected;
    private UIManager _ui;
    private readonly Color32 _white = Color.white;

    void Start()
    {
        Initialize();
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

    protected override void OnActive()
   {
       Initialize();
        if(GameManager.instance.player == null) return;

       _playerMonsters = GameManager.instance.player.monsterSlots;
       ChangeMonsterUI();
       var monsterCount = _playerMonsters.Count;

       for (var i = 0; i < monsterCount; i++)
       {
           if (_playerMonsters[i].monster == null) continue;
           
           _monsterButtons[i].onClick.Invoke();
           break;
       }
   }

   private void ChangeMonsterUI()
   {
       var monsterCount = _playerMonsters.Count;

       for (var i = 0; i < monsterCount; i++)
       {
           _monsterImages.TryGetValue(_monsterButtons[i], out var image);
           
           if(image == null) continue;
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
       UpdateSkills();
       UpdateItems();
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

    private void UpdateItems()
    {
       var itemsCount = _itemImages.Length;

       for (var i = 0; i < itemsCount; i++)
       {
           var tooltip = _itemTooltipTriggers[i];
           if (tooltip == null) continue;

           tooltip.SetTitleContent(string.Empty, string.Empty);

           ItemObject item = null;
           try
           {
               item = _currentMonsterSlotSelected.inventory[i].inventoryItem;
               if (item == null)
               {
                   _itemAmount[i].text = string.Empty;
                   _itemImages[i].sprite = _ui.blankSlotSquare;
                   continue;
               }
           }
           catch
           {
               _itemAmount[i].text = string.Empty;
               _itemImages[i].sprite = _ui.blankSlotSquare;
               continue;
           }
           
           var title = "<b>" + item.itemName + "</b>";
           var content = item.itemDescription;
           tooltip.enabled = true;
           tooltip.SetTitleContent(title, content);
           var amount = _currentMonsterSlotSelected.inventory[i].amountOfItems;
           _itemAmount[i].text = amount <= 0 ? string.Empty : amount.ToString();
            _itemImages[i].sprite = item.itemIcon;
       }
    }

   private void UpdateSkills()
   {
       var skillCount = _skillImages.Length;

       for (var i = 0; i < skillCount; i++)
       {
           var tooltip = _skillTooltipTriggers[i];
           if (tooltip == null) continue;

           tooltip.SetTitleContent(string.Empty, string.Empty);

           Skill skill;

           try
           {
               skill = _currentMonsterSlotSelected.skillSlots[i].skill;
               if (skill == null)
               {
                   _skillImages[i].sprite = _ui.blankSlotSquare;
                   continue;
               }
           }
           catch
           {
               _skillImages[i].sprite = _ui.blankSlotSquare;
               continue;
           }

           var color = GetTextColorForType(skill);
           var title = "<b><color=#" + color + ">" + skill.skillName + "</color></b>";
           var content = "<size=90%>Power: <b>" + skill.power + "</b></size>\n\n" + skill.description;
           tooltip.enabled = true;
           tooltip.SetTitleContent(title, content);
           _skillImages[i].sprite = skill.skillIcon;
       }
   }

   private string GetTextColorForType(Skill skill)
   {
       switch (skill.skillType)
       {
           case MonsterType.Piercer: return "ffef7d";
           case MonsterType.Brawler: return "d8a1ff";
           case MonsterType.Slasher: return "b3f47a";
           case MonsterType.Charger: return "ffc880";
           case MonsterType.Emitter: return "97e4ff";
           case MonsterType.Keeper: return "f48989";
       }

       return string.Empty;
   }
}
