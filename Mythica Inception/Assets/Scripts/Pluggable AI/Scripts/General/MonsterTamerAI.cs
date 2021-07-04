using System.Collections.Generic;
using Assets.Scripts._Core;
using Assets.Scripts.Core;
using Assets.Scripts.Monster_System;
using Assets.Scripts.Skill_System;
using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Pluggable_AI.Scripts.General
{
    [RequireComponent(typeof(StateController))]
    public class MonsterTamerAI : MonoBehaviour, IEntity, IHaveMonsters, IHaveHealth
    {
        public AIStats aiStats;
        public NavMeshAgent agent;
        public FieldOfView fieldOfView;
        public GameObject unitIndicator;
        [HideInInspector] public Health health;
        [HideInInspector] public Animator currentAnimator = null;
        [HideInInspector] public List<Transform> waypoints;
        [HideInInspector] public int nextWaypoint;
        [HideInInspector] public Transform target;
        [HideInInspector] public Vector3 lastKnownTargetPosition;
        [HideInInspector] public int currentMonster;
        private StateController _stateController;
        public List<Monster> monsters;
        [HideInInspector] public List<GameObject> monsterGameObjects;
        private SkillManager _skillManager;
        private void Init()
        {
            _stateController = GetComponent<StateController>();
            
            if (monsters.Count <= 0)
            {
                //if Wild monsters
                InitializeMonstersData();
            }
            
            InitializeMonsters();
            currentAnimator = monsterGameObjects[0].GetComponent<Animator>();
            health = GetComponent<Health>();
            if (health == null)
            {
                Debug.LogWarning("Added new Health component to " + this.name + ". Please setup this first.");
                health = gameObject.AddComponent<Health>();
            }
        }

        private void InitializeMonsters()
        {
            if (monsters.Count > 0) return;
            
            for (int i = 0; i < monsters.Count; i++)
            {
                GameObject monsterObj = GameManager.instance.pooler.SpawnFromPool(transform, monsters[i].monsterName,
                    monsters[i].monsterPrefab, Vector3.zero, Quaternion.identity);
                monsterGameObjects.Add(monsterObj);
            }
        }

        private void InitializeMonstersData()
        {
            //TODO: initialize monster's data here (put Monsters in the monsters list)
        }

        public StateController GetStateController()
        {
            return _stateController;
        }

        public Animator GetEntityAnimator()
        {
            return currentAnimator;
        }

        public float GetMonsterSwitchRate()
        {
            return .5f;
        }

        public int MonsterSwitched() { return currentMonster; }

        public List<Monster> GetMonsters()
        {
            return monsters;
        }

        public bool isPlayerSwitched()
        {
            return false;
        }

        public void SetAnimator(Animator animatorToChange)
        {
            currentAnimator = animatorToChange;
        }

        public GameObject GetTamer()
        {
            return null;
        }

        public void ActivateSkillManager(bool isActivated)
        {
            if (_skillManager == null)
            {
                _skillManager = GetComponent<SkillManager>();
            }

            _skillManager.enabled = isActivated;
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        public void TakeDamage(int damageToTake)
        {
            
        }

        public void Heal(int amountToHeal)
        {
            
        }
    }
}