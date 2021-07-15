using System.Collections;
using System.Collections.Generic;
using Assets.Scripts._Core;
using Assets.Scripts.Pluggable_AI.Scripts.General;
using UnityEngine;

namespace Assets.Scripts.Monster_System
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
        

        void Start()
        {
            InvokeRepeating("SpawnMonsters", 0, 1);
        }

        void SpawnMonsters()
        {
            if (!activated) return;
            
            if(currentNoOfMonsters >= noOfMonstersLimit) return;
            
            var distance = Vector3.Distance(transform.position, GameManager.instance.player.position);
            if (distance > 30) return;
            
            if(_waitSpawn != null) return;
            
            Spawn();
            currentNoOfMonsters++;
            _waitSpawn = StartCoroutine(WaitSpawnTime());
        }

        private IEnumerator WaitSpawnTime()
        {
            yield return new WaitForSeconds(Random.Range(1, randomSpawnTimeMax+1));
            _waitSpawn = null;
        }

        private void Spawn()
        {
            var monsterIndex = Random.Range(0, monsters.Count);
            var monsterXp = GameCalculations.Experience(Random.Range(lowestLevel, highestLevel));
            var highX = transform.position.x + xAxis;
            var lowX = transform.position.x - xAxis;
            var highZ = transform.position.z + zAxis;
            var lowZ = transform.position.z - zAxis;
            var monsterPos = new Vector3(Random.Range(lowX, highX), transform.position.y, Random.Range(lowZ, highZ));

            var monsterObj = GameManager.instance.pooler.SpawnFromPool(null, wildMonsterPrefab.name, wildMonsterPrefab,
                monsterPos,
                Quaternion.identity);
            var newMonSlot = new MonsterSlot(monsters[monsterIndex], monsterXp, Random.Range(1, 51));
            var monTamerAi = monsterObj.GetComponent<MonsterTamerAI>();
            monTamerAi.ActivateWildMonster(newMonSlot, waypointsList, this);
            Debug.Log("Spawned " + monsters[monsterIndex].name);
        }
    }
}
