using System;
using _Core.Managers;
using Monster_System;
using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MythicaButton : MonoBehaviour
{
    [Foldout("For Button", true)]
    [InitializationField] [SerializeField] private TextMeshProUGUI _mythicaName;
    [InitializationField] [SerializeField] private Image _mythicaLogo;
    [InitializationField] [SerializeField] private Button _buttonComponent;
    [ReadOnly] [SerializeField] private Monster _monster;

    [Foldout("For Other Info", true)]
    [InitializationField] [SerializeField] private TextMeshProUGUI _description;
    [InitializationField] [SerializeField] private Image _logoImage;
    [InitializationField] [SerializeField] private GameObject[] _patterns;
    
    private readonly Color _green = new Color32(179, 244, 122, 255);
    private readonly Color _grey = new Color32(128, 128, 128, 255);
    private readonly Color _alphaZero = new Color(255, 255, 255, 0);
    private readonly Color _white = Color.white;


    public void InitializeMonsterButton(Monster monster)
    {
        _monster = monster;
        _mythicaName.text = monster.monsterName;
        _mythicaLogo.color = _green;
        _buttonComponent.onClick.RemoveAllListeners();
        _buttonComponent.onClick.AddListener(ChangeOtherInfo);
    }

    public void ChangeToBlank()
    {
        _mythicaName.text = "??????";
        _buttonComponent.onClick.RemoveAllListeners();
        _buttonComponent.onClick.AddListener(ChangeInfoToBlank);
        _mythicaLogo.color = _grey;
    }

    public void ChangeInfoToBlank()
    {
        foreach (var pattern in _patterns)
        {
            pattern.SetActive(false);
        }

        var text = "<b><size=200%><s>Mythica Undiscovered</s></size></b>\n" +
                   "<size=90%>Type: <b>None</b>\n" +
                   "Basic Attack: <b>None</b></size>\n" +
                   "You have to discover the mythica in this slot.\n\n" +
                   "<size=120%><b>Base Stats</b></size>\n" +
                   "<size=90%>HP: <b>None</b>\n" +
                   "Max Lives: <b>None</b>\n" +
                   "Physical Attack: <b>None</b>\n" +
                   "Physical Defense: <b>None</b>\n" +
                   "Special Attack: <b>None</b>\n" +
                   "Special Defense: <b>None</b>\n" +
                   "Experience Yield: <b>None</b>\n" +
                   "Tame Resistance: <b>None</b>\n" +
                   "Critical Chance: <b>None</b>\n";

        _description.text = text;
        _logoImage.color = _alphaZero;
    }

    private void ChangeOtherInfo()
    {
        foreach (var pattern in _patterns)
        {
            pattern.SetActive(true);
        }

        var ui = GameManager.instance.uiManager;
        var text = "<b><size=200%>" + _monster.monsterName + "</size></b>\n" +
                   "<size=90%>Type: <b>" + _monster.type + "</b>\n" +
                   GetTypeAdvantage(_monster.type) +
                   "Basic Attack: <b>" + _monster.basicAttackType + "</b></size>\n" +
                   _monster.description + "\n\n" +
                   "<size=120%><b>Base Stats</b></size>\n" +
                   "<size=90%>HP: <b>" + _monster.stats.baseHealth + "</b>\n" +
                   "Max Lives: <b>" + _monster.stats.maxLives + "</b>\n" +
                   "Physical Attack: <b>" + _monster.stats.physicalAttack + "</b>\n" +
                   "Physical Defense: <b>" + _monster.stats.physicalDefense + "</b>\n" +
                   "Special Attack: <b>" + _monster.stats.specialAttack + "</b>\n" +
                   "Special Defense: <b>" + _monster.stats.specialDefense + "</b>\n" +
                   "Experience Yield: <b>" + _monster.stats.baseExpYield + "</b>\n" +
                   "Tame Resistance: <b>" + _monster.stats.tameResistance + "</b>\n" +
                   "Critical Chance: <b>" + (_monster.stats.criticalChance * 100).ToString("#.00") + "%</b>\n";
        _description.text = text;
        _logoImage.color = _white;

        _logoImage.sprite = _monster.type switch
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
}
