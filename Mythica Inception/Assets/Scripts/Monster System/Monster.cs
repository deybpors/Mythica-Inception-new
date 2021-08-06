using System.Collections.Generic;
using _Core.Others;
using Combat_System;
using Items_and_Barter_System.Scripts;
using Skill_System;
using UnityEngine;

namespace Monster_System
{
    [CreateAssetMenu(menuName = "Monster System/New Monster")]
    public class Monster : ScriptableObjectWithID
    {
        public string monsterName;
        public GameObject monsterPrefab;
        public Sprite monsterPortrait;
        public MonsterType type;
        public BasicAttackType basicAttackType;
        public Skill basicAttackSkill;
        public ProjectileGraphics basicAttackObjects;
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
        public bool fainted;
        public string nickName;
        public Monster monster;
        public int currentHealth;
        public int currentExp = 1;
        public int currentLives;
        [Range(1,50)]
        public float stabilityValue = 1;
        public SkillSlot[] skillSlots = new SkillSlot[4];
        public InventorySlot[] inventory = new InventorySlot[6];

        public MonsterSlot() { }
        
        public MonsterSlot(Monster mon, int xp, int sv)
        {
            inParty = false;
            slotNumber = 0;
            nickName = "";
            monster = mon;
            currentHealth = 0;
            currentExp = xp;
            currentLives = monster.stats.maxLives;
            stabilityValue = sv;
            skillSlots = new SkillSlot[4];
            inventory = new InventorySlot[6];
        }
    }

    [System.Serializable]
    public class MonsterStats
    {
        [Range(1,255)]
        public int baseHealth;
        [Range(1,10)]
        public int maxLives;
        [Range(1,255)]
        public int physicalAttack;
        [Range(1,255)]
        public int physicalDefense;
        [Range(1,255)]
        public int specialAttack;
        [Range(1,255)]
        public int specialDefense;
        [Range(20,255)]
        public int baseExpYield = 20;
        [Range(1,255)]
        public int tameResistance;
        [Range(.05f, 1)] 
        public float criticalChance = .05f;
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