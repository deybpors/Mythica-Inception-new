using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Core
{
    public interface IHaveMonsters
    {
        float GetMonsterSwitchRate();
        int MonsterSwitched();
        List<GameObject> GetMonsters();
    }
}