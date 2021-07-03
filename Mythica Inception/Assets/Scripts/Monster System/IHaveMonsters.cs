using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Monster_System
{
    public interface IHaveMonsters
    {
        float GetMonsterSwitchRate();
        int MonsterSwitched();
        List<GameObject> GetMonsters();

        bool isPlayerSwitched();

        void SetAnimator(Animator animatorToChange);

        void Deactivate();
    }
}