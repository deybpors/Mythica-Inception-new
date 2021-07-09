using System;
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
        public GameObject dashGraphic;
        public float tameRadius;
        public GameObject deathParticles;
        
        [Header("Skill Indicators")] 
        public Texture2D normalCursor;
        public GameObject areaIndicator;
        public Texture2D pointIndicator;
        public GameObject vectorIndicator;
        public GameObject unitIndicator;

        #region Hidden Fields

        [HideInInspector] public float tempSpeed;
        [HideInInspector] public float tempAttackRate;
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
        private MonsterManager _monsterManager;
        
        #endregion
        

        void Awake()
        {
            Init();
        }

        private void Init()
        {
            GetNeededComponents();
            InitializePlayerData();
            _monsterManager.ActivateMonsterManager(this, skillManager);
            tempSpeed = playerData.speed;
            tempAttackRate = playerData.attackRate;
            unitIndicator.transform.localScale = new Vector3(tameRadius, tameRadius, tameRadius);
            Cursor.SetCursor(normalCursor, Vector2.zero, CursorMode.Auto);
            _stateController.ActivateAI(true, null, this);
        }

        private void GetNeededComponents()
        {
            mainCamera = GameManager.instance.mainCamera;
            skillManager = GetComponent<SkillManager>();
            skillManager.skillSlots.Clear();
            _monsterManager = GetComponent<MonsterManager>();
            staminaComponent = GetComponent<Stamina>();
            selectionManager = GetComponent<SelectionManager>();
            selectionManager.ActivateSelectionManager(this);
            controller = GetComponent<CharacterController>();
            inputHandler = GetComponent<PlayerInputHandler>();
            inputHandler.ActivatePlayerInputHandler(this);
            tamer = transform.FindChildWithTag("Tamer").gameObject;
            currentAnimator = tamer.GetComponent<Animator>();
            _stateController = GetComponent<StateController>();
            _healthComponent = GetComponent<Health>();
        }

        public void InitializePlayerData()
        {
            //TODO: initialize the monsters from player's save data and put it in monsters list
            Debug.Log("Initializing save data...");
            
            
            //after getting all data,
            //initialize player's health
            playerHealth.maxHealth = GameCalculations.Stats(
                GameCalculations.MonstersAvgHealth(monsterSlots.ToList()),
                GameCalculations.MonstersAvgStabilityValue(monsterSlots.ToList()),
                GameCalculations.MonstersAvgLevel(monsterSlots.ToList()));
            _healthComponent.health.maxHealth = playerHealth.maxHealth;
            _healthComponent.health.currentHealth = playerHealth.currentHealth;
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

        public Monster GetCurrentMonster()
        {
            return monsterSlots[inputHandler.currentMonster].monster;
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

        public void ChangeMonsterUnitIndicatorRadius(float radius)
        {
            unitIndicator.transform.localScale = new Vector3(radius, radius, radius);
        }

        public void ReleaseBasicAttack()
        {
            var monAttacking = GetCurrentMonster();

            var range = monAttacking.basicAttackType != BasicAttackType.Melee;
            var projectile = GameManager.instance.pooler.
                SpawnFromPool(range ? null : projectileRelease.transform, monAttacking.basicAttackObjects.projectile.name,
                    monAttacking.basicAttackObjects.projectile, range ? projectileRelease.position : Vector3.zero, range ? transform.rotation : Quaternion.identity);
            
            var rangeProjectile = projectile.GetComponent<IRange>() ?? projectile.AddComponent<ProjectileMove>();
            var target = selectionManager.selectables.Count > 0 ? selectionManager.selectables[0] : null;
            var deathTime = range ? .25f : .1f;
            var speed = range ? 50f : 30f;
            
            rangeProjectile.ProjectileData(true, range,monAttacking.basicAttackObjects.targetObject,monAttacking.basicAttackObjects.impact, 
                monAttacking.basicAttackObjects.muzzle,false, true, transform, target,
                Vector3.zero, deathTime, speed,1.5f,monAttacking.basicAttackSkill);
        }

        public void SpawnSwitchFX()
        {
            GameManager.instance.pooler.
                SpawnFromPool(transform, tameBeam.projectileGraphics.targetObject.name,
                    tameBeam.projectileGraphics.targetObject, Vector3.zero, 
                    Quaternion.identity);
        }

        public void ChangeStatsToMonster(int slot)
        {
            tempSpeed = playerData.speed;
            tempAttackRate = playerData.attackRate;

            if (slot < 0)
            {
                _healthComponent.health.currentHealth = playerHealth.currentHealth;
                playerHealth.maxHealth = GameCalculations.Stats(
                    GameCalculations.MonstersAvgHealth(monsterSlots.ToList()),
                    GameCalculations.MonstersAvgStabilityValue(monsterSlots.ToList()),
                    GameCalculations.MonstersAvgLevel(monsterSlots.ToList())
                    );
                _healthComponent.health.maxHealth = playerHealth.maxHealth;
                return;
            }
            
            tempSpeed *= monsterSlots[slot].monster.stats.movementSpeed;
            tempAttackRate *= monsterSlots[slot].monster.stats.attackRate;
            //Initialize Monster's health
            _healthComponent.health.currentHealth = monsterSlots[slot].currentHealth;
            _healthComponent.health.maxHealth =
                GameCalculations.Stats(
                    monsterSlots[slot].monster.stats.baseHealth,
                    monsterSlots[slot].stabilityValue,
                    GameCalculations.Level(monsterSlots[slot].currentExp)
                    );
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
                Quaternion.identity);
            var rangeProjectile = projectile.GetComponent<IRange>() ?? projectile.AddComponent<ProjectileMove>();
            rangeProjectile.ProjectileData(true, true, tameBeam.projectileGraphics.targetObject,tameBeam.projectileGraphics.impact, 
                tameBeam.projectileGraphics.muzzle,true, false, transform, selectionManager.selectables[0], 
                Vector3.zero, 10, 50,1, tameBeam.skill);
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
                if (playerHealth.currentHealth <= 0)
                {
                    Die();
                }
                return;
            }
            monsterSlots[inputHandler.currentMonster].currentHealth = _healthComponent.health.currentHealth;
            if (monsterSlots[inputHandler.currentMonster].currentHealth > 0) return;
            
            var slotToSwitch = 9999999;
            for (int i = 0; i < monsterSlots.Length; i++)
            {
                if (monsterSlots[i].currentHealth > 0)
                {
                    slotToSwitch = i;
                }
            }

            if (slotToSwitch > monsterSlots.Length)
            {
                inputHandler.playerSwitch = true;
                _monsterManager.SwitchToTamer();
            }
            else
            {
                if (_monsterManager != null)
                {
                    inputHandler.currentMonster = slotToSwitch;
                    _monsterManager.SwitchMonster(slotToSwitch);
                }
            }
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
            playerStamina.currentStamina = staminaComponent.stamina.currentStamina;
        }

        public void AddStamina(int staminaToAdd)
        {
            staminaComponent.AddStamina(staminaToAdd);
            playerStamina.currentStamina = staminaComponent.stamina.currentStamina;
        }
    }
}
