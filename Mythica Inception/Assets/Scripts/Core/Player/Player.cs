using Assets.Scripts.Core.Player.Input;
using Assets.Scripts.Core.Player_FSM;
using Assets.Scripts.Pluggable_AI.Scripts.General;
using Assets.Scripts.Skill_System;
using UnityEngine;

namespace Assets.Scripts.Core.Player
{
    [RequireComponent(typeof(StateController))]
    public class Player : MonoBehaviour,IEntity
    {
        public Camera mainCamera;
        public PlayerFSMData playerData;
        public CharacterController controller;
        public PlayerInputHandler inputHandler;
        public SkillManager skillManager;
        public Transform target;
        private StateController _stateController;

        [Header("Skill Indicators")] 
        public Texture2D normalCursor;
        public GameObject areaIndicator;
        public Texture2D pointIndicator;
        public GameObject vectorIndicator;
        void Start()
        {
            _stateController = GetComponent<StateController>();
            _stateController.InitializeAI(true, null);
            Cursor.SetCursor(normalCursor, Vector2.zero, CursorMode.Auto);
        }

        public StateController GetStateController()
        {
            return _stateController;
        }

        public Transform GetTarget()
        {
            Transform returnTarget = target;
            target = null;
            return returnTarget;
        }
    }
}
