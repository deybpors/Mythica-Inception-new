using System.Collections.Generic;
using Assets.Scripts._Core.Player;
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
        //TODO: update this so suitable for object pooling
        
        public StateMachineType stateMachineType;
        public Transform machineEyes;
        
        public State currentState;
        public State remainState;

        [HideInInspector] public Player player;
        [HideInInspector] public MonsterTamerAI aI;
        [HideInInspector] public Animator controllerAnimator;

        [HideInInspector] public Vector3 machineDestination;
        [HideInInspector] public bool stateBoolVariable;
        [HideInInspector] public float stateTimeElapsed;

        public bool isActive;
        
        public void ActivateAI(bool activate, List<Transform> waypointList)
        {
            aI = GetComponent<MonsterTamerAI>();
            player = GetComponent<Player>();
            isActive = activate;
            
            if (aI == null) return;
            aI.waypoints = waypointList;
            aI.agent.enabled = isActive;
        }

        void OnDisable()
        {
            isActive = false;
        }

        void Update()
        {
            if(!isActive) return;
            
            if (stateMachineType == StateMachineType.Player && controllerAnimator != player.currentAnimator)
            {
                controllerAnimator = player.currentAnimator;
            }
            else if (stateMachineType == StateMachineType.AI && controllerAnimator != aI.currentAnimator)
            {
                controllerAnimator = aI.currentAnimator;
            }
            
            if (controllerAnimator != null && !currentState.stateAnimation.Equals(""))
            {
                controllerAnimator.SetBool(currentState.stateAnimation, true);
            }
            currentState.UpdateState(this);
        }

        public void TransitionToState(State nextState)
        {
            if (nextState == remainState) return;
            
            if (controllerAnimator != null && !currentState.stateAnimation.Equals(""))
            {
                controllerAnimator.SetBool(currentState.stateAnimation, false);
            }
            
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
