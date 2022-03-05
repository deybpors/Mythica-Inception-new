using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Core.Input;
using _Core.Managers;
using _Core.Others;
using _Core.Player.Player_FSM;
using Combat_System;
using Items_and_Barter_System.Scripts;
using Monster_System;
using Pluggable_AI.Scripts.General;
using Skill_System;
using UnityEngine;
using Action = System.Action;

namespace _Core.Player
{
    [RequireComponent(typeof(StateController))]
    public class Player : MonoBehaviour, IEntity, IHaveMonsters, IHaveHealth, ICanTame
    {
        public string playerName;
        
        [Space]

        public List<MonsterSlot> monsterSlots;
        public EntityHealth playerHealth;
        public PlayerInventory inventory;

        [Space]

        [Header("Settings")]
        [SerializeField] private bool isTesting;
        [SerializeField] private GameObject deathParticles;
        public TameBeam tameBeam;
        public ProjectileRelease projectileReleases;
        public GameObject dashGraphic;
        public PlayerFSMData playerData;
        public float tameRadius;
        public TamerSexGFX tamerSexGfx;
        public GameObject unitIndicator;
        public GameObject vectorIndicator;

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
        [HideInInspector] public PlayerQuestManager playerQuestManager;
        private StateController _stateController;
        [HideInInspector] public MonsterManager monsterManager;
        private readonly Vector3 zero = Vector3.zero;
        [HideInInspector] public MonsterSlot monsterAttacker;
        [HideInInspector] public PlayerSaveData savedData;
        private Vector3 startingPosition;
        public float currentGameplayTimeScale = 1;
        
        #endregion
        

        void Awake()
        {
            Init();
        }

        private void Init()
        {
            if(GameManager.instance == null) return;
            GetNeededComponents();
            InitializePlayerSavedData();
            monsterManager.ActivateMonsterManager(this, skillManager);
            tempSpeed = playerData.speed;
            tempAttackRate = playerData.attackRate;
            unitIndicator.transform.localScale = new Vector3(tameRadius, tameRadius, tameRadius);
            _stateController.ActivateAI(true, null, this);
            GameManager.instance.uiManager.InitGameplayUI(playerName, playerHealth.currentHealth, playerHealth.maxHealth, monsterSlots);
            GameManager.instance.uiManager.loadingScreen.SetActive(false);
            GameManager.instance.uiManager.loadingScreenCamera.gameObject.SetActive(false);
        }

        private void GetNeededComponents()
        {
            mainCamera = GameManager.instance.currentWorldCamera;
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
            playerQuestManager = GetComponent<PlayerQuestManager>();
        }
        
        private void InitializePlayerSavedData()
        {
            //TODO: initialize the monsters from player's save data and put it in monsters list
            Debug.Log("Initializing save data...");
            
            if (savedData != null)
            {
                transform.position = savedData.position;
                playerName = savedData.name;
                monsterSlots = savedData.playerMonsters;
                playerHealth = savedData.playerHealth;
                inventory = savedData.inventory;
                //TODO: uncomment if there is already a choice in new game if boy or girl then erase tamer = trans... in SetPlayerSavedData then delete Luna in Unity
                //tamer = Instantiate(tamerSexGfx.GetTamerGFX(savedData.sex), transform);
            }

            SetPlayerSavedData();
        }

        private void SetPlayerSavedData()
        {
            //after getting all data,
            var monsterAvgLvl = GameCalculations.MonstersAvgLevel(monsterSlots);
            tamer = transform.FindChildWithTag("Tamer").gameObject;
            tamer.layer = LayerMask.NameToLayer("Player");
            currentAnimator = tamer.GetComponent<Animator>();
            startingPosition = transform.position;
            //initialize player's health
            playerHealth.maxHealth = GameCalculations.Stats(
                GameCalculations.MonstersAvgHealth(monsterSlots.ToList()),
                GameCalculations.MonstersAvgStabilityValue(monsterSlots.ToList()),
                monsterAvgLvl);
            _healthComponent.UpdateHealth(playerHealth.maxHealth, playerHealth.currentHealth);
            //initialize party's avg level for difficulty adjustment
            GameManager.instance.DifficultyUpdateChange("Average Party Level", monsterAvgLvl);
        }

