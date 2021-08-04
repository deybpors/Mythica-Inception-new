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

        public void DataAdjusted(string dataName)
        {
            dataName = dataName.Replace(" ", string.Empty).ToLower();

            if (dictParamDataNeeded.ContainsKey(dataName))
            {
                var parametersToAdjust = new List<DifficultyParameter>();
                
                //check which parameter has this needed data and add it to parameters to adjust
                foreach (var parameter in difficultyParameters)
                {
                    foreach (var dataNeededName in parameter.dataNeeded)
                    {
                        var dataNeededNameLower = dataNeededName.Replace(" ", string.Empty).ToLower();
                        
                        if (dataNeededNameLower.Equals(dataName))
                        {
                            parametersToAdjust.Add(parameter);
                        }
                    }   
                }


                //adjust all data in parameters to adjust
                foreach (var parameter in parametersToAdjust)
                {
                    //check if we should increase difficulty
                    if (dictParamDataNeeded[dataName].IncreaseDifficulty())
                    {
                        Debug.Log("Increasing difficulty to " + parameter.name);
                        parameter.AdjustDifficultyParameterValue(Difficulty.higher);
                        //return;
                    }
                
                    //check if we should decrease difficulty
                    if (dictParamDataNeeded[dataName].DecreaseDifficulty())
                    {
                        Debug.Log("Decreasing difficulty to " + parameter.name);
                        parameter.AdjustDifficultyParameterValue(Difficulty.lower);   
                        //break;   
                    }
                }
            }
        }
    }
}
