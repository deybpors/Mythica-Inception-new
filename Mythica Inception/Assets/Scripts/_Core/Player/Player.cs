using System.Collections.Generic;
using System.Linq;
using Assets.Scripts._Core.Input;
using Assets.Scripts._Core.Managers;
using Assets.Scripts._Core.Others;
using Assets.Scripts._Core.Player.Player_FSM;
using Assets.Scripts.Combat_System;
using Assets.Scripts.Monster_System;
using Assets.Scripts.Pluggable_AI.Scripts.General;
using Assets.Scripts.Skill_System;
using Combat_System;
using UnityEngine;

namespace Assets.Scripts._Core.Player
{
    [RequireComponent(typeof(StateController))]
    public class Player : MonoBehaviour, IEntity, IHaveMonsters, IHaveHealth, ICanTame, IHaveStamina
    {

        public List<MonsterSlot> monsterSlots;
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
        [HideInInspector] public MonsterManager monsterManager;
        
        #endregion
        

        void Awake()
        {
            Init();
        }

        private void Init()
        {
            GetNeededComponents();
            InitializePlayerData();
            monsterManager.ActivateMonsterManager(this, skillManager);
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
            monsterManager = GetComponent<MonsterManager>();
            staminaComponent = GetComponent<Stamina>();
            selectionManager = GetComponent<SelectionManager>();
            selectionManager.ActivateSelectionManager(this);
            controller = GetComponent<CharacterController>();
            inputHandler = GetComponent<PlayerInputHandler>();
            inputHandler.ActivatePlayerInputHandler(this);
            _stateController = GetComponent<StateController>();
            _healthComponent = GetComponent<Health>();
        }

        public void InitializePlayerData()
        {
            //TODO: initialize the monsters from player's save data and put it in monsters list
            Debug.Log("Initializing save data...");
            
            
            
            //after getting all data,
            tamer = transform.FindChildWithTag("Tamer").gameObject;
            tamer.layer = LayerMask.NameToLayer("Player");
            currentAnimator = tamer.GetComponent<Animator>();
            //initialize player's health
            playerHealth.maxHealth = GameCalculations.Stats(
                GameCalculations.MonstersAvgHealth(monsterSlots.ToList()),
                GameCalculations.MonstersAvgStabilityValue(monsterSlots.ToList()),
                GameCalculations.MonstersAvgLevel(monsterSlots.ToList()));
            _healthComponent.UpdateHealth(playerHealth.maxHealth, playerHealth.currentHealth);
        }

        public float GetMonsterSwitchRate()
        {
            return playerData.monsterSwitchRate;
        }

        public void SwitchMonster(int slot)
        {
            if (slot < 0)
            {
                monsterManager.SwitchToTamer();
            }
            else
            {
                monsterManager.SwitchMonster(slot);
            }
        }

        public int CurrentSlotNumber()
        {
            return inputHandler.currentMonster;
        }
        
        public List<Monster> GetMonsters()
        {
            var monsters = new List<Monster>();
            if (monsterSlots.Count <= 0) return monsters;
            monsters.AddRange(monsterSlots.Select(slot => slot.monster != null ? slot.monster : null));
            return monsters;
        }

        public void AddNewMonsterSlot(int slotNum, MonsterSlot newSlot)
        {
            monsterSlots[slotNum] = newSlot;
            monsterSlots[slotNum].slotNumber = slotNum;
            monsterSlots[slotNum].inParty = true;
            monsterManager.RequestPoolMonstersPrefab();
            monsterManager.GetMonsterAnimators();
        }

        public List<MonsterSlot> GetMonsterSlots()
        {
            return monsterSlots;
        }

        public MonsterSlot GetMonsterWithHighestExp()
        {
            var mSlot = new MonsterSlot();
            foreach (var slot in monsterSlots.Where(slot => mSlot.monster == null || mSlot.currentExp >= slot.currentExp))
            {
                mSlot = slot;
            }
            return mSlot;
        }

        public Monster GetCurrentMonster()
        {
            return inputHandler.currentMonster < 0 ? null : monsterSlots[inputHandler.currentMonster].monster;
        }

        public bool IsPlayerSwitched()
        {
            return inputHandler.currentMonster < 0;
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
                    monAttacking.basicAttackObjects.projectile, range ? projectileRelease.position : Vector3.zero, range ? transform.rotation : Quaternion.Euler(-90,transform.rotation.y,transform.rotation.z));
            
