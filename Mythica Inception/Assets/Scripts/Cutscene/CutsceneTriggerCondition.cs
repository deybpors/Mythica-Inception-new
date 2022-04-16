using UnityEngine;

namespace Assets.Scripts.Timeline
{
    public abstract class CutsceneTriggerCondition : ScriptableObject
    {
        public abstract bool MeetConditions(CutsceneTrigger triggerObject);
    }
}
