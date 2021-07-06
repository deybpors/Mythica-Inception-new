using System.Collections.Generic;
using System.Linq;
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
    public class Player : MonoBehaviour, IEntity, IHaveMonsters, IHaveHealth, ICanTame, IHaveStamina
    {

        public MonsterSlot[] monsterSlots;
        public PlayerFSMData playerData;
        public EntityHealth playerHealth;
        public EntityStamina playerStamina;
        public TameBeam tameBeam;
        public Transform projectileRelease;
        
        [Header("Skill Indicators")] 
        public Texture2D normalCursor;
        public GameObject areaIndicator;
        public Texture2D pointIndicator;
        public GameObject vectorIndicator;
        public GameObject unitIndicator;

        #region Hidden Fields

        [HideInInspector] public SelectionManager selectionManager;
        [HideInInspector] public Camera mainCamera;
        [HideInInspector] public GameObject tamer;
        private Health _healthComponent;
        [HideInInspector] public SkillManager skillManager;
        [HideInInspector] public PlayerInputHandler inputHandler;
        [HideInInspector] public CharacterController controller;
        [HideInInspector] public Animator currentAnimator;
        [HideInInspector] public Stamina staminaComponent;
        private StateController _stateController;

        #endregion
        

        void Awake()
        {
            Init();
        }

        private void Init()
        {
            InitializePlayerData();
            mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
            staminaComponent = GetComponent<Stamina>();
            selectionManager = GetComponent<SelectionManager>();
            selectionManager.ActivateSelectionManager();
            controller = GetComponent<CharacterController>();
            inputHandler = GetComponent<PlayerInputHandler>();
            tamer = transform.FindChildWithTag("Tamer").gameObject;
            currentAnimator = tamer.GetComponent<Animator>();
            _stateController = GetComponent<StateController>();
            _stateController.ActivateAI(true, null);
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
            if (inputHandler.currentMonster >= monsterSlots.Length)
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
            var monsters = new List<Monster>();
            if (monsterSlots.Length <= 0) return monsters;
            monsters.AddRange(monsterSlots.Select(slot => slot.monster));
            return monsters;
        }

        public List<MonsterSlot> GetMonsterSlots()
        {
            return monsterSlots.ToList();
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


        public StateController GetStateController()
        {
            return _stateController;
        }

        public void ReleaseTameBeam()
        {
            if(!inputHandler.playerSwitch) return;
            if(selectionManager.selectables.Count <= 0) return;
            
            var projectile = GameManager.instance.pooler.
                SpawnFromPool(null, tameBeam.projectileGraphics.projectile.name,
                tameBeam.projectileGraphics.projectile, projectileRelease.position,
                Quaternion.FromToRotation(Vector3.up, Vector3.zero));
            var rangeProjectile = projectile.GetComponent<IRange>();
            if (rangeProjectile == null)
            {
                ProjectileMove projectileMove = projectile.AddComponent<ProjectileMove>();
                projectileMove.ProjectileData(true, tameBeam.projectileGraphics.impact, tameBeam.projectileGraphics.muzzle, true, false, tameBeam.power, transform, selectionManager.selectables[0], Vector3.zero, 10, 50,1);
                return;
            }
            rangeProjectile.ProjectileData(true, tameBeam.projectileGraphics.impact, tameBeam.projectileGraphics.muzzle,true, false, tameBeam.power, transform, selectionManager.selectables[0], Vector3.zero, 10, 50,1);
        }

        public Animator GetEntityAnimator()
        {
            return currentAnimator;
        }

        public void TakeDamage(int damageToTake)
        {
            _healthComponent.ReduceHealth(damageToTake);
            if (inputHandler.playerSwitch)
            {
                playerHealth.currentHealth = _healthComponent.health.currentHealth;
                return;
            }
            monsterSlots[inputHandler.currentMonster].currentHealth = _healthComponent.health.currentHealth;
        }

        public void Heal(int amountToHeal)
        {
            _healthComponent.AddHealth(amountToHeal);
            if (inputHandler.playerSwitch)
            {
                playerHealth.currentHealth = _healthComponent.health.currentHealth;
                return;
            }
            monsterSlots[inputHandler.currentMonster].currentHealth = _healthComponent.health.currentHealth;
        }

        public void Die()
        {
            if (inputHandler.playerSwitch)
            {
                //TODO: Go back to last town
            }
            else
            {
                //switch to next monster
                //if no more monster that still has health
                    //go back to last town
            }
        }

        public void TakeStamina(int staminaToTake)
        {
            staminaComponent.ReduceStamina(staminaToTake);
            if (inputHandler.playerSwitch)
            {
                playerStamina.currentStamina = staminaComponent.stamina.currentStamina;
                return;
            }
            monsterSlots[inputHandler.currentMonster].currentStamina = staminaComponent.stamina.currentStamina;
        }

        public void AddStamina(int staminaToAdd)
        {
            staminaComponent.AddStamina(staminaToAdd);
            if (inputHandler.playerSwitch)
            {
                playerStamina.currentStamina = staminaComponent.stamina.currentStamina;
                return;
            }
            monsterSlots[inputHandler.currentMonster].currentStamina = staminaComponent.stamina.currentStamina;
        }
    }
}