            var rangeProjectile = projectile.GetComponent<IDamageDetection>() ?? projectile.AddComponent<Projectile>();
            var target = selectionManager.selectables.Count > 0 ? selectionManager.selectables[0] : null;
            var deathTime = range ? .25f : .1f;
            var speed = range ? 30f : 20f;
            
            rangeProjectile.ProjectileData(true, range,monAttacking.basicAttackObjects.targetObject,monAttacking.basicAttackObjects.impact, 
                monAttacking.basicAttackObjects.muzzle,false, true, transform, target,
                Vector3.zero, deathTime, speed,.5f,monAttacking.basicAttackSkill);
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
                playerHealth.maxHealth = GameCalculations.Stats(
                    GameCalculations.MonstersAvgHealth(monsterSlots.ToList()),
                    GameCalculations.MonstersAvgStabilityValue(monsterSlots.ToList()),
                    GameCalculations.MonstersAvgLevel(monsterSlots.ToList())
                    );
                _healthComponent.UpdateHealth(playerHealth.maxHealth, playerHealth.currentHealth);
                return;
            }
            
            tempSpeed *= monsterSlots[slot].monster.stats.movementSpeed;
            tempAttackRate *= monsterSlots[slot].monster.stats.attackRate;
            //Initialize Monster's health
            var maxHealth =
                GameCalculations.Stats(
                    monsterSlots[slot].monster.stats.baseHealth,
                    monsterSlots[slot].stabilityValue,
                    GameCalculations.Level(monsterSlots[slot].currentExp)
                    );
            _healthComponent.UpdateHealth(maxHealth, monsterSlots[slot].currentHealth);
        }


        public StateController GetStateController()
        {
            return _stateController;
        }

        public void ReleaseTameBeam()
        {
            if(inputHandler.currentMonster >= 0) return;
            if(selectionManager.selectables.Count <= 0) return;
            
            var tameable = selectionManager.selectables[0].GetComponent<ITameable>();
            if (tameable == null)
            {
                //TODO: display in UI that the target has to be a Wild monster
                return;
            }
            
            //spawn projectile
            var projectile = GameManager.instance.pooler.
                SpawnFromPool(null, tameBeam.projectileGraphics.projectile.name,
                tameBeam.projectileGraphics.projectile, projectileRelease.position,
                Quaternion.identity);
            var rangeProjectile = projectile.GetComponent<IDamageDetection>() ?? projectile.AddComponent<Projectile>();
            rangeProjectile.ProjectileData(true, true, tameBeam.projectileGraphics.targetObject,tameBeam.projectileGraphics.impact, 
                tameBeam.projectileGraphics.muzzle,true, false, transform, selectionManager.selectables[0], 
                Vector3.zero, 10, 30,1f, tameBeam.skill);
        }

        public Animator GetEntityAnimator()
        {
            return currentAnimator;
        }

        public void TakeDamage(int damageToTake)
        {
            _healthComponent.ReduceHealth(damageToTake);
            if (inputHandler.currentMonster < 0)
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
            monsterSlots[inputHandler.currentMonster].fainted = true;
            FindAliveMonsterOrPlayer();
        }

        private void FindAliveMonsterOrPlayer()
        {
            var monsterSlot = new MonsterSlot();
            foreach (var monster in monsterSlots)
            {
                if (monster.currentHealth > 0 && monster.monster != null)
                {
                    monsterSlot = monster;
                }
            }

            if (monsterSlot.monster == null)
            {
                if(inputHandler.currentMonster < 0) return;
                inputHandler.currentMonster = -1;
                monsterManager.SwitchToTamer();
            }
            else
            {
                if (monsterManager == null) return;
                inputHandler.currentMonster = monsterSlot.slotNumber;
                monsterManager.SwitchMonster(monsterSlot.slotNumber);
            }
        }

        public void Heal(int amountToHeal)
        {
            _healthComponent.AddHealth(amountToHeal);
            if (inputHandler.currentMonster < 0)
            {
                playerHealth.currentHealth = _healthComponent.health.currentHealth;
                return;
            }
            monsterSlots[inputHandler.currentMonster].currentHealth = _healthComponent.health.currentHealth;
        }

        public void Die()
        {
            Debug.Log("player dead");
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
