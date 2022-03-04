using System.Collections.Generic;
using _Core.Managers;
using _Core.Others;
using Items_and_Barter_System.Scripts;
using Monster_System;
using ToolBox.Serialization;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public PlayerSaveData GetPlayerSaveData(string saveKey)
    {
        //if no saved data, then return null
        return DataSerializer.HasKey(saveKey) ? DataSerializer.Load<PlayerSaveData>(saveKey) : null;
    }

    // This method will be called before application quits
    private void SavePlayerData(string saveKey, string playerName, Sex playerSex,Vector3 position, List<MonsterSlot> monsterSlots, EntityHealth playerHealth, PlayerInventory inventory, string scenePath)
    {
        DataSerializer.Save(saveKey, new PlayerSaveData(playerName, playerSex, position, monsterSlots, playerHealth, inventory, scenePath));
    }
}

public class PlayerSaveData
{
    [SerializeField] private string _playerName;
    [SerializeField] private Sex _sex;
    [SerializeField] private Vector3 _position;
    [SerializeField] private List<MonsterSlot> _monsterSlots;
    [SerializeField] private EntityHealth _playerHealth;
    [SerializeField] private PlayerInventory _inventory;
    [SerializeField] private string _scenePath;

    public string name => _playerName;
    public Sex sex => _sex;
    public Vector3 position => _position;
    public List<MonsterSlot> playerMonsters => _monsterSlots;
    public EntityHealth playerHealth => _playerHealth;
    public PlayerInventory inventory => _inventory;
    public string scenePath => _scenePath;

    public PlayerSaveData(string playerName, Sex sex, Vector3 position, List<MonsterSlot> monsterSlots, EntityHealth playerHealth, PlayerInventory inventory, string scenePath)
    {
        this._playerName = playerName;
        this._sex = sex;
        this._position = position;
        this._monsterSlots = monsterSlots;
        this._playerHealth = playerHealth;
        this._inventory = inventory;
        this._scenePath = scenePath;
    }
}
