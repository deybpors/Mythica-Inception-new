using System.Collections.Generic;
using _Core.Others;
using Databases.Scripts;
using DDA;
using Quest_System;
using UI;
using UnityEngine;

namespace _Core.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;
        public DatabaseManager databaseManager;
        public UIManager uiManager;
        public AudioManager audioManager;
        public GameSceneManager gameSceneManager;
        public ObjectPooler pooler;
        public ScenePicker gameplayScene;
        public DynamicDifficultyAdjustment difficultyManager;
        public QuestManager questManager;
        public SaveManager saveManager;
        public bool gameplayActive;
        public List<Transform> enemiesSeePlayer;
        [HideInInspector] public string currentWorldScenePath = string.Empty;
        [HideInInspector] public Camera currentWorldCamera;
        [HideInInspector] public Player.Player player;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(instance.gameObject);
            }
            else
            {
                Destroy(instance.gameObject);
                instance = this;
                DontDestroyOnLoad(instance.gameObject);
            }
            
            databaseManager.InitializeTypeChartData();
        }

        public void InitializePlayerReference(Player.Player p)
        {
            player = p;
        }

        public void InitializeCurrentWorldCamera(Camera cam)
        {
            currentWorldCamera = cam;
        }

        #region Difficulty Adjustment

        public void DifficultyUpdateAdd(string dataKey, float valueToAdd)
        {
            var data = difficultyManager.GetDataNeeded(dataKey);
            var dataValue = data.value;
            data.ChangeValue(dataValue + valueToAdd);
            difficultyManager.DataAdjusted(dataKey);
        }
        
        public void DifficultyUpdateChange(string dataKey, float newValue)
        {
            var data = difficultyManager.GetDataNeeded(dataKey);
            data.ChangeValue(newValue);
            difficultyManager.DataAdjusted(dataKey);
        }

        #endregion
        
        
        public void UpdateEnemiesSeePlayer(Object enemyTransform, out int enemyCount)
        {
            enemyCount = enemiesSeePlayer.Count;
            for (var i = 0; i < enemyCount; i++)
            {
                var enemy = enemiesSeePlayer[i];
                if (enemyTransform != enemy) continue;
                enemiesSeePlayer.Remove(enemy);
                enemyCount--;
                break;
            }
        }
    }
}
