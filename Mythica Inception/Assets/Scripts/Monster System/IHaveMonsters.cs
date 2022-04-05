using System.Collections.Generic;
using _Core.Others;
using UnityEngine;

namespace Monster_System
{
    public interface IHaveMonsters : IEntity
    {
        float GetMonsterSwitchRate();

        List<Monster> GetMonsters();

        void AddNewMonsterSlot(int slotNum, MonsterSlot newSlot);

        int GetCurrentSlotNumber();

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