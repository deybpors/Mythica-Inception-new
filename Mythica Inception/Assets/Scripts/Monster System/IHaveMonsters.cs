using System.Collections.Generic;
using Assets.Scripts.Skill_System;
using UnityEngine;

namespace Assets.Scripts.Monster_System
{
    public interface IHaveMonsters
    {
        float GetMonsterSwitchRate();
        int MonsterSwitched();
        List<Monster> GetMonsters();

        List<MonsterSlot> GetMonsterSlots();

        Monster GetCurrentMonster();

        bool isPlayerSwitched();

        void SetAnimator(Animator animatorToChange);

        GameObject GetTamer();
        void ChangeMonsterUnitIndicatorRadius(float radius);

        void SpawnSwitchFX();
    }
}