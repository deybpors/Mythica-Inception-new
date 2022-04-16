using UnityEngine;

namespace Assets.Scripts.Timeline.Cutscene_Trigger_Conditions
{
    [CreateAssetMenu(menuName = "Cutscene/Distance Trigger Condition")]
    public class DistanceCondition : CutsceneTriggerCondition
    {
        public float distanceToTrigger = 1.5f;

        public override bool MeetConditions(CutsceneTrigger triggerObject)
        {
            return Vector3.Distance(triggerObject.playerTransform.position, triggerObject._transform.position) <= distanceToTrigger;
        }
    }
}
