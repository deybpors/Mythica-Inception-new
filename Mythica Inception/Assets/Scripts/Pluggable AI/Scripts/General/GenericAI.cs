using System.Collections.Generic;
using MyBox;
using UnityEngine;
using UnityEngine.AI;

namespace Pluggable_AI.Scripts.General
{
    public class GenericAI : MonoBehaviour
    {
        [Foldout("Generic AI Fields", true)]
        public FieldOfView fieldOfView;
        public List<Transform> waypoints;
        public NavMeshAgent agent;
        public StateController stateController;
        public AIStats aiData;
        public GameObject unitIndicator;
        
        [HideInInspector] public int nextWaypoint;
        [HideInInspector] public Transform target;
        [HideInInspector] public Vector3 lastKnownTargetPosition;
        [ReadOnly] public Animator currentAnimator;
    }
}