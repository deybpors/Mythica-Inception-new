using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Core
{
    public class ObjectPooler : MonoBehaviour
    {
        [System.Serializable]
        public class Pool
        {
            public string tag;
            public GameObject prefab;
            public int size;
        }

        public List<Pool> pools;
        public Dictionary<string, Queue<GameObject>> poolDictionary;

        void Start()
        {
            poolDictionary = new Dictionary<string, Queue<GameObject>>();
            
            //for each pool created
            foreach (Pool pool in pools)
            {
                //create a queue of Game objects
                Queue<GameObject> objectPool = new Queue<GameObject>();
                
                //add all the objects in the pool depending on its size
                for (int i = 0; i < pool.size; i++)
                {
                    InstantiateAndAddToPool(pool, objectPool);
                }
                
                //add it in the pool dictionary
                poolDictionary.Add(pool.tag, objectPool);
            }
        }

        public GameObject SpawnFromPool(string newSpawnedTag, GameObject prefabCheck, Vector3 position, Quaternion rotation)
        {
            if (!poolDictionary.ContainsKey(newSpawnedTag))
            {
                AddNewPool(newSpawnedTag, prefabCheck);
                return null;
            }
            
            GameObject objectToSpawn = poolDictionary[newSpawnedTag].Dequeue();
            
            //if the object is currently active in hierarchy (used)
            if (objectToSpawn.activeInHierarchy)
            {
                poolDictionary[newSpawnedTag].Enqueue(objectToSpawn);
                //then let's add another in the pool
                int index = 0;
                for (int i = 0; i < pools.Count; i++)
                {
                    if (pools[i].tag.Equals(newSpawnedTag))
                    {
                        index = i;
                        break;
                    }
                }
                objectToSpawn = InstantiateAndAddToPool(pools[index], poolDictionary[newSpawnedTag]);
            }
            
            objectToSpawn.SetActive(true);
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;
           
            poolDictionary[newSpawnedTag].Enqueue(objectToSpawn);

            return objectToSpawn;
        }

        private void AddNewPool(string newSpawnedTag, GameObject prefabCheck)
        {
            Queue<GameObject> newPool = new Queue<GameObject>();

            Pool pool = new Pool
            {
                tag = newSpawnedTag,
                prefab = prefabCheck,
                size = 10
            };

            for (int i = 0; i < pool.size; i++)
            {
                InstantiateAndAddToPool(pool, newPool);
            }

            poolDictionary.Add(pool.tag, newPool);
            Debug.LogWarning("Pool with tag " + newSpawnedTag + " doesn't exist.\nProceed to adding new pool.");
        }

        private GameObject InstantiateAndAddToPool(Pool pool, Queue<GameObject> objectPool)
        {
            //instantiating the object then setting it inactive
            GameObject obj = Instantiate(pool.prefab);
            obj.transform.parent = this.transform;
            obj.SetActive(false);

            //put it in the new queue
            objectPool.Enqueue(obj);
            return obj;
        }
    }
}
