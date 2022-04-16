using Monster_System;
using MyBox;
using Quest_System;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest System/Quest Goal/Kill Goal")]
public class KillGoal : QuestGoal
{
    public bool anyMonster;
    [ConditionalField(nameof(anyMonster), true)] public Monster monsterToKill;

    public void EnemyKilled(PlayerAcceptedQuest acceptedQuest, Monster monsterKilled, out int updatedAmount)
    {
        updatedAmount = acceptedQuest.currentAmount;

        //checks if any monster is true or if false when monsterToKill and the monsterKilled is true
        if ((anyMonster || monsterToKill != monsterKilled) && !anyMonster) return;
        
        acceptedQuest.currentAmount++;
        updatedAmount = acceptedQuest.currentAmount;
    }
}
