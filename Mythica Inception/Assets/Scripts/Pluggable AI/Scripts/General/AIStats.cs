using UnityEngine;

namespace Assets.Scripts.Pluggable_AI.Scripts.General
{
    [CreateAssetMenu(menuName = "Pluggable AI/AI Stats")]
    public class AIStats : ScriptableObject
    {
        public float combatDecisionEvery;
        public int searchDuration;
        public int searchingTurnSpeed;
        public float fleeDuration;
        public float movementSpeed;
        
    }
}
