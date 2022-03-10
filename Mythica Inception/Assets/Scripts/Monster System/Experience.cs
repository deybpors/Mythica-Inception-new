using _Core.Others;
using UnityEngine;

namespace Monster_System
{
    public class Experience : MonoBehaviour, IHaveExperience
    {
        public int currentLevel;
        public int currentExperience;
        public int nextLevelExpRequirement;

        public void InitializeLevel()
        {
            currentLevel = GameSettings.Level(currentExperience);
        }

        public void AddExperience(int experienceToAdd)
        {
            currentExperience += experienceToAdd;

            if (currentExperience >= nextLevelExpRequirement)
            {
                LevelUp();
            }
        }

        public int GetCurrentLevel()
        {
            return currentLevel;
        }

        public void LevelUp()
        {
            currentLevel++;
        }
    }
}