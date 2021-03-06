using System.Collections.Generic;
using Assets.Scripts.Skill_System;
using UnityEngine;

namespace Assets.Scripts.Monster_System
{
    public interface IHaveMonsters
    {
        float GetMonsterSwitchRate();
        List<Monster> GetMonsters();
        void AddNewMonsterSlot(int slotNum, MonsterSlot newSlot);
        int CurrentSlotNumber();
        List<MonsterSlot> GetMonsterSlots();
        MonsterSlot GetMonsterWithHighestExp();

        Monster GetCurrentMonster();

        void SetAnimator(Animator animatorToChange);

        GameObject GetTamer();
        void ChangeMonsterUnitIndicatorRadius(float radius);

        void SpawnSwitchFX();

        void ChangeStatsToMonster(int slot);
    }
}