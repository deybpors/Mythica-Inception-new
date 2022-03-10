using System;
using System.Collections.Generic;
using _Core.Managers;
using _Core.Others;
using Items_and_Barter_System.Scripts;
using Monster_System;
using UnityEngine;

[System.Serializable]
public class PlayerSaveData
{
    [SerializeField] private string _playerName;
    [SerializeField] private Sex _sex;
    [SerializeField] private List<MonsterSlot> _monsterSlots;
    [SerializeField] private EntityHealth _playerHealth;
    [SerializeField] private List<InventorySlot> _inventorySlots;
    [SerializeField] private string _currentScenePath;
    [SerializeField] private DateTime _lastOpened;
    [SerializeField] private DateTime _lastClosed;

    public string name => _playerName;
    public DateTime lastOpened => _lastOpened;
    public DateTime lastClosed => _lastClosed;
    public Sex sex => _sex;
    public WorldPlacementData playerWorldPlacement;
    public List<MonsterSlot> playerMonsters => _monsterSlots;
    public EntityHealth playerHealth => _playerHealth;
    public List<InventorySlot> inventorySlots => _inventorySlots;
    public string currentScenePath => _currentScenePath;

    public PlayerSaveData(string playerName, Sex sex, WorldPlacementData playerWorldPlacement, List<MonsterSlot> monsterSlots, EntityHealth playerHealth, List<InventorySlot> inventorySlots, string currentScenePath, DateTime lastOpened, DateTime lastClosed)
    {
        _playerName = playerName;
        _sex = sex; 
        this.playerWorldPlacement = playerWorldPlacement;
        _monsterSlots = monsterSlots;
        _playerHealth = playerHealth;
        _inventorySlots = inventorySlots;
        _currentScenePath = currentScenePath;
        _lastOpened = lastOpened;
        _lastClosed = lastClosed;
    }
}
