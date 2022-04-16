using UnityEngine;

namespace Assets.Scripts.Timeline.Cutscene_Trigger_Conditions
{
    [CreateAssetMenu(menuName = "Cutscene/Take Damage Trigger Condition")]
    public class TakeDamageCondition : CutsceneTriggerCondition
    {
        public override bool MeetConditions(CutsceneTrigger triggerObject)
        {
            return triggerObject.player.playerHealth.currentHealth < triggerObject.player.playerHealth.maxHealth;
        }
    }
}
