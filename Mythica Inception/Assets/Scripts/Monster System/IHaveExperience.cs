namespace Monster_System
{
    public interface IHaveExperience
    {
        void AddExperience(int experienceToAdd);
        int GetCurrentLevel();

        void LevelUp();
    }
}