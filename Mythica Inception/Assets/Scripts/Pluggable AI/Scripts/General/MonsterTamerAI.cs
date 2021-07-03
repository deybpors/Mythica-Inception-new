using System.Collections.Generic;
using Assets.Scripts.Core;
using Assets.Scripts.Monster_System;
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
        [HideInInspector] public Animator animator = null;
        [HideInInspector] public List<Transform> waypoints;
        [HideInInspector] public int nextWaypoint;
        [HideInInspector] public Transform target;
        [HideInInspector] public Vector3 lastKnownTargetPosition;
        [HideInInspector] public int currentMonster;
        private StateController _stateController;

        //TODO: change type from gameobject to whatever the data type name of monster
        public List<GameObject> monsters;

        void Start()
        {
            Init();
        }

        void OnEnable()
        {
            Init();
        }

        private void Init()
        {
            _stateController = GetComponent<StateController>();
            InitializeMonsters();
            InitializeMonstersData();
            animator = monsters[0].GetComponent<Animator>();
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
            
            //TODO: spawn monster prefab from pool here instead of finding the tag that has monster in it
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).CompareTag("Monster"))
                {
                    monsters.Add(transform.GetChild(i).gameObject);
                }
            }
        }

        private void InitializeMonstersData()
        {
            //TODO: initialize monster's data here
        }

        public StateController GetStateController()
        {
            return _stateController;
        }

        public Animator GetEntityAnimator()
        {
            return animator;
        }

        public float GetMonsterSwitchRate()
        {
            return .5f;
        }

        public int MonsterSwitched() { return currentMonster; }

        public List<GameObject> GetMonsters()
        {
            return monsters;
        }

        public bool isPlayerSwitched()
        {
            return false;
        }

        public void SetAnimator(Animator animatorToChange)
        {
            animator = animatorToChange;
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