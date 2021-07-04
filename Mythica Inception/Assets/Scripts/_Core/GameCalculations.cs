using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Core
{
    public static class GameCalculations
    {
        public static int Damage(float gamePace, int attackerLvl, int attackerAttack, int targetDefense, int skillPower, int maxSkillPower, float modifier)
        {
            int damage = (int)(gamePace * attackerLvl * ((float) attackerAttack / targetDefense) *
                                ((float) skillPower / maxSkillPower) * modifier);
            return damage;
        }

        public static float Modifier(bool stab, float attackerMonsterSV, float attackerMonsterType, bool critical)
        {
            float modifier = (stab ? 1.5f : 1) * (1 + (attackerMonsterSV / 100)) * attackerMonsterType * (critical ? 2 : 1) *
                             Random.Range(.75f, 1);
            return modifier;
        }
        
        public static int Stats(int baseValue, float stabilityValue, int monsterCurrentLevel, int monsterMaxLevel)
        {
            int stat = (int) (((baseValue + stabilityValue) * monsterCurrentLevel) / monsterMaxLevel) +
                       monsterCurrentLevel;
            return stat;
        }

        public static int TameValue(int wildMonsterLvl, bool statusFX, int wildMonsterCurrentHP, int wildMonsterMaxHP)
        {
            float hp = (float) wildMonsterCurrentHP / wildMonsterMaxHP;
            if (hp < .5f) { hp = .5f; }

            float sfx = statusFX ? .75f : 1;

            int tameValue = (int)(200 + ((wildMonsterLvl ^ 3) / 5) * sfx * hp);
            
            return tameValue;
        }
        
        public static int TameBeam(int avgLevel, int tSPower, int wildMonsterTamingResistance , float gamePace)
        {
            float random = Random.Range(0.85f, 1);
            int tameBeam = (int) ((avgLevel ^ 2) * ((float) tSPower / wildMonsterTamingResistance) * random * gamePace);
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
    }
}