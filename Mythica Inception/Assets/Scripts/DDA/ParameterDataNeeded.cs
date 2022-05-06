using MyBox;
using UnityEngine;


namespace DDA
{
    [System.Serializable]
    public class ParameterDataNeeded
    {
        public string name;
        public float value;
        public float previousValue;

        public Difficulty increaseDifficulty;
        [ConditionalField(nameof(decreaseDifficultyOnEquals), true)]
        public bool increaseDifficultyOnEquals;

        [ConditionalField(nameof(increaseDifficultyOnEquals), true)]
        public bool decreaseDifficultyOnEquals;

        public bool IncreaseDifficulty()
        {
            if (increaseDifficultyOnEquals && previousValue.Equals(value))
            {
                return true;
            }


            switch (increaseDifficulty)
            {
                case Difficulty.higher:
                    return value > previousValue;
                case Difficulty.lower:
                    return value < previousValue;
                default:
                    return false;
            }
        }

        public bool DecreaseDifficulty()
        {
            if (decreaseDifficultyOnEquals && previousValue.Equals(value))
            {
                return true;
            }


            switch (increaseDifficulty)
            {
                case Difficulty.higher:
                    return value < previousValue;
                case Difficulty.lower:
                    return value > previousValue;
                default:
                    return false;
            }
        }

        public void ChangeValue(float newValue)
        {
            previousValue = value;
            value = newValue;
        }
    }
}