using System.Collections.Generic;
using Assets.Scripts._Core;
using Assets.Scripts.Items_and_Barter_System.Scripts;
using Assets.Scripts.Skill_System;
using UnityEngine;

namespace Assets.Scripts.Monster_System
{
    [CreateAssetMenu(menuName = "Monster System/New Monster")]
    public class Monster : ScriptableObjectWithID
    {
        public string monsterName;
        public GameObject monsterPrefab;
        public MonsterType type;
        public BasicAttackType basicAttackType;
        public float basicAttackRadius;
        public MonsterStats stats;
        public List<MonsterSkillLearnSets> skillLearnSets;
        [TextArea(15,20)]
        public string description;
    }
    
    [System.Serializable]
    public class MonsterSlot
    {
        //this will be the one saved
        public bool inParty;
        public int slotNumber;
        public string nickName;
        public Monster monster;
        public int currentHealth;
        public int currentStamina;
        public int currentExp;
        public float stabilityValue;
        public Skill[] skill = new Skill[4];
        public InventorySlot[] inventory = new InventorySlot[6];
    }

    [System.Serializable]
    public class MonsterStats
    {
        [Range(0,255)]
        public int baseHealth;
        [Range(0,10)]
        public int maxLives;
        [Range(0,255)]
        public int physicalAttack;
        [Range(0,255)]
        public int physicalDefense;
        [Range(0,255)]
        public int specialAttack;
        [Range(0,255)]
        public int specialDefense;
        [Range(20,255)]
        public int baseExpYield = 20;
        [Range(0,255)]
        public int tameResistance;
        [Range(0,1)]
        public float stamina;
        [Range(.05f, 1)] 
        public float criticalChance = .05f;
        [Tooltip("The lower the movement speed, the faster the monster attacks.")]
        [Range(.5f, 1.5f)] 
        public float movementSpeed = .5f;
        [Tooltip("The lower the attack rate, the faster the monster attacks.")]
        [Range(.5f, 1.5f)]
        public float attackRate = 1f;
    }

    [System.Serializable]
    public class MonsterSkillLearnSets
    {
        public Skill skill;
        public int levelToLearn;
    }

    public enum MonsterType
    {
        Piercer,
        Brawler,
        Slasher,
        Charger,
        Emitter,
        Keeper
    }

    public enum BasicAttackType
    {
        Melee,
        Range
    }
}