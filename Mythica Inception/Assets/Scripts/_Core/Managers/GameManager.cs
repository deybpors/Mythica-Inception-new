using System.Collections.Generic;
using _Core.Others;
using Databases.Scripts;
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
        public bool gameplayActive;
        [HideInInspector] public List<Transform> enemiesSeePlayer;
        [HideInInspector] public string currentWorldScenePath;
        [HideInInspector] public Camera currentWorldCamera;
        [HideInInspector] public Player.Player player;

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(instance.gameObject);
            }
            else if (instance != null || instance != this)
            {
                Destroy(gameObject);
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
    }
}
