using System.Collections.Generic;
using _Core.Managers;
using UnityEngine;

namespace DDA
{
    public class DynamicDifficultyAdjustment : MonoBehaviour
    {
        public List<DifficultyParameter> difficultyParameters;
        public List<ParameterDataNeeded> parameterDataNeeded;
        private Dictionary<string, ParameterDataNeeded> dictParamDataNeeded = new Dictionary<string, ParameterDataNeeded>();
        private Dictionary<string, DifficultyParameter> dictDiffParam = new Dictionary<string, DifficultyParameter>();

        void Awake()
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

        private void AdjustParameter(string dataNeeded, DifficultyParameter parameter)
        {
            //check if we should increase difficulty
            if (dictParamDataNeeded[dataNeeded].IncreaseDifficulty())
            {
                GameManager.instance.uiManager.debugConsole.DisplayLogUI("Increasing difficulty by changing values to " + parameter.name);
                parameter.AdjustDifficultyParameterValue(Difficulty.higher);
            }

            //check if we should decrease difficulty
            if (!dictParamDataNeeded[dataNeeded].DecreaseDifficulty()) return;

            GameManager.instance.uiManager.debugConsole.DisplayLogUI("Decreasing difficulty by changing values to " + parameter.name);
            parameter.AdjustDifficultyParameterValue(Difficulty.lower);
        }


        //DataAdjusted Method tells the system that data has been adjusted
        public void DataAdjusted(string dataNeeded)
        {
            dataNeeded = dataNeeded.Replace(" ", string.Empty).ToLower();

            if (!dictParamDataNeeded.ContainsKey(dataNeeded)) return;

            //check which parameter has this needed data and adjust the parameter's value
            var parametersCount = difficultyParameters.Count;
            
            for (var i = 0; i < parametersCount; i++)
            {
                if (difficultyParameters[i].HasData(dataNeeded))
                {
                    AdjustParameter(dataNeeded, difficultyParameters[i]);
                }
            }
        }
    }
}