        #region Monster
        public int GetCurrentSlotNumber()
        {
            return inputHandler.currentMonster;
        }

        public float GetMonsterSwitchRate()
        {
            return playerData.monsterSwitchRate;
        }

        public bool SwitchMonster(int currentSlot, out string message)
        {
            if (currentSlot < 0)
            {
                monsterManager.SwitchToTamer();
            }
            else
            {
                if (monsterSlots[currentSlot].fainted)
                {
                    message = "Monster selected already fainted.";
                    return false;
                }
                monsterManager.SwitchMonster(currentSlot);
            }

            message = string.Empty;
            return true;
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
            //TODO: Fan fare for taming a mythica
            GameManager.instance.uiManager.UpdatePartyUI(monsterSlots[slotNum]);
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
        public GameObject GetTamer()
        {
            return tamer;
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
        #endregion

        #region Combat

        public void ChangeMonsterUnitIndicatorRadius(float radius)
        {
            unitIndicator.transform.localScale = new Vector3(radius, radius, radius);
        }

        public void ReleaseBasicAttack()
        {
            var monAttacking = GetCurrentMonster();

            var range = monAttacking.basicAttackType != BasicAttackType.Melee;
            var rotation = transform.rotation;
            var projectile = GameManager.instance.pooler.
                SpawnFromPool(range ? null : projectileReleases.front, monAttacking.basicAttackObjects.projectile.name,
                    monAttacking.basicAttackObjects.projectile, range ? projectileReleases.front.position : zero, range ? rotation : Quaternion.Euler(-90, rotation.y, rotation.z));

            var rangeProjectile = projectile.GetComponent<IDamageDetection>() ?? projectile.AddComponent<Projectile>();
            var target = selectionManager.selectables.Count > 0 ? selectionManager.selectables[0] : null;
            var deathTime = range ? .25f : .1f;
            var speed = range ? 30f : 20f;

            rangeProjectile.ProjectileData(true, range, monAttacking.basicAttackObjects.targetObject, monAttacking.basicAttackObjects.impact,
                monAttacking.basicAttackObjects.muzzle, false, true, transform, target,
                zero, deathTime, speed, .5f, monAttacking.basicAttackSkill);
        }

        public void SpawnSwitchFX()
        {
            GameManager.instance.pooler.
                SpawnFromPool(transform, tameBeam.projectileGraphics.targetObject.name,
                    tameBeam.projectileGraphics.targetObject, zero,
                    Quaternion.identity);
        }
        public void ReleaseTameBeam()
        {
            if (inputHandler.currentMonster >= 0) return;
            if (selectionManager.selectables.Count <= 0) return;

            var tameable = selectionManager.selectables[0].GetComponent<ITameable>();
            if (tameable == null)
            {
                //TODO: display in UI that the target has to be a Wild monster
                return;
            }

            //spawn projectile
            var projectile = GameManager.instance.pooler.
                SpawnFromPool(null, tameBeam.projectileGraphics.projectile.name,
                tameBeam.projectileGraphics.projectile, projectileReleases.front.position,
                Quaternion.identity);
            var rangeProjectile = projectile.GetComponent<IDamageDetection>() ?? projectile.AddComponent<Projectile>();
            rangeProjectile.ProjectileData(true, true, tameBeam.projectileGraphics.targetObject, tameBeam.projectileGraphics.impact,
                tameBeam.projectileGraphics.muzzle, true, false, transform, selectionManager.selectables[0],
                zero, 10, 30, 1f, tameBeam.skill);
        }

        private void FindAliveMonsterOrPlayer()
        {
            var monsterSlot = new MonsterSlot();
            var count = monsterSlots.Count;
            for (var i = 0; i < count; i++)
            {
                var monster = monsterSlots[i];
                if (monster.currentHealth > 0 && monster.monster != null)
                {
                    monsterSlot = monster;
                }
            }

            if (monsterSlot.monster == null)
            {
                if (inputHandler.currentMonster < 0) return;
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

        public void AddExperience(int experienceToAdd, int slotNum)
        {
            var nextLevel = GameCalculations.Level(monsterSlots[slotNum].currentExp) + 1;
            var nextLevelExp = GameCalculations.Experience(nextLevel);
            monsterSlots[slotNum].currentExp += experienceToAdd;

            if (monsterSlots[slotNum].currentExp > nextLevelExp)
            {
                //TODO: display fanfare for level up monster
            }

            if (inputHandler.currentMonster != slotNum) return;
            GameManager.instance.uiManager.UpdateExpUI(slotNum, experienceToAdd);
        }


        private void FullyRestoreAllMonsters()
        {
            var count = monsterSlots.Count;
            for (var i = 0; i < count; i++)
            {
                if (monsterSlots[i].monster == null) continue;
                var maxHealth = GameCalculations.Stats(monsterSlots[i].monster.stats.baseHealth,
                    monsterSlots[i].stabilityValue, GameCalculations.Level(monsterSlots[i].currentExp));
                monsterSlots[i].currentHealth = maxHealth;
                monsterSlots[i].fainted = false;
                GameManager.instance.uiManager.UpdatePartyMemberHealth(i, maxHealth, maxHealth);
                for (var j = 0; j < monsterSlots[i].skillSlots.Length; j++)
                {
                    if (monsterSlots[i].skillSlots[j] == null)
                    {
                        continue;
                    }
                    monsterSlots[i].skillSlots[j].cooldownTimer = 0;
                    monsterSlots[i].skillSlots[j].skillState = SkillManager.SkillState.ready;
                }
            }
        }

        #endregion

        #region Health
        public void TakeDamage(int damageToTake)
        {
            _healthComponent.ReduceHealth(damageToTake);
            var current = inputHandler.currentMonster;
            if (current <= 0)
            {
                playerHealth.currentHealth = _healthComponent.health.currentHealth;
                GameManager.instance.uiManager.UpdateHealthUI(current, playerHealth.currentHealth);

                if (playerHealth.currentHealth <= 0)
                {
                    Die();
                }
                return;
            }
            monsterSlots[current].currentHealth = _healthComponent.health.currentHealth;
            GameManager.instance.uiManager.UpdateHealthUI(current, monsterSlots[current].currentHealth);

            if (monsterSlots[current].currentHealth > 0) return;
            monsterSlots[current].fainted = true;
            monsterSlots[current].currentLives--;
            FindAliveMonsterOrPlayer();
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

        public void RecordDamager(MonsterSlot slot)
        {
            monsterAttacker = slot;
        }

        public void Die()
        {
            Debug.Log("Player dies.");
            if (isTesting) return;

            var playerGameObject = gameObject;
            tamer.SetActive(false);
            inputHandler.movementInput = Vector2.zero;
            inputHandler.activate = false;
            GameManager.instance.pooler.SpawnFromPool(null, deathParticles.name, deathParticles, transform.position,
                Quaternion.identity);
            var layer = playerGameObject.layer;
            playerGameObject.layer = 0;
            skillManager.targeting = false;
            Action action = () =>
            {
                playerGameObject.layer = layer;
                inputHandler.activate = true;
                tamer.SetActive(true);
                playerHealth.maxHealth = GameCalculations.Stats(
                    GameCalculations.MonstersAvgHealth(monsterSlots.ToList()),
                    GameCalculations.MonstersAvgStabilityValue(monsterSlots.ToList()),
                    GameCalculations.MonstersAvgLevel(monsterSlots));
                playerHealth.currentHealth = playerHealth.maxHealth;
                _healthComponent.UpdateHealth(playerHealth.maxHealth, playerHealth.currentHealth);
                TakeDamage(0);
                FullyRestoreAllMonsters();
                GameManager.instance.uiManager.loadingScreen.SetActive(false);
            };
            GameManager.instance.uiManager.loadingScreen.SetActive(true);
            transform.position = startingPosition;
            StartCoroutine(DelayAction(3f, action));
        }


        #endregion

        #region Animation
        public void SetAnimator(Animator animatorToChange)
        {
            currentAnimator = animatorToChange;
        }

        public Animator GetEntityAnimator()
        {
            return currentAnimator;
        }

        #endregion

        #region StateController
        public StateController GetStateController()
        {
            return _stateController;
        }

        #endregion

        #region Miscellaneous

        public void ChangeTimeScaleGameplay(float scale)
        {
            Time.timeScale = scale;
            currentGameplayTimeScale = scale;
        }

        IEnumerator DelayAction(float delay, Action action)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }

        #endregion

    }
}
