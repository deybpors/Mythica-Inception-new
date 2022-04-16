using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Timeline;
using UnityEngine;

[CreateAssetMenu(menuName = "Cutscene/Have Monster Trigger Condition")]
public class HaveMonsterCondition : CutsceneTriggerCondition
{
    public override bool MeetConditions(CutsceneTrigger triggerObject)
    {
        var haveMonsters = false;
        var monsterSlotsCount = triggerObject.player.monsterSlots.Count;
        for (var i = 0; i < monsterSlotsCount; i++)
        {
            var slot = triggerObject.player.monsterSlots[i];
            haveMonsters = slot.monster != null;
            if (haveMonsters)
            {
                break;
            }
        }

        return haveMonsters;
    }
}
