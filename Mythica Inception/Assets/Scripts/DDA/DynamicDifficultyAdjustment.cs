using System.Collections.Generic;
using _Core.Managers;
using TMPro;
using UnityEngine;

namespace DDA
{
    public class DynamicDifficultyAdjustment : MonoBehaviour
    {
        public List<DifficultyParameter> difficultyParameters;
        public List<ParameterDataNeeded> parameterDataNeeded;

        private Dictionary<string, ParameterDataNeeded> dictParamDataNeeded = new Dictionary<string, ParameterDataNeeded>();
        private Dictionary<string, DifficultyParameter> dictDiffParam = new Dictionary<string, DifficultyParameter>();
        public HashSet<string> _parameter = new HashSet<string>();
        private TextMeshProUGUI ddaText;

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

        public double GetParameterValue(string parameterName)
        {
            _parameter.Clear();
            parameterName = parameterName.Replace(" ", string.Empty).ToLower();
            return dictDiffParam.ContainsKey(parameterName) ? dictDiffParam[parameterName].value : 0;
        }

        public double GetParameterMaxValue(string parameterName)
        {
            parameterName = parameterName.Replace(" ", string.Empty).ToLower();
            return dictDiffParam.ContainsKey(parameterName) ? dictDiffParam[parameterName].maxValue : 0;
        }

        public double GetParameterMinValue(string parameterName)
        {
            parameterName = parameterName.Replace(" ", string.Empty).ToLower();
            return dictDiffParam.ContainsKey(parameterName) ? dictDiffParam[parameterName].minValue : 0;
        }

        public ParameterDataNeeded GetDataNeeded(string dataName)
        {
            dataName = dataName.Replace(" ", string.Empty).ToLower();
            return dictParamDataNeeded.ContainsKey(dataName) ? dictParamDataNeeded[dataName] : null;
        }

        //DataAdjusted tells the system that data has been adjusted
        public void DataAdjusted(string dataNeeded)
        {
            dataNeeded = dataNeeded.Replace(" ", string.Empty).ToLower();

            if (!dictParamDataNeeded.ContainsKey(dataNeeded)) return;

            //check which _parameter has this needed data and adjust the _parameter's value
            var parametersCount = difficultyParameters.Count;

            for (var i = 0; i < parametersCount; i++)
            {
                if (!difficultyParameters[i].HasData(dataNeeded)) continue;
                var parameter = difficultyParameters[i];
                var count = parameter.dataNeeded.Count;
                
                for (var j = 0; j < count; j++)
                {
                    var dataNeededName = parameter.dataNeeded[j].Replace(" ", string.Empty).ToLower();

                    if (dataNeededName != dataNeeded) continue;
                    
                    AdjustParameter(dataNeeded, parameter);
                }
            }
        }

        private void AdjustParameter(string dataNeeded, DifficultyParameter parameter)
        {
            //check if we should increase difficulty
            if (dictParamDataNeeded[dataNeeded].IncreaseDifficulty())
            {
                try
                {
                    _parameter.Add(parameter.name);
                    parameter.AdjustDifficultyParameterValue(Difficulty.higher);
                    ChangeDDAText();
                }
                catch
                {
                    _parameter.Remove(parameter.name);
                }
            }

            //check if we should decrease difficulty
            if (!dictParamDataNeeded[dataNeeded].DecreaseDifficulty()) return;

            try
            {
                _parameter.Add(parameter.name);
                parameter.AdjustDifficultyParameterValue(Difficulty.lower);
                ChangeDDAText();
            }
            catch
            {
                _parameter.Remove(parameter.name);
            }
        }

        public void ChangeValuesFromSaved()
        {
            var paramCount = difficultyParameters.Count;
            var dataCount = parameterDataNeeded.Count;

            for (var i = 0; i < paramCount; i++)
            {
                if (!GameManager.instance.saveManager.LoadDataObject(difficultyParameters[i].name, out double value))
                    continue;
                difficultyParameters[i].value = value;
            }

            for (var i = 0; i < dataCount; i++)
            {
                if (!GameManager.instance.saveManager.LoadDataObject(parameterDataNeeded[i].name, out float value))
                    continue;
                parameterDataNeeded[i].value = value;
                parameterDataNeeded[i].previousValue = value;
            }
            ChangeDDAText();
        }

        private void SaveParameterValues()
        {
            var paramCount = difficultyParameters.Count;
            var dataCount = parameterDataNeeded.Count;

            for (var i = 0; i < paramCount; i++)
            {
                GameManager.instance.saveManager.SaveOtherData(difficultyParameters[i].name, difficultyParameters[i].value);
            }

            for (var i = 0; i < dataCount; i++)
            {
                GameManager.instance.saveManager.SaveOtherData(parameterDataNeeded[i].name, parameterDataNeeded[i].value);
            }
        }

        private void ChangeDDAText()
        {
            if (ddaText == null)
            {
                ddaText = GameManager.instance.uiManager.debugConsole.DDAText;
            }

            var count = difficultyParameters.Count;
            ddaText.text = string.Empty;
            for (var i = 0; i < count; i++)
            {
                var text = difficultyParameters[i].name + ": " + difficultyParameters[i].value.ToString("0.00") + "\t\t\t\t\t\t";
                ddaText.text += text;
            }
        }

        void OnApplicationQuit()
        {
            SaveParameterValues();
        }
    }
}
