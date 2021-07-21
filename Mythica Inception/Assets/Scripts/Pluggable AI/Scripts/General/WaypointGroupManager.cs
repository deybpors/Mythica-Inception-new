using System.Collections.Generic;
using Assets.Scripts.Pluggable_AI.Scripts.General;
using MyBox;
using UnityEngine;

namespace Pluggable_AI.Scripts.General
{
    [System.Serializable]
    public class WaypointGroupManager : MonoBehaviour
    {
        [HideInInspector]
        public List<Transform> waypoints;
        [Tooltip("If unchecked, this will find all GameObjects with StateController Component in the Scene")]
        public bool giveSpecificStateControllers;
        [ConditionalField(nameof(giveSpecificStateControllers))] public CollectionWrapper<StateController> stateControllers;

        private StateController[] _stateControllers;
        

        void Start()
        {
            
            if (!giveSpecificStateControllers)
            {
                _stateControllers = FindObjectsOfType<StateController>();
            }
            else
            {
                _stateControllers = stateControllers.Value;
            }
            
            foreach (Transform child in transform)
                waypoints.Add(child);
            
            if (waypoints.Count == 0) Debug.LogError("You have to put Waypoint children in " + this.name + " GameObject.");
            
            if (_stateControllers.Length == 0) Debug.LogWarning("No StateController found in " + this.name + " GameObject.");
            
            if (_stateControllers.Length == 0 || waypoints.Count == 0) return;
            
            foreach (var stateController in _stateControllers)
            {
                if(stateController == null){ continue; }

                stateController.ActivateAI(stateController.gameObject.activeInHierarchy, waypoints, null);
            }
        }
    }
}
