using System.Collections.Generic;
using Assets.Scripts.Core.Player;
using Assets.Scripts.Pluggable_AI.Scripts.States;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts.Pluggable_AI.Scripts.General
{
    public enum StateMachineType
    {
        AI,
        Player
    }
    public class StateController : MonoBehaviour
    {
        public StateMachineType stateMachineType;
        public Transform machineEyes;
        
        public State currentState;
        public State remainState;

        [HideInInspector] public Player player;
        [HideInInspector] public AI aI;

        [HideInInspector] public Vector3 machineDestination;
        [HideInInspector] public bool stateBoolVariable;
        [HideInInspector] public float stateTimeElapsed;

        private bool _isActive;

        void Awake()
        {
            aI = GetComponent<AI>();
            player = GetComponent<Player>();
        }

        void Update()
        {
            if(!_isActive) return;

            currentState.UpdateState(this);
        }
        
        public void InitializeAI(bool activate, List<Transform> waypointList)
        {
            _isActive = activate;
            
            if (aI == null) return;
            aI.waypoints = waypointList;
            aI.agent.enabled = _isActive;
        }

        public void TransitionToState(State nextState)
        {
            if (nextState == remainState) return;
            
            currentState = nextState;
            OnExitState();

        }

        public bool HasTimeElapsed(float duration)
        {
            stateTimeElapsed += Time.deltaTime;
            if (stateTimeElapsed >= duration)
            {
                stateTimeElapsed = 0;
                return true;
            }

            return false;
        }

        private void OnExitState()
        {
            stateBoolVariable = false;
            stateTimeElapsed = 0;
        }

        void OnDrawGizmos()
        {
            if (currentState == null)
            {
                Debug.LogError("Current State of StateController " + this.name + " is not initialized.");
                return;
            }
            
            Gizmos.color = currentState.gizmoColor;
            Gizmos.DrawWireSphere(transform.position, 1.5f);
            if (machineDestination != Vector3.zero)
            {
                Gizmos.DrawWireSphere(machineDestination, 1.5f);
            }
        }
    }
}
