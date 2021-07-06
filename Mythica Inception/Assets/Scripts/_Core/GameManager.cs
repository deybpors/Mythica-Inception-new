using Assets.Scripts.Databases.Scripts;
using UnityEngine;

namespace Assets.Scripts._Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance = null;
        public ObjectPooler pooler;
        
        [Header("Databases")]
        public Database monstersDatabase;
        public Database skillsDatabase;
        public Database itemsDatabase;
        
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
