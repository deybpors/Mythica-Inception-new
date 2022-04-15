using System.Collections.Generic;
using _Core.Input;
using _Core.Others;
using Assets.Scripts._Core.Managers;
using Databases.Scripts;
using DDA;
using MyBox;
using Pluggable_AI.Scripts.General;
using Pluggable_AI.Scripts.States;
using Quest_System;
using SoundSystem;
using UI;
using UnityEngine;

namespace _Core.Managers
{
    public class GameManager : MonoBehaviour
    {
        [Foldout("Managers", true)]
        public static GameManager instance;
        public DatabaseManager databaseManager;
        public UIManager uiManager;
        public AudioManager audioManager;
        public GameSceneManager gameSceneManager;
        public ObjectPooler pooler;
        public SceneReference gameplayScene;
        public DynamicDifficultyAdjustment difficultyManager;
        public QuestManager questManager;
        public SaveManager saveManager;
        public PauseManager pauseManager;
        public PlayerInputHandler inputHandler;
        public TimelineManager timelineManager;
        public StateController gameStateController;
        public List<Transform> enemiesSeePlayer;

        [Foldout("States", true)]
        public State gameplayState;
        public State UIState;
        public State dialogueState;
        public State cutsceneState;

        [HideInInspector] public int currentWorldScenePath = -1;
        [HideInInspector] public Camera currentWorldCamera;
        [HideInInspector] public Player.Player player;
        [HideInInspector] public PlayerSaveData loadedSaveData;

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

        void Start()
        {
            audioManager.PlayMusic(MusicMood.Calm);
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
