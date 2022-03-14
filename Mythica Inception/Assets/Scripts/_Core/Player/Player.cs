using System;
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
        public PlayerInventory playerInventory;

        [Space]

        [Header("Settings")]
        [SerializeField] private bool isTesting;
        [SerializeField] private GameObject deathParticles;
        [SerializeField] private GameObject male;
        [SerializeField] private GameObject female;
        public TameBeam tameBeam;
        public ProjectileRelease projectileReleases;
        public GameObject dashGraphic;
        public PlayerFSMData playerData;
        public float tameRadius;
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
        [HideInInspector] public PlayerQuestManager playerQuestManager;
        private StateController _stateController;
        [HideInInspector] public MonsterManager monsterManager;
        [HideInInspector] public MonsterSlot monsterAttacker;
        [HideInInspector] public PlayerSaveData savedData;
        private readonly Vector3 _zeroVector = Vector3.zero;

        #endregion
        

        void Awake()
        {
            savedData = GameManager.instance.loadedSaveData;
            Init();
            TransferPlayerPositionRotation(savedData.playerWorldPlacement);
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
            playerName = savedData.name;
            monsterSlots = savedData.playerMonsters;
            playerHealth = savedData.playerHealth;
            playerInventory.inventorySlots = savedData.inventorySlots;
            if (savedData.sex.Equals(Sex.Male))
            {
                male.gameObject.SetActive(true);
                tamer = male;
            }
            else
            {
                female.gameObject.SetActive(true);
                tamer = female;
            }
            SetPlayerSavedData();
        }

        private void SetPlayerSavedData()
        {
            //after getting all data,
            var monsterAvgLvl = GameSettings.MonstersAvgLevel(monsterSlots);
            tamer.layer = LayerMask.NameToLayer("Player");
            currentAnimator = tamer.GetComponent<Animator>();
            
            //initialize player's health
            if (GameSettings.MonstersAvgHealth(monsterSlots.ToList()) <= 0)
            {
                playerHealth.maxHealth = GameManager.instance.saveManager.defaultPlayerHealth;
            }
            else
            {
                playerHealth.maxHealth = GameSettings.Stats(
                    GameSettings.MonstersAvgHealth(monsterSlots.ToList()),
                    GameSettings.MonstersAvgStabilityValue(monsterSlots.ToList()),
                    monsterAvgLvl);
            }
            
            _healthComponent.UpdateHealth(playerHealth.maxHealth, playerHealth.currentHealth);
            //initialize party's avg level for difficulty adjustment
            GameManager.instance.DifficultyUpdateChange("Average Party Level", monsterAvgLvl);
        }

        public PlayerSaveData GetCurrentSaveData()
        {
            savedData = new PlayerSaveData(savedData.name, savedData.sex, new WorldPlacementData(transform.position, transform.rotation, transform.localScale), monsterSlots, playerHealth,
                playerInventory.inventorySlots, GameManager.instance.currentWorldScenePath, savedData.lastOpened,
                DateTime.Now);
            return savedData;
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
                playerHealth.maxHealth = GameSettings.Stats(
                    GameSettings.MonstersAvgHealth(monsterSlots.ToList()),
                    GameSettings.MonstersAvgStabilityValue(monsterSlots.ToList()),
                    GameSettings.MonstersAvgLevel(monsterSlots.ToList())
                );
                _healthComponent.UpdateHealth(playerHealth.maxHealth, playerHealth.currentHealth);
                return;
            }

            tempSpeed *= monsterSlots[slot].monster.stats.movementSpeed;
            tempAttackRate *= monsterSlots[slot].monster.stats.attackRate;

            //Initialize Monster's health
            var maxHealth =
                GameSettings.Stats(
                    monsterSlots[slot].monster.stats.baseHealth,
                    monsterSlots[slot].stabilityValue,
                    GameSettings.Level(monsterSlots[slot].currentExp)
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
                    monAttacking.basicAttackObjects.projectile, range ? projectileReleases.front.position : _zeroVector, range ? rotation : Quaternion.Euler(-90, rotation.y, rotation.z));

            var rangeProjectile = projectile.GetComponent<IDamageDetection>() ?? projectile.AddComponent<Projectile>();
            var target = selectionManager.selectables.Count > 0 ? selectionManager.selectables[0] : null;
            var deathTime = range ? .25f : .1f;
            var speed = range ? 30f : 20f;

            rangeProjectile.ProjectileData(true, range, monAttacking.basicAttackObjects.targetObject, monAttacking.basicAttackObjects.impact,
                monAttacking.basicAttackObjects.muzzle, false, true, transform, target,
                _zeroVector, deathTime, speed, .5f, monAttacking.basicAttackSkill);
        }

        public void SpawnSwitchFX()
        {
            GameManager.instance.pooler.
                SpawnFromPool(transform, tameBeam.projectileGraphics.targetObject.name,
                    tameBeam.projectileGraphics.targetObject, _zeroVector,
                    Quaternion.identity);
        }
        public void ReleaseTameBeam()
        {
            if (inputHandler.currentMonster >= 0) return;
            if (selectionManager.selectables.Count <= 0) return;

            var tameable = selectionManager.selectables[0].GetComponent<ITameable>();
            if (tameable == null)
            {
                Debug.Log("Target has to be a wild mythica.");
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
                _zeroVector, 10, 30, 1f, tameBeam.skill);
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
            var nextLevel = GameSettings.Level(monsterSlots[slotNum].currentExp) + 1;
            var nextLevelExp = GameSettings.Experience(nextLevel);
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
                var maxHealth = GameSettings.Stats(monsterSlots[i].monster.stats.baseHealth,
                    monsterSlots[i].stabilityValue, GameSettings.Level(monsterSlots[i].currentExp));
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

            Action ResetPlayerData = () =>
            {
                playerGameObject.layer = layer;
                inputHandler.activate = true;
                tamer.SetActive(true);

                if (GameSettings.MonstersAvgHealth(monsterSlots.ToList()) <= 0)
                {
                    playerHealth.maxHealth = GameManager.instance.saveManager.defaultPlayerHealth;
                }
                else
                {
                    playerHealth.maxHealth = GameSettings.Stats(
                        GameSettings.MonstersAvgHealth(monsterSlots.ToList()),
                        GameSettings.MonstersAvgStabilityValue(monsterSlots.ToList()),
                        GameSettings.MonstersAvgLevel(monsterSlots));
                }

                playerHealth.currentHealth = playerHealth.maxHealth;

                _healthComponent.UpdateHealth(playerHealth.maxHealth, playerHealth.currentHealth);
                TakeDamage(0);
                FullyRestoreAllMonsters();
                GameManager.instance.uiManager.loadingScreen.SetActive(false);
            };

            GameManager.instance.uiManager.loadingScreen.SetActive(true);
            TransferPlayerPositionRotation(savedData.playerWorldPlacement);
            StartCoroutine(DelayAction(3f, ResetPlayerData));
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

        IEnumerator DelayAction(float delay, Action action)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }
        private void TransferPlayerPositionRotation(WorldPlacementData placementData)
        {
            if(placementData == null) return;

            transform.SetPositionAndRotation(placementData.position, placementData.rotation);
        }
        public bool SamePosition()
        {
            try
            {
                return savedData.playerWorldPlacement.position == transform.position;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        
    }
}
