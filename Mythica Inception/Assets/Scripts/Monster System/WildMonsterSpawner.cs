using System.Collections;
using System.Collections.Generic;
using _Core.Managers;
using Assets.Scripts._Core.Others;
using Assets.Scripts.Monster_System;
using Assets.Scripts.Pluggable_AI.Scripts.General;
using UnityEngine;

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
        public int currentNoOfMonsters;
        private Coroutine _waitSpawn;
        private Vector3 _spawnerPosition;
        

        void Start()
        {
            _spawnerPosition = transform.position;
            InvokeRepeating(nameof(SpawnMonsters), 0, 1);
        }

        private void SpawnMonsters()
        {
            if (!activated) return;
            
            if(currentNoOfMonsters >= noOfMonstersLimit) return;
            
            if(GameManager.instance.player == null) return;
            var distance = Vector3.Distance(transform.position, GameManager.instance.player.transform.position);
            if (distance > 30) return;
            
            if(_waitSpawn != null) return;
            
            Spawn();
            currentNoOfMonsters++;
            _waitSpawn = StartCoroutine(WaitSpawnTime());
        }
        private void Spawn()
        {
            var monsterIndex = Random.Range(0, monsters.Count);
            var monsterXp = GameCalculations.Experience(Random.Range(lowestLevel, highestLevel));
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
            var newMonSlot = new MonsterSlot(monsters[monsterIndex], monsterXp, Random.Range(1, 51));
            var monTamerAi = monsterObj.GetComponent<MonsterTamerAI>();
            monTamerAi.ActivateWildMonster(newMonSlot, waypointsList, this);
        }

        private IEnumerator WaitSpawnTime()
        {
            yield return new WaitForSeconds(Random.Range(1, randomSpawnTimeMax+1));
            _waitSpawn = null;
        }
    }
}
