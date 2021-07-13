using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Monster_System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts._Core
{
    public static class GameCalculations
    {
        public static int Damage(float gamePace, int attackerLvl, int attackerAttack, int targetDefense, float skillPower, int maxSkillPower, float modifier)
        {
            return (int)(((2 * attackerLvl * gamePace + 10) * skillPower / maxSkillPower *
                ((float) attackerAttack / targetDefense) + 2) * modifier);
        }

        public static float Modifier(bool stab, float attackerMonsterSV, float attackerMonsterType, bool critical)
        {
            float modifier = (stab ? 1.5f : 1) * (1 + (attackerMonsterSV / 100)) * attackerMonsterType * (critical ? 2 : 1) *
                             Random.Range(.75f, 1);
            return modifier;
        }
        
        public static int Stats(int baseValue, float stabilityValue, int monsterCurrentLevel)
        {
            var stat = (int) (0.01 * (2 * baseValue + (stabilityValue * 2) * monsterCurrentLevel) + monsterCurrentLevel + 10);
            return stat;
        }

        public static int MonstersAvgLevel(List<MonsterSlot> monsterSlots)
        {
            var levels = monsterSlots.Select(slot => Level(slot.monster != null ? slot.currentExp : 0)).ToList();
            return (int)levels.Average();
        }

        public static int MonstersAvgHealth(List<MonsterSlot> monsterSlots)
        {
            var health = monsterSlots.Select(slot => slot.monster != null ? slot.monster.stats.baseHealth : 0).ToList();
            return (int) health.Average();
        }

        public static float MonstersAvgStabilityValue(List<MonsterSlot> monsterSlots)
        {
            var stabilityValue = monsterSlots.Select(slot => slot.monster != null ? slot.stabilityValue : 0).ToList();
            return stabilityValue.Average();
        }
        
        public static int TameValue(int wildMonsterLvl, bool statusFX, int wildMonsterCurrentHP, int wildMonsterMaxHP)
        {
            var hp = (float) wildMonsterCurrentHP / wildMonsterMaxHP;
            //if (hp < .5f) { hp = .5f; }
            var sfx = statusFX ? .75f : 1;
            return (int)(50 + ((Mathf.Pow(wildMonsterLvl, 3)) / 5) * sfx * hp);
        }
        
        public static int TameBeam(int avgLevel, float tSPower, int wildMonsterTamingResistance , float gamePace)
        { 
            float random = Random.Range(0.85f, 1);
            int tameBeam = (int) ((Mathf.Pow(avgLevel, 2) * 2 + 10 * gamePace) * ((float) tSPower / wildMonsterTamingResistance) * random + 2);
            return tameBeam;
        }

        public static int Level(int exp)
        {
            return (int) Mathf.Pow(exp, 1f / 3f);
        }
        
        public static int LvlUpRequirement()
        {
            return 0;
        }
        
        public static int ExperienceGain()
        {
            return 0;
        }

        public static float TypeComparison(MonsterType attackerSkillType, MonsterType monsterHitType)
        {
            var offenseTypeNum = 0;
            var defenseTypeNum = 0;

            for (int i = 0; i < GameManager.instance.attackerTypes.Count; i++)
            {
                if (attackerSkillType.ToString().ToUpper().Equals(GameManager.instance.attackerTypes[i]))
                {
                    offenseTypeNum = i;
                    break;
                }
            }

            for (int i = 0; i < GameManager.instance.defenseTypes.Count; i++)
            {
                if (monsterHitType.ToString().ToUpper().Equals(GameManager.instance.defenseTypes[i]))
                {
                    defenseTypeNum = i;
                    break;
                }
            }
            var typeComparison = GameManager.instance.typeChart[offenseTypeNum][defenseTypeNum];
            return typeComparison;
        }
    }
}