using UnityEngine;

namespace Quest_System
{
    public abstract class QuestGoal : ScriptableObject
    {
        public int requiredAmount = 5;
        
        public virtual void Initialization(){}

        public bool IsComplete(int currentAmount)
        {
            return currentAmount >= requiredAmount;
        }
    }
}