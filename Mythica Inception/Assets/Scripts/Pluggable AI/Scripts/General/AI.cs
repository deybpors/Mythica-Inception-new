using System.Collections.Generic;
using Assets.Scripts.Core;
using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Pluggable_AI.Scripts.General
{
    [RequireComponent(typeof(StateController))]
    public class AI : MonoBehaviour, IEntity, IHaveMonsters
    {
        public AIStats aiStats;
        public NavMeshAgent agent;
        public FieldOfView fieldOfView;
        public GameObject unitIndicator;
        public float monsterSwitchRate = .5f;
        [HideInInspector] public Animator animator = null;
        [HideInInspector] public List<Transform> waypoints;
        [HideInInspector] public int nextWaypoint;
        [HideInInspector] public Transform target;
        [HideInInspector] public Vector3 lastKnownTargetPosition;
        private StateController _stateController;

        //TODO: change type from gameobject to whatever the data type name of monster
        public List<GameObject> monsters;

        void Start()
        {
            _stateController = GetComponent<StateController>();
            InitializeMonsters();
            animator = monsters[0].GetComponent<Animator>();
        }

        private void InitializeMonsters()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).CompareTag("Monster"))
                {
                    monsters.Add(transform.GetChild(i).gameObject);
                }
            }
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
            return monsterSwitchRate;
        }

        public int MonsterSwitched() { return 0; }

        public List<GameObject> GetMonsters()
        {
            return monsters;
        }

    }
}