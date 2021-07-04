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

        public GameObject SpawnFromPool(Transform parent, string newSpawnedTag, GameObject prefabCheck, Vector3 position, Quaternion rotation)
        {
            while (true)
            {
                if (poolDictionary == null)
                {
                    poolDictionary = new Dictionary<string, Queue<GameObject>>();
                    continue;
                }

                if (!poolDictionary.ContainsKey(newSpawnedTag))
                {
                    AddNewPoolToDictionary(parent, newSpawnedTag, prefabCheck, 10);
                    continue;
                }

                GameObject objectToSpawn = poolDictionary[newSpawnedTag].Dequeue();

                objectToSpawn.SetActive(true);
                objectToSpawn.transform.position = position;
                objectToSpawn.transform.rotation = rotation;

                poolDictionary[newSpawnedTag].Enqueue(objectToSpawn);

                return objectToSpawn;
            }
        }

        private void AddNewPoolToDictionary(Transform parent, string newSpawnedTag, GameObject prefabCheck, int size)
        {
            //create a queue of Game objects
            Queue<GameObject> newPool = new Queue<GameObject>();

            Pool pool = new Pool
            {
                tag = newSpawnedTag,
                prefab = prefabCheck,
                size = size
            };
            //add all the objects in the pool depending on its size
            for (int i = 0; i < pool.size; i++)
            {
                if (poolDictionary.Count <= 1)
                {
                    if (i == 0)
                    {
                        InstantiateAndAddToPool(parent, pool, newPool);
                        continue; 
                    }
                }
                InstantiateAndAddToPool(null, pool, newPool);
            }
            //add pool to pools
            pools.Add(pool);
            //add it in the pool dictionary
            poolDictionary.Add(newSpawnedTag, newPool);
            Debug.LogWarning("Pool with tag " + newSpawnedTag + " doesn't exist.\nProceed to adding new pool.");
        }

        private GameObject InstantiateAndAddToPool(Transform parent, Pool pool, Queue<GameObject> objectPool)
        {
            //instantiating the object then setting it inactive
            GameObject obj = Instantiate(pool.prefab);
            obj.transform.parent = parent == null ? transform : parent;
            obj.SetActive(false);

            //put it in the new queue
            objectPool.Enqueue(obj);
            return obj;
        }
    }
}
