using System.Collections.Generic;
using Assets.Scripts._Core.Input;
using Assets.Scripts._Core.Player.Player_FSM;
using Assets.Scripts.Combat_System;
using Assets.Scripts.Monster_System;
using Assets.Scripts.Pluggable_AI.Scripts.General;
using Assets.Scripts.Skill_System;
using UnityEngine;

namespace Assets.Scripts._Core.Player
{
    [RequireComponent(typeof(StateController))]
    public class Player : MonoBehaviour, IEntity, IHaveMonsters, IHaveHealth, ICanTame
    {
        private bool _activated;
        
        public List<Monster> monsters;
        public PlayerFSMData playerData;
        public EntitiesHealth playerHealth;
        public TameBeam tameBeam;
        public Transform projectileRelease;

        [Header("Skill Indicators")] 
        public Texture2D normalCursor;
        public GameObject areaIndicator;
        public Texture2D pointIndicator;
        public GameObject vectorIndicator;
        public GameObject unitIndicator;

        #region Hidden Fields
        
        [HideInInspector] public Camera mainCamera;
        [HideInInspector] public GameObject tamer;
        private Health _healthComponent;
        [HideInInspector] public SkillManager skillManager;
        [HideInInspector] public PlayerInputHandler inputHandler;
        [HideInInspector] public CharacterController controller;
        [HideInInspector] public Animator currentAnimator;
        [HideInInspector] public Transform target;
        private StateController _stateController;

        #endregion
        

        void Awake()
        {
            Init();
            _activated = true;
        }

        private void Init()
        {
            InitializePlayerData();
            mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
            controller = GetComponent<CharacterController>();
            inputHandler = GetComponent<PlayerInputHandler>();
            tamer = transform.FindChildWithTag("Tamer").gameObject;
            currentAnimator = tamer.GetComponent<Animator>();
            _stateController = GetComponent<StateController>();
            _stateController.InitializeAI(true, null);
            if(_healthComponent == null){ _healthComponent = GetComponent<Health>(); }
            Cursor.SetCursor(normalCursor, Vector2.zero, CursorMode.Auto);
            GetComponent<MonsterManager>().ActivateMonsterManager();
        }
        public void InitializePlayerData()
        {
            //TODO: initialize the monsters from player's save data and put it in monsters list
            Debug.Log("Initializing save data...");
        }

        public float GetMonsterSwitchRate()
        {
            return playerData.monsterSwitchRate;
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
        

        public List<Monster> GetMonsters()
        {
            return monsters;
        }

        public bool isPlayerSwitched()
        {
            return inputHandler.playerSwitch;
        }

        public void SetAnimator(Animator animatorToChange)
        {
            currentAnimator = animatorToChange;
        }

        public GameObject GetTamer()
        {
            return tamer;
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
            return currentAnimator;
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
            GameObject projectile = GameManager.instance.pooler.
                SpawnFromPool(null, tameBeam.projectileGraphics.projectile.name,
                tameBeam.projectileGraphics.projectile, projectileRelease.position,
                Quaternion.FromToRotation(Vector3.up, Vector3.zero));
            IRange rangeProjectile = projectile.GetComponent<IRange>();
            if (rangeProjectile == null)
            {
                ProjectileMove projectileMove = projectile.AddComponent<ProjectileMove>();
                projectileMove.ProjectileData(tameBeam.projectileGraphics.impact, tameBeam.projectileGraphics.muzzle, true, false, tameBeam.power, transform, target, Vector3.zero, 10, 50,1);
                return;
            }
            rangeProjectile.ProjectileData(tameBeam.projectileGraphics.impact, tameBeam.projectileGraphics.muzzle,true, false, tameBeam.power, transform, target, Vector3.zero, 10, 50,1);
        }

        public void TakeDamage(int damageToTake)
        {
            if (inputHandler.playerSwitch)
            {
                PlayerTakeDamage(damageToTake);
                return;
            }
            _healthComponent.ReduceHealth(damageToTake, inputHandler.currentMonster);
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
            
        }
    }
}
