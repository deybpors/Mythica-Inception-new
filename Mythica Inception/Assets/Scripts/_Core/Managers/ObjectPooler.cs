using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;

namespace _Core.Managers
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


        public Transform masterParent;
        public List<Pool> pools;
        public Dictionary<string, Queue<GameObject>> poolDictionary;
        private readonly Vector3 zero = Vector3.zero;
        private void PreInstantiate(List<Pool> p)
        {
            pools = p;
            if (pools.Count <= 0) return;
            
            foreach (var pool in p.ToList())
            {
                if (pool.tag.IsNullOrEmpty())
                {
                    pool.tag = pool.prefab.name;
                }
                AddNewPoolToDictionary(transform, pool.tag, pool.prefab, pool.size, zero);
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
                    AddNewPoolToDictionary(parent, newSpawnedTag, prefabCheck, 10, position);
                    continue;
                }

                var objectToSpawn = poolDictionary[newSpawnedTag].Dequeue();

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
                    objectToSpawn.transform.SetParent(masterParent);
                    objectToSpawn.transform.position = position;
                    objectToSpawn.transform.rotation = rotation;
                }

                poolDictionary[newSpawnedTag].Enqueue(objectToSpawn);

                return objectToSpawn;
            }
        }

        private void AddNewPoolToDictionary(Transform parent, string newSpawnedTag, GameObject prefabCheck, int size, Vector3 position)
        {
            poolDictionary ??= new Dictionary<string, Queue<GameObject>>();
            
            if(poolDictionary.ContainsKey(newSpawnedTag)) return;
            
            //create a queue of Game objects
            var newPool = new Queue<GameObject>();

            var pool = new Pool
            {
                tag = newSpawnedTag,
                prefab = prefabCheck,
                size = size
            };
            //add all the objects in the pool depending on its size
            for (var i = 0; i < pool.size; i++)
            {
                if (poolDictionary.Count <= 1)
                {
                    if (i == 0)
                    {
                        InstantiateAndAddToPool(pool, newPool, position);
                        continue; 
                    }
                }
                InstantiateAndAddToPool(pool, newPool, position);
            }
            //add pool to pools
            pools.Add(pool);

            poolDictionary ??= new Dictionary<string, Queue<GameObject>>();
            
            //add it in the pool dictionary
            poolDictionary.Add(newSpawnedTag, newPool);
        }

        private GameObject InstantiateAndAddToPool(Pool pool, Queue<GameObject> objectPool, Vector3 position)
        {
            //instantiating the object then setting it inactive
            var obj = Instantiate(pool.prefab, position, Quaternion.identity, masterParent);
            obj.SetActive(false);

            //put it in the new queue
            objectPool.Enqueue(obj);

            return obj;
        }

        public void BackToPool(GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.rotation = Quaternion.Euler(0,0,0);
            obj.transform.parent = masterParent;
        }
    }
}
