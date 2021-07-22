using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Pluggable_AI.Scripts.General
{
    public class GenericAI : MonoBehaviour
    {
        public FieldOfView fieldOfView;
        public List<Transform> waypoints;
        public NavMeshAgent agent;
        public StateController stateController;
        public AIStats aiData;
        public GameObject unitIndicator;
        
        [HideInInspector] public int nextWaypoint;
        [HideInInspector] public Transform target;
        [HideInInspector] public Vector3 lastKnownTargetPosition;
        [HideInInspector] public Animator currentAnimator;
    }
}