using System;
using System.Collections;
using System.Collections.Generic;
using _Core.Managers;
using _Core.Others;
using Codice.Client.BaseCommands.BranchExplorer;
using Items_and_Barter_System.Scripts;
using MyBox;
using Pluggable_AI.Scripts.General;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Monster_System
{
    public class WildMonsterSpawner : MonoBehaviour
    {
        [Foldout("Spawner Info", true)]
        public bool activated;
        public GameObject wildMonsterPrefab;
        public List<Transform> waypointsList;
        public List<Monster> monsters;
        public float xAxis;
        public float zAxis;
        public float randomSpawnTimeMax;
        public int highestLevel;
        public int lowestLevel;
        public int noOfMonstersLimit;
        [ReadOnly] public int currentNoOfMonsters;


        [Space]
        [Foldout("Item Drops", true)]
        [SerializeField] private GameObject _itemDropPrefab;
        [SerializeField] private int _maxNumberOfDrops;
        [SerializeField] private int _goldDropsMax = 25;
        public List<ItemObject> itemDrops;

        private Coroutine _waitSpawn;
        private Vector3 _spawnerPosition;
        private Transform _thisTransform;

        private Dictionary<GameObject, NavMeshAgent> _agents = new Dictionary<GameObject, NavMeshAgent>();
        private Dictionary<GameObject, ItemDrop> _drops = new Dictionary<GameObject, ItemDrop>();
        private Dictionary<GameObject, Transform> _monsterTransforms = new Dictionary<GameObject, Transform>();

        void OnEnable()
        {
            activated = true;
            InvokeRepeating(nameof(SpawnMonsters), 0, 1);
        }

        void OnDisable()
        {
            activated = false;
        }

        void Start()
        {
            _thisTransform = transform;
            _spawnerPosition = _thisTransform.position;

            if(!activated) return;
            InvokeRepeating(nameof(SpawnMonsters), 0, 1);
        }

        private void SpawnMonsters()
        {
            if (!activated) return;
            
            if(currentNoOfMonsters >= noOfMonstersLimit) return;
            if(GameManager.instance == null) return;
            if(GameManager.instance.player == null) return;
            var distance = Vector3.Distance(_thisTransform.position, GameManager.instance.player.playerTransform.position);
            if (distance > 30) return;
            
            if(_waitSpawn != null) return;
            
            Spawn();
            currentNoOfMonsters++;
            _waitSpawn = StartCoroutine(WaitSpawnTime());
        }
        private void Spawn()
        {
            var monsterIndex = Random.Range(0, monsters.Count);
            double level = Random.Range(lowestLevel, highestLevel);
            level *= GameManager.instance.difficultyManager.GetParameterValue("Wild Lvl Multiplier");
            var monsterXp = GameSettings.Experience((int) Math.Round(level, MidpointRounding.AwayFromZero));
            var highX = _spawnerPosition.x + xAxis;
            var lowX = _spawnerPosition.x - xAxis;
            var highZ = _spawnerPosition.z + zAxis;
            var lowZ = _spawnerPosition.z - zAxis;
            var monsterPos = new Vector3(Random.Range(lowX, highX), _spawnerPosition.y, Random.Range(lowZ, highZ));

            var monsterObj = 
                GameManager.instance.pooler.SpawnFromPool(
                    null, 
                    wildMonsterPrefab.name, 
                    wildMonsterPrefab,
                    monsterPos,
                    Quaternion.identity);
            try
            {
                if (!_monsterTransforms.TryGetValue(monsterObj, out var trans))
                {
                    trans = monsterObj.transform;
                    _monsterTransforms.Add(monsterObj, trans);
                }

                GameManager.instance.activeEnemies.Add(trans, monsterObj);
            }
            catch
            {
                //ignored
            }

            if (!_agents.TryGetValue(monsterObj, out var agent))
            {
                agent = monsterObj.GetComponent<NavMeshAgent>();
                try
                {
                    _agents.Add(monsterObj, agent);
                }
                catch
                {
                    //ignored
                }
            }
 
            agent.Warp(monsterPos);
            var newMonsters = new List<MonsterSlot> { new MonsterSlot(monsters[monsterIndex], monsterXp, Random.Range(1, 51)) };
            var monTamerAi = monsterObj.GetComponent<MonsterTamerAI>();

            monTamerAi.ActivateMonsterAi(newMonsters, waypointsList, this);
        }
        public void DropItems(Vector3 position)
        {
            var numberOfDrops = Random.Range(0, _maxNumberOfDrops);
            var itemDropsCount = itemDrops.Count;


            for (var i = 0; i < numberOfDrops; i++)
            {
                var randX = Random.Range(-3, 3) + position.x;
                var randZ = Random.Range(-3, 3) + position.z;
                var itemPosition = new Vector3(randX, position.y, randZ);


                var obj = GameManager.instance.pooler.SpawnFromPool(null, _itemDropPrefab.name, _itemDropPrefab, itemPosition, Quaternion.identity);
                
                if (!_drops.TryGetValue(obj, out var drop))
                {
                    drop = obj.GetComponent<ItemDrop>();
                    _drops.Add(obj, drop);
                }

                var item = itemDrops[Random.Range(0, itemDropsCount)];
                if (item is Gold)
                {
                    drop.SetupItemDrop(position, item, Random.Range(1, _goldDropsMax));
                    continue;
                }
                
                drop.SetupItemDrop(position, item, 1);
            }
        }
        private IEnumerator WaitSpawnTime()
        {
            yield return new WaitForSeconds(Random.Range(1, randomSpawnTimeMax+1));
            _waitSpawn = null;
        }
    }
}
