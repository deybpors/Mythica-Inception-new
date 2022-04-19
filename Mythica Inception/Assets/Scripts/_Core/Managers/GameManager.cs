using System.Collections.Generic;
using _Core.Input;
using _Core.Others;
using Assets.Scripts._Core.Managers;
using Assets.Scripts._Core.Player;
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
        [HideInInspector] public Light mainLight;
        [HideInInspector] public Terrain currentTerrain;

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
            if(loadedSaveData.optionsSaveData.difficulty != OptionsSaveData.DifficultyOptions.Dynamic) return;
            var dataNeeded = difficultyManager.GetDataNeeded(dataKey);
            if(dataNeeded == null) return;
            var dataValue = dataNeeded.value;
            
            dataNeeded.ChangeValue(dataValue + valueToAdd);
            difficultyManager.DataAdjusted(dataKey);
        }
        
        public void DifficultyUpdateChange(string dataKey, float newValue)
        {
            if (loadedSaveData.optionsSaveData.difficulty != OptionsSaveData.DifficultyOptions.Dynamic) return;
            var dataNeeded = difficultyManager.GetDataNeeded(dataKey);
            if (dataNeeded == null) return;
            dataNeeded.ChangeValue(newValue);
            difficultyManager.DataAdjusted(dataKey);
        }

        #endregion
        
        
        public void UpdateEnemiesSeePlayer(MonsterTamerAI ai, out int enemyCount)
        {
            enemyCount = enemiesSeePlayer.Count;
            for (var i = 0; i < enemyCount; i++)
            {
                var enemySee = enemiesSeePlayer[i];

                if (ai.thisTransform != enemySee) continue;
                
                enemiesSeePlayer.Remove(enemySee);
                enemyCount--;
                break;
            }

            if (enemyCount > 0) return;

            DifficultyUpdateChange("Average Party Level", GameSettings.MonstersAvgLevel(player.monsterSlots));
        }
    }
}
