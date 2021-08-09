namespace Quest_System
{
    public enum QuestGoalType
    {
        kill, gather
    }
    
    [System.Serializable]
    public class QuestGoal
    {
        public QuestGoalType type;
        public int requiredValue;
    }

}