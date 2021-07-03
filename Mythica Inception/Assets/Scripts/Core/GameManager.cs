using Assets.Scripts.Databases.Scripts;
using Assets.Scripts.Monster_System;
using Assets.Scripts.Skill_System;
using UnityEngine;

namespace Assets.Scripts.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance = null;
        
        [Header("Databases")]
        public Database monstersDatabase;
        public Database skillsDatabase;

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
        }
    }
}
