using System.Collections.Generic;
using Assets.Scripts.Databases;
using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts._Core.Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;
        public ObjectPooler pooler;
        public Camera mainCamera;
        public DatabaseManager databaseManager;
        public AudioManager audioManager;
        public UIManager uiManager;
        [HideInInspector] public Transform player;
        public List<Transform> enemiesSeePlayer;
        void Awake()
        {
            if (instance == null)
            {
                instance = this;
                // DontDestroyOnLoad(instance.gameObject);
            }
            else if (instance != null || instance != this)
            {
                Destroy(gameObject);
            }

            databaseManager.InitializeTypeChartData();
            player = FindObjectOfType<Player.Player>().transform;
        }
    }
}
