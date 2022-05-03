using System.Collections;
using System.Collections.Generic;
using _Core.Input;
using _Core.Others;
using Assets.Scripts._Core.Managers;
using Assets.Scripts._Core.Player;
using Cinemachine;
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
        [ReadOnly] public List<Transform> enemiesSeePlayer;
        [HideInInspector] public Dictionary<Transform, GameObject> activeEnemies = new Dictionary<Transform, GameObject>();
        private Dictionary<GameObject, Transform> _enemiesTransforms = new Dictionary<GameObject, Transform>();

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
        private CinemachineBasicMultiChannelPerlin _perlinNoise;

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

            audioManager.PlayMusic(audioManager.currentSituation);
            DifficultyUpdateChange("Average Party Level", GameSettings.MonstersAvgLevel(player.monsterSlots));
        }

        public void Screenshake(float intensity, float shakeTime)
        {
            if (_perlinNoise == null)
            {
                _perlinNoise = player.virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            }

            _perlinNoise.m_AmplitudeGain = intensity;
            StopAllCoroutines();
            StartCoroutine(FadeScreenShake(shakeTime));
        }

        public void RemoveInActiveEnemies(GameObject activeEnemy)
        {
            if (!_enemiesTransforms.TryGetValue(activeEnemy, out var trans))
            {
                trans = activeEnemy.transform;
                _enemiesTransforms.Add(activeEnemy, trans);
            }

            activeEnemies.Remove(trans);
        }

        private IEnumerator FadeScreenShake(float shakeTime)
        {
            var timeElapsed = 0f;
            while (timeElapsed <= shakeTime)
            {
                timeElapsed += Time.deltaTime;
                _perlinNoise.m_AmplitudeGain = Mathf.Lerp(_perlinNoise.m_AmplitudeGain, 0, timeElapsed / shakeTime);

                yield return null;
            }
        }
    }
}
