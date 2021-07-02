using System.Collections.Generic;
using Assets.Scripts.Core.Player.Input;
using Assets.Scripts.Core.Player.Player_FSM;
using Assets.Scripts.Pluggable_AI.Scripts.General;
using Assets.Scripts.Skill_System;
using UnityEngine;

namespace Assets.Scripts.Core.Player
{
    [RequireComponent(typeof(StateController))]
    public class Player : MonoBehaviour,IEntity, IHaveMonsters
    {
        public Camera mainCamera;
        public PlayerFSMData playerData;
        public CharacterController controller;
        public PlayerInputHandler inputHandler;
        public SkillManager skillManager;
        public float monsterSwitchRate = .5f;
        
        //TODO: change type from gameobject to whatever the data type name of monster
        public List<GameObject> monsters;
        public Animator animator = null;
        [HideInInspector] public Transform target;
        private StateController _stateController;

        [Header("Skill Indicators")] 
        public Texture2D normalCursor;
        public GameObject areaIndicator;
        public Texture2D pointIndicator;
        public GameObject vectorIndicator;
        public GameObject unitIndicator;

        void Awake()
        {
            InitializeMonstersPlayerData();
            animator = monsters[0].GetComponent<Animator>();
        }
        void Start()
        {
            _stateController = GetComponent<StateController>();
            _stateController.InitializeAI(true, null);
            Cursor.SetCursor(normalCursor, Vector2.zero, CursorMode.Auto);
        }

        public void InitializeMonstersPlayerData()
        {
            //TODO: initialize the monsters from player's save data and put it in monsters list
        }

        public float GetMonsterSwitchRate()
        {
            return monsterSwitchRate;
        }

        public int MonsterSwitched()
        {
            if (inputHandler.currentMonster >= monsters.Count)
            {
                int monster = inputHandler.previousMonster;
                inputHandler.currentMonster = monster;
                //TODO: Update UI to send message that there is currently no monsters in the selected slot
                Debug.Log("Currently no monsters in the selected slot");
                return inputHandler.previousMonster;
            }
            return inputHandler.currentMonster;
        }

        public List<GameObject> GetMonsters()
        {
            return monsters;
        }
        public StateController GetStateController()
        {
            return _stateController;
        }

        public Animator GetEntityAnimator()
        {
            return animator;
        }

        public GameObject GetUnitIndicator()
        {
            return unitIndicator;
        }

        public Transform GetTarget()
        {
            Transform returnTarget = target;
            target = null;
            return returnTarget;
        }
    }
}
