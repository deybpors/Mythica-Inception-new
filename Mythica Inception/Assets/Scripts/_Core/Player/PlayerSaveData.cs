using System;
using System.Collections.Generic;
using _Core.Managers;
using _Core.Others;
using Assets.Scripts._Core.Player;
using Items_and_Barter_System.Scripts;
using Monster_System;
using Quest_System;
using UnityEngine;
public class PlayerSaveData
{
    [SerializeField] private string _playerName;
    [SerializeField] private Sex _sex;
    [SerializeField] private List<MonsterSlot> _monsterSlots;
    [SerializeField] private EntityHealth _playerHealth;
    [SerializeField] private List<InventorySlot> _inventorySlots;
    [SerializeField] private string _currentScenePath;
    [SerializeField] private Dictionary<string, PlayerAcceptedQuest> _activeQuests;
    [SerializeField] private Dictionary<string, PlayerAcceptedQuest> _finishedQuests;
    [SerializeField] private Dictionary<string, Monster> _discoveredMonsters;
    [SerializeField] private TimeSpan _timeSpent;
    [SerializeField] private DateTime _lastOpened;
    [SerializeField] private OptionsSaveData _optionsSaveData;

    public string name => _playerName;
    public Sex sex => _sex;
    public WorldPlacementData playerWorldPlacement;
    public List<MonsterSlot> playerMonsters => _monsterSlots;
    public EntityHealth playerHealth => _playerHealth;
    public List<InventorySlot> inventorySlots => _inventorySlots;
    public string currentScenePath => _currentScenePath;
    public Dictionary<string, PlayerAcceptedQuest> activeQuests => _activeQuests;
    public Dictionary<string, PlayerAcceptedQuest> finishedQuests => _finishedQuests;
    public TimeSpan timeSpent => _timeSpent;
    public DateTime lastOpened => _lastOpened;
    public Dictionary<string, Monster> discoveredMonsters => _discoveredMonsters;
    public OptionsSaveData optionsSaveData => _optionsSaveData;

    public PlayerSaveData(string playerName, Sex sex, WorldPlacementData playerWorldPlacement, List<MonsterSlot> monsterSlots, EntityHealth playerHealth, List<InventorySlot> inventorySlots, string currentScenePath, Dictionary<string, Monster> discoveredMonsters, Dictionary<string, PlayerAcceptedQuest> activeQuests, Dictionary<string, PlayerAcceptedQuest> finishedQuests, TimeSpan timeSpent, DateTime lastOpened, OptionsSaveData optionsSaveData)
    {
        _playerName = playerName;
        _sex = sex; 
        this.playerWorldPlacement = playerWorldPlacement;
        _monsterSlots = monsterSlots;
        _playerHealth = playerHealth;
        _inventorySlots = inventorySlots;
        _currentScenePath = currentScenePath;
        _activeQuests = activeQuests;
        _finishedQuests = finishedQuests;
        _timeSpent = timeSpent;
        _lastOpened = lastOpened;
        _discoveredMonsters = discoveredMonsters;
        _optionsSaveData = optionsSaveData;
    }
}
