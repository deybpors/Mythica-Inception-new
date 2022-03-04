using System.Collections.Generic;
using UnityEngine;

namespace DDA
{
    public class DynamicDifficultyAdjustment : MonoBehaviour
    {
        public List<DifficultyParameter> difficultyParameters;
        public List<ParameterDataNeeded> parameterDataNeeded;
        private Dictionary<string, ParameterDataNeeded> dictParamDataNeeded = new Dictionary<string, ParameterDataNeeded>();
        private Dictionary<string, DifficultyParameter> dictDiffParam = new Dictionary<string, DifficultyParameter>();

        private void Awake()
        {
            var paramCount = difficultyParameters.Count;
            var dataCount = parameterDataNeeded.Count;

            for (var i = 0; i < paramCount; i++)
            {
                dictDiffParam.Add(difficultyParameters[i].name.Replace(" ", string.Empty).ToLower(), difficultyParameters[i]);
            }

            for (var i = 0; i < dataCount; i++)
            {
                dictParamDataNeeded.Add(parameterDataNeeded[i].name.Replace(" ", string.Empty).ToLower(), parameterDataNeeded[i]);
            }
        }

        public float GetParameterValue(string parameterName)
        {
            parameterName = parameterName.Replace(" ", string.Empty).ToLower();
            return dictDiffParam.ContainsKey(parameterName) ? dictDiffParam[parameterName].value : 0;
        }

        public ParameterDataNeeded GetDataNeeded(string dataName)
        {
            dataName = dataName.Replace(" ", string.Empty).ToLower();
            return dictParamDataNeeded.ContainsKey(dataName) ? dictParamDataNeeded[dataName] : null;
        }

        private void AdjustParameter(string dataName, DifficultyParameter parameter)
        {
            //check if we should increase difficulty
            if (dictParamDataNeeded[dataName].IncreaseDifficulty())
            {
                Debug.Log("Increasing difficulty to " + parameter.name);
                parameter.AdjustDifficultyParameterValue(Difficulty.higher);
            }

            //check if we should decrease difficulty
            if (!dictParamDataNeeded[dataName].DecreaseDifficulty()) return;
            
            Debug.Log("Decreasing difficulty to " + parameter.name);
            parameter.AdjustDifficultyParameterValue(Difficulty.lower);
        }


        //DataAdjusted Method is the main method for adjusting the Difficulty
        public void DataAdjusted(string dataName)
        {
            dataName = dataName.Replace(" ", string.Empty).ToLower();

            if (!dictParamDataNeeded.ContainsKey(dataName)) return;

            //check which parameter has this needed data and adjust the parameter's value
            var parametersCount = difficultyParameters.Count;
            for (var i = 0; i < parametersCount; i++)
            {
                var parameter = difficultyParameters[i];
                var dataNeededInParameterCount = parameter.dataNeeded.Count;
                for (var j = 0; j < dataNeededInParameterCount; j++)
                {
                    var dataNeededName = parameter.dataNeeded[j].Replace(" ", string.Empty).ToLower();

                    if (dataNeededName.Equals(dataName))
                    {
                        AdjustParameter(dataName, parameter);
                    }
                }
            }

        }
    }
}
