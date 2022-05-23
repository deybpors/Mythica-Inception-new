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
        private Dictionary<GameObject, Transform> _transforms = new Dictionary<GameObject, Transform>();
        private readonly Vector3 _zero = Vector3.zero;
        private List<GameObject> _masterChildren;

        void Start()
        {
            PreInstantiate(pools);
        }

        private void PreInstantiate(List<Pool> p)
        {
            pools = p;
            if (pools.Count <= 0) return;
            
            foreach (var pool in p.ToList())
            {
                if (pool.tag == string.Empty)
                {
                    pool.tag = pool.prefab.name;
                }
                AddNewPoolToDictionary(pool.tag, pool.prefab, pool.size, _zero);
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
                    AddNewPoolToDictionary(newSpawnedTag, prefabCheck, 10, position);
                    continue;
                }

                var objectToSpawn = poolDictionary[newSpawnedTag].Dequeue();

                if (objectToSpawn == null || objectToSpawn.Equals(null))
                {
                    return null;
                }
                
                objectToSpawn.SetActive(true);

                if (!_transforms.TryGetValue(objectToSpawn, out var trans))
                {
                    trans = objectToSpawn.transform;
                    _transforms.Add(objectToSpawn, trans);
                }


                if (parent != null)
                {
                    trans.SetParent(parent);
                    trans.localPosition = position;
                    trans.rotation = Quaternion.Euler(0,0,0);
                }
                else
                {
                    trans.SetParent(masterParent);
                    trans.position = position;
                    trans.rotation = rotation;
                }

                poolDictionary[newSpawnedTag].Enqueue(objectToSpawn);

                return objectToSpawn;
            }
        }

        private void AddNewPoolToDictionary(string newSpawnedTag, GameObject prefabCheck, int size, Vector3 position)
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
            try
            {
                obj.SetActive(false);
                var objTransform = obj.transform;
                objTransform.rotation = Quaternion.Euler(0, 0, 0);

                var transformParent = objTransform.parent;
                if (transformParent.gameObject.activeInHierarchy)
                {
                    objTransform.SetParent(masterParent);
                }

                var tag = obj.name.Replace("(Clone)", string.Empty);
                poolDictionary[tag].Enqueue(obj);
            }
            catch
            {
                //ignored
            }
        }

        public void DisableAllObjects()
        {

            foreach (Transform child in masterParent)
            {
                var childGameObject = child.gameObject;
                if (childGameObject.activeInHierarchy)
                {
                    BackToPool(childGameObject);
                }
            }

            try
            {
                var queue = poolDictionary.Values.ToList();
                var queueCount = queue.Count;
                for (var i = 0; i < queueCount; i++)
                {
                    var objectsCount = queue[i].Count;
                    var list = queue[i].ToList();
                    for (var j = 0; j < objectsCount; j++)
                    {
                        BackToPool(list[i]);
                    }
                }
            }
            catch
            {
                //ignored
            }
        }
    }
}
