using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Databases.Scripts
{
    public class DatabaseManager : MonoBehaviour
    {
        [SerializeField] private TextAsset _monsterTypeChart;
        [SerializeField] private TextAsset _npcNames;
        [HideInInspector] public List<string> attackerTypes;
        [HideInInspector] public List<string> defenseTypes;
        [HideInInspector] public readonly List<List<float>> typeChart = new List<List<float>>();
        [HideInInspector] private List<string> _npcNamesList = new List<string>();

        void Start()
        {
            if(_npcNames == null) return;
            InitializeNPCNames();
        }

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

        private void InitializeNPCNames()
        {
            var names = _npcNames.text.Split('\n');
            _npcNamesList = names.ToList();
        }

        public string GetRandomNPCName()
        {
            if (_npcNames == null) return string.Empty;
            var num = Random.Range(0, _npcNamesList.Count);
            var npc = _npcNamesList[num];

            try
            {
                npc = npc.Replace(" ", string.Empty).ToLowerInvariant();
                npc = char.ToUpperInvariant(npc[0]) + npc.Substring(1);
            }
            catch
            {
                GetRandomNPCName();
            }

            return npc;
        }
    }
}