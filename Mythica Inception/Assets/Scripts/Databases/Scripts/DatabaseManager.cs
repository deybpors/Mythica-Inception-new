using System.Collections.Generic;
using Assets.Scripts.Databases.Scripts;
using UnityEngine;

namespace Assets.Scripts.Databases
{
    public class DatabaseManager : MonoBehaviour
    {
        public Database monstersDatabase;
        public Database skillsDatabase;
        public Database itemsDatabase;
        
        [SerializeField] private TextAsset _monsterTypeChart;
        [HideInInspector] public List<string> attackerTypes;
        [HideInInspector] public List<string> defenseTypes;
        [HideInInspector] public readonly List<List<float>> typeChart = new List<List<float>>();

        public void InitializeTypeChartData()
        {
            if (_monsterTypeChart == null) { return; }
            
            attackerTypes.Clear();
            defenseTypes.Clear();
            
            var typeChartData = _monsterTypeChart.text.Split('\n');

            for (var i = 0; i < typeChartData.Length; i++)
            {
                var separation = typeChartData[i].Split(',');
                var newLine = new List<float>();
                
                for (var j = 0; j < separation.Length; j++)
                {
                    if (i == 0)
                    {
                        if (j >= 1)
                        {
                            defenseTypes.Add(separation[j]);
                        }
                    }
                    else
                    {
                        if (j == 0)
                        {
                            attackerTypes.Add(separation[j]);
                        }
                        else
                        {
                            newLine.Add(float.Parse(separation[j]));
                        }
                    }
                }

                if (i > 0)
                {
                    typeChart.Add(newLine);
                }
            }
        }
        
    }
}