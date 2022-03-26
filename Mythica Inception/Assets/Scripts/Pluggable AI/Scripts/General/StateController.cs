using System;
using System.Collections.Generic;
using _Core.Managers;
using _Core.Player;
using Pluggable_AI.Scripts.States;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Pluggable_AI.Scripts.General
{
    public enum StateMachineType
    {
        AI,
        Player,
        Manager
    }
    public class StateController : MonoBehaviour
    {
        
        public StateMachineType stateMachineType;
        public Transform machineEyes;
        
        public State currentState;
        public State remainState;

        [HideInInspector] public Player player;
        [HideInInspector] public GenericAI aI;
        public Animator controllerAnimator;
        [HideInInspector] public Vector3 machineDestination;
        [HideInInspector] public bool stateBoolVariable;
        [HideInInspector] public float stateTimeElapsed;

        public bool active;
        
        public void ActivateAI(bool activate, List<Transform> waypointList, Player player)
        {
            aI = GetComponent<GenericAI>();
            this.player = player;
            active = activate;
            stateTimeElapsed = 0;
            if (aI == null) return;
            aI.waypoints = waypointList;
            aI.agent.enabled = active;
        }

        void OnDisable()
        {
            active = false;
        }

        void Update()
        {
            if(!active) return;
            
            CheckAnimator();
            currentState.UpdateState(this);
        }

        private void CheckAnimator()
        {
            if (stateMachineType == StateMachineType.Player && controllerAnimator != player.currentAnimator)
            {
                controllerAnimator = player.currentAnimator;
            }
            else if (stateMachineType == StateMachineType.AI && controllerAnimator != aI.currentAnimator)
            {
                controllerAnimator = aI.currentAnimator;
            }

            if (controllerAnimator == null || currentState.stateAnimation.Equals(string.Empty) ||
                stateMachineType != StateMachineType.Player) return;

            if (GameManager.instance.inputHandler.currentMonster >= 0 && currentState.stateAnimation.Equals("Roll")) return;

            controllerAnimator.SetBool(currentState.stateAnimation, true);
        }

        public void TransitionToState(State nextState)
        {
            if (nextState == remainState) return;

            if (controllerAnimator != null && !currentState.stateAnimation.Equals(string.Empty))
            {
                if (stateMachineType != StateMachineType.Player || GameManager.instance.inputHandler.currentMonster < 0 ||
                    !currentState.stateAnimation.Equals("Roll"))
                {
                    controllerAnimator.SetBool(currentState.stateAnimation, false);
                }
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
