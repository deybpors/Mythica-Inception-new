using System;
using System.Collections;
using System.Collections.Generic;
using _Core.Managers;
using _Core.Others;
using MyBox;
using Pluggable_AI.Scripts.General;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Monster_System
{
    public class WildMonsterSpawner : MonoBehaviour
    {
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
        private Coroutine _waitSpawn;
        private Vector3 _spawnerPosition;
        private Transform _thisTransform;

        private Dictionary<GameObject, NavMeshAgent> _agents = new Dictionary<GameObject, NavMeshAgent>();


        void Start()
        {
            _spawnerPosition = transform.position;
            InvokeRepeating(nameof(SpawnMonsters), 0, 1);
        }

        private void SpawnMonsters()
        {
            if (!activated) return;
            
            if(currentNoOfMonsters >= noOfMonstersLimit) return;
            if(GameManager.instance == null) return;
            if(GameManager.instance.player == null) return;
            if (_thisTransform == null)
            {
                _thisTransform = transform;
            }
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

        private IEnumerator WaitSpawnTime()
        {
            yield return new WaitForSeconds(Random.Range(1, randomSpawnTimeMax+1));
            _waitSpawn = null;
        }
    }
}
