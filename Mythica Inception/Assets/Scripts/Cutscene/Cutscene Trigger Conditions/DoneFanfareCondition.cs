using _Core.Managers;
using Assets.Scripts.Timeline;
using UnityEngine;

[CreateAssetMenu(menuName = "Cutscene/Fanfare Done Trigger Condition")]
public class DoneFanfareCondition : CutsceneTriggerCondition
{
    public override bool MeetConditions(CutsceneTrigger triggerObject)
    {
        return !GameManager.instance.uiManager.monsterTamedUi.FanfarePlaying();
    }
}
