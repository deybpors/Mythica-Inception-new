using System.Collections.Generic;
using Assets.Scripts.Combat_System;
using Assets.Scripts.Core.Player.Input;
using Assets.Scripts.Core.Player.Player_FSM;
using Assets.Scripts.Monster_System;
using Assets.Scripts.Pluggable_AI.Scripts.General;
using Assets.Scripts.Skill_System;
using UnityEngine;

namespace Assets.Scripts.Core.Player
{
    [RequireComponent(typeof(StateController))]
    public class Player : MonoBehaviour, IEntity, IHaveMonsters, IHaveHealth, ICanTame
    {
        private bool _activated;
        public Camera mainCamera;
        public PlayerFSMData playerData;
        public CharacterController controller;
        public PlayerInputHandler inputHandler;
        public SkillManager skillManager;
        public float monsterSwitchRate = .5f;
        public EntitiesHealth playerHealth;
        [HideInInspector] public Health healthComponent;
        //TODO: change type with string to request from object pooler
        public GameObject projectilePrefab;
        public Transform projectileRelease;
        
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
            Init();
            _activated = true;
        }

        private void Init()
        {
            InitializeMonstersPlayerData();
            animator = monsters[0].GetComponent<Animator>();
            _stateController = GetComponent<StateController>();
            _stateController.InitializeAI(true, null);
            if(healthComponent == null){ healthComponent = GetComponent<Health>(); }
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
                inputHandler.currentMonster = inputHandler.previousMonster;
                
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

        public bool isPlayerSwitched()
        {
            return inputHandler.playerSwitch;
        }

        public void SetAnimator(Animator animatorToChange)
        {
            animator = animatorToChange;
        }

        public void Deactivate()
        {
            if(!_activated) return;
            gameObject.SetActive(false);
        }


        public StateController GetStateController()
        {
            return _stateController;
        }

        public Animator GetEntityAnimator()
        {
            return animator;
        }

        public Transform GetTarget()
        {
            Transform returnTarget = target;
            target = null;
            return returnTarget;
        }

        public void ReleaseTameBeam()
        {
            if(!inputHandler.playerSwitch) return;
            
            GameObject projectile = Instantiate(projectilePrefab, projectileRelease.position, Quaternion.identity);
            projectile.GetComponent<IRange>().SetDataForProjectile(true, false,10, transform, target, Vector3.zero, 10, 50,1);
        }

        public void TakeDamage(int damageToTake)
        {
            if (inputHandler.playerSwitch)
            {
                PlayerTakeDamage(damageToTake);
                return;
            }
            healthComponent.ReduceHealth(damageToTake, inputHandler.currentMonster);
        }

        public void Heal(int amountToHeal)
        {
            
        }

        private void PlayerTakeDamage(int damageToTake)
        {
            playerHealth.currentHealth -= damageToTake;
            if (playerHealth.currentHealth <= 0)
            {
                playerHealth.currentHealth = 0;
                //TODO: Game over command here
            }
        }
        private void PlayerHeal(int amountToHeal)
        {
            playerHealth.currentHealth += amountToHeal;
            if (playerHealth.currentHealth >= playerHealth.maxHealth)
            {
                playerHealth.currentHealth = playerHealth.maxHealth;
            }
        }

        public void AddCurrentTameValue(int tameBeamValue)
        {
            throw new System.NotImplementedException();
        }
    }
}
