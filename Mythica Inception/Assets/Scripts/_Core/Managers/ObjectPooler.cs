using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;

namespace Assets.Scripts._Core.Managers
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
        [HideInInspector] public int objectInstatiated;
        [HideInInspector] public int totalObjectToInstantiate;
        [HideInInspector] public bool isDone;

        void Awake()
        {
            if (pools.Count > 0)
            {
                PreInstantiate();
                isDone = true;
            }
        }

        private void PreInstantiate()
        {
            foreach (var pool in pools) { totalObjectToInstantiate += pool.size; }
            
            foreach (var pool in pools.ToList())
            {
                if (pool.tag.IsNullOrEmpty())
                {
                    pool.tag = pool.prefab.name;
                }
                AddNewPoolToDictionary(transform, pool.tag, pool.prefab, pool.size);
            }
        }


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

                if (objectToSpawn == null || objectToSpawn.Equals(null))
                {
                    return null;
                }
                
                objectToSpawn.SetActive(true);
                
                if (parent != null)
                {
                    objectToSpawn.transform.SetParent(parent);
                    objectToSpawn.transform.localPosition = position;
                    objectToSpawn.transform.rotation = Quaternion.Euler(0,0,0);
                }
                else
                {
                    objectToSpawn.transform.SetParent(null);
                    objectToSpawn.transform.position = position;
                    objectToSpawn.transform.rotation = rotation;
                }

                poolDictionary[newSpawnedTag].Enqueue(objectToSpawn);

                return objectToSpawn;
            }
        }

        private void AddNewPoolToDictionary(Transform parent, string newSpawnedTag, GameObject prefabCheck, int size)
        {
            poolDictionary ??= new Dictionary<string, Queue<GameObject>>();
            
            if(poolDictionary.ContainsKey(newSpawnedTag)) return;
            
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
                        InstantiateAndAddToPool(pool, newPool);
                        continue; 
                    }
                }
                InstantiateAndAddToPool(pool, newPool);
                objectInstatiated++;
            }
            //add pool to pools
            pools.Add(pool);

            poolDictionary ??= new Dictionary<string, Queue<GameObject>>();
            
            //add it in the pool dictionary
            poolDictionary.Add(newSpawnedTag, newPool);
        }

        private GameObject InstantiateAndAddToPool(Pool pool, Queue<GameObject> objectPool)
        {
            //instantiating the object then setting it inactive
            GameObject obj = Instantiate(pool.prefab);
            obj.SetActive(false);

            //put it in the new queue
            objectPool.Enqueue(obj);
            return obj;
        }

        public void BackToPool(GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.rotation = Quaternion.Euler(0,0,0);
            obj.transform.parent = null;
        }
    }
}
