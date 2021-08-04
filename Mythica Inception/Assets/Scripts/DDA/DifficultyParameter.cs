using System.Collections.Generic;
using _Core.Others;
using UnityEngine;

namespace DDA
{
    public enum Difficulty
    {
        higher, lower    
    }
    
    [System.Serializable]
    public class DifficultyParameter
    {
        public string name;
        public float value = 1f;
        public float minValue;
        public float maxValue;
        public float valueAdjustment;
        public Difficulty increaseDifficulty;
        public List<string> dataNeeded;

        public void AdjustDifficultyParameterValue(Difficulty difficulty)
        {
            //if we want the difficulty to increase
            if (difficulty == Difficulty.higher)
            {
                //check if what should be the parameter to make it more difficult
                switch (increaseDifficulty)
                {
                    //if the parameter should be higher to make it difficult, then increase
                    case Difficulty.higher:
                        value += valueAdjustment;
                        break;
                    //if the parameter should be lower to make it difficult, then decrease
                    case Difficulty.lower:
                        value -= valueAdjustment;
                        break;
                }
            }
            //if we want the difficulty to decrease
            else
            {
                //check if what should be the parameter to make it more difficult
                switch (increaseDifficulty)
                {
                    //if the parameter should be higher to make it difficult, then decrease
                    case Difficulty.higher:
                        value -= valueAdjustment;
                        break;
                    //if the parameter should be lower to make it difficult, then increase
                    case Difficulty.lower:
                        value += valueAdjustment;
                        break;
                }
            }
            
            value = Clamp(value, minValue, maxValue);
        }
        
        public static float Clamp( float value, float min, float max )
        {
            return (value < min) ? min : (value > max) ? max : value;
        }
    }
}
