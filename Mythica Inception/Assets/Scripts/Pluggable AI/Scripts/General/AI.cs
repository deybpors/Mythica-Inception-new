using System.Collections.Generic;
using Assets.Scripts.Core;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Pluggable_AI.Scripts.General
{
    [RequireComponent(typeof(StateController))]
    public class AI : MonoBehaviour, IEntity
    {
        public AIStats aiStats;
        public NavMeshAgent agent;
        public FieldOfView fieldOfView;
        [HideInInspector] public List<Transform> waypoints;
        [HideInInspector] public int nextWaypoint;
        [HideInInspector] public Transform target;
        [HideInInspector] public Vector3 lastKnownTargetPosition;
        private StateController _stateController;

        void Start()
        {
            _stateController = GetComponent<StateController>();
        }
        public StateController GetStateController()
        {
            return _stateController;
        }
    }
}