using System.Collections.Generic;
using Assets.Scripts.Databases.Scripts;
using Assets.Scripts.Monster_System;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts._Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance = null;
        public ObjectPooler pooler;
        public Camera mainCamera;
        
        [Header("Databases")]
        public Database monstersDatabase;
        public Database skillsDatabase;
        public Database itemsDatabase;
        public TextAsset monsterTypeChart;
        
        [HideInInspector] public List<string> attackerTypes;
        [HideInInspector] public List<string> defenseTypes;
        [HideInInspector] public List<List<float>> typeChart = new List<List<float>>();

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }

            if (instance != this)
            {
                Destroy(gameObject);
            }
            
            InitializeTypeChartData();
        }

        private void InitializeTypeChartData()
        {
            attackerTypes.Clear();
            defenseTypes.Clear();
            
            //split per line
            string[] typeChartData = monsterTypeChart.text.Split('\n');

            for (int i = 0; i < typeChartData.Length; i++)
            {
                string[] separation = typeChartData[i].Split(',');
                var newLine = new List<float>();
                
                for (int j = 0; j < separation.Length; j++)
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
