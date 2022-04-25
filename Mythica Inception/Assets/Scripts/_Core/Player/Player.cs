using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Core.Input;
using _Core.Managers;
using _Core.Others;
using Assets.Scripts._Core.Player;
using Cinemachine;
using Combat_System;
using Items_and_Barter_System.Scripts;
using Monster_System;
using MyBox;
using Pluggable_AI.Scripts.General;
using Skill_System;
using SoundSystem;
using UnityEngine;
using UnityEngine.Events;

namespace _Core.Player
{
    [RequireComponent(typeof(StateController))]
    public class Player : MonoBehaviour, IHaveMonsters, IHaveHealth, ICanTame
    {
        [Foldout("Data", true)]
        [ReadOnly] public string playerName;
         public List<MonsterSlot> monsterSlots;
        [ReadOnly] public EntityHealth playerHealth;
        [ReadOnly] public PlayerInventory playerInventory;
        public Dictionary<string, Monster> discoveredMonsters = new Dictionary<string, Monster>();

        [Space]

        [Foldout("Settings", true)]
        [DisplayInspector] public PlayerSettings playerSettings;
        [ReadOnly] public ProjectileRelease projectileReleases;
        [ReadOnly] public GameObject unitIndicator;
        [ReadOnly] public GameObject vectorIndicator;
        [SerializeField] private Light _gamePlayLight;
        [InitializationField] public CinemachineVirtualCamera virtualCamera;
        [InitializationField] public GameObject expOrbReceiveFx;
        [InitializationField] public GameObject lvlUpFx;

        #region Hidden Fields

        [HideInInspector] public float tempSpeed;
        [HideInInspector] public float tempAttackRate;
        [HideInInspector] public SelectionManager selectionManager;
        [HideInInspector] public Camera mainCamera;
        [HideInInspector] public GameObject tamer;
        [HideInInspector] public Health healthComponent;
        [HideInInspector] public SkillManager skillManager;
        [HideInInspector] private PlayerInputHandler _inputHandler;
        [HideInInspector] public Rigidbody rgdbody;
        [HideInInspector] public Animator currentAnimator;
        [HideInInspector] public PlayerQuestManager playerQuestManager;
        private StateController _stateController;
        [HideInInspector] public MonsterManager monsterManager;
        [HideInInspector] public MonsterSlot monsterAttacker;
        [HideInInspector] public PlayerSaveData savedData;
        [HideInInspector] public Vector3 colliderExtents;
        [HideInInspector] public List<MonsterSlot> storageMonsters;
        [HideInInspector] public Transform playerTransform;
        private readonly Vector3 _zeroVector = Vector3.zero;
        private DateTime _dateOpened;
        [ReadOnly] [SerializeField] private bool _tamerInvulnerable = false;
        private readonly Color _white = Color.white;

        #endregion


        void Awake()
        {
            Destroy(GameManager.instance.GetComponent<AudioListener>());
            savedData = GameManager.instance.loadedSaveData;
            GameManager.instance.uiManager.generalOptionsUi.ChangeUIValues();
            Init();
            if(savedData == null) return;
            TransferPlayerPositionRotation(savedData.playerWorldPlacement);
            GameManager.instance.mainLight = _gamePlayLight;
            GameManager.instance.uiManager.UpdateGoldUI();
        }

        #region Initialization

        private void Init()
        {
            if (GameManager.instance == null) return;
            playerTransform = transform;
            GetNeededComponents();
            InitializePlayerSavedData();
            monsterManager.ActivateMonsterManager(this, skillManager);
            tempSpeed = playerSettings.playerData.speed;
            tempAttackRate = playerSettings.playerData.attackRate;
            unitIndicator.transform.localScale = new Vector3(playerSettings.tameRadius, playerSettings.tameRadius, playerSettings.tameRadius);
            _stateController.ActivateAI(true, null, this);
            GameManager.instance.uiManager.InitGameplayUI(playerName, playerHealth.currentHealth, playerHealth.maxHealth, monsterSlots);
            GameManager.instance.uiManager.loadingScreen.gameObject.SetActive(false);
        }
        private void GetNeededComponents()
        {
            mainCamera = GameManager.instance.currentWorldCamera;
            skillManager = GetComponent<SkillManager>();
            skillManager.skillSlots.Clear();
            monsterManager = GetComponent<MonsterManager>();
            selectionManager = GetComponent<SelectionManager>();
            selectionManager.ActivateSelectionManager(this);
            _inputHandler = GameManager.instance.inputHandler;
            rgdbody = GetComponent<Rigidbody>();
            _inputHandler.ActivatePlayerInputHandler(this, mainCamera);
            _inputHandler.EnterGameplay();
            _stateController = GetComponent<StateController>();
            healthComponent = GetComponent<Health>();
            playerQuestManager = GetComponent<PlayerQuestManager>();
            colliderExtents = GetComponent<Collider>().bounds.extents;
        }
        private void InitializePlayerSavedData()
        {
            if (savedData == null) return;

            playerName = savedData.name;
            monsterSlots = savedData.playerMonsters;
            playerHealth = savedData.playerHealth;
            playerInventory.inventorySlots = savedData.inventorySlots;
            playerInventory.UpdateTotalInventory();
            playerQuestManager.activeQuests = savedData.activeQuests;
            GameManager.instance.uiManager.questUI.UpdateQuestIcons();
            playerQuestManager.finishedQuests = savedData.finishedQuests;
            discoveredMonsters = savedData.discoveredMonsters;
            storageMonsters = savedData.storageMonsters ?? GameSettings.GetDefaultMonsterSlots(30);
            var playerGFX = Instantiate(savedData.sex.Equals(Sex.Male) ? playerSettings.male : playerSettings.female, playerTransform);
            tamer = playerGFX;
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

            healthComponent.UpdateHealth(playerHealth.maxHealth, playerHealth.currentHealth);
            //TODO: Update DDA Here

            _dateOpened = DateTime.Now;
        }
        public PlayerSaveData GetCurrentSaveData()
        {
            try
            {
                savedData = new PlayerSaveData(savedData.name,
                    savedData.sex,
                    new WorldPlacementData(playerTransform.position + playerSettings.savePositionOffset, playerTransform.rotation,
                        playerTransform.localScale),
                    monsterSlots,
                    playerHealth,
                    playerInventory.inventorySlots,
                    GameManager.instance.currentWorldScenePath,
                    discoveredMonsters,
                    playerQuestManager.activeQuests,
                    playerQuestManager.finishedQuests,
                    savedData.timeSpent + (DateTime.Now - _dateOpened),
                    DateTime.Now,
                    GameManager.instance.uiManager.generalOptionsUi.GetCurrentOptionsData(),
                    storageMonsters
                    );
            }
            catch
            {
                //ignored
            }
            
            return savedData;
        }

        #endregion

        #region Monster

        public int GetCurrentSlotNumber()
        {
            return _inputHandler.currentMonster;
        }

        public float GetMonsterSwitchRate()
        {
            return playerSettings.playerData.monsterSwitchRate;
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

        public void AddNewMonsterSlotToParty(int slotNum, MonsterSlot newSlot)
        {
            monsterSlots[slotNum] = newSlot;
            monsterSlots[slotNum].slotNumber = slotNum;
            monsterSlots[slotNum].inParty = true;
            monsterManager.RequestPoolMonstersPrefab();
            monsterManager.GetMonsterAnimators();
            GameManager.instance.uiManager.UpdatePartyUI(monsterSlots[slotNum]);
        }

        public void AddNewMonsterSlotToStorage(MonsterSlot newSlot, out int slotNum)
        {
            slotNum = 0;
            var storageMonstersCount = storageMonsters.Count;
            for (var i = 0; i < storageMonstersCount; i++)
            {
                if (storageMonsters[i].monster != null) continue;
                
                slotNum = i;
                break;
            }

            storageMonsters[slotNum] = newSlot;
            storageMonsters[slotNum].slotNumber = slotNum;
            storageMonsters[slotNum].inParty = false;
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
            return _inputHandler.currentMonster < 0 ? null : monsterSlots[_inputHandler.currentMonster].monster;
        }

        public GameObject GetTamer()
        {
            return tamer;
        }

        public void ChangeStatsToMonster(int slot)
        {
            tempSpeed = playerSettings.playerData.speed;
            tempAttackRate = playerSettings.playerData.attackRate;

            if (slot < 0)
            {
                playerHealth.maxHealth = GameSettings.Stats(
                    GameSettings.MonstersAvgHealth(monsterSlots.ToList()),
                    GameSettings.MonstersAvgStabilityValue(monsterSlots.ToList()),
                    GameSettings.MonstersAvgLevel(monsterSlots.ToList())
                );
                healthComponent.UpdateHealth(playerHealth.maxHealth, playerHealth.currentHealth);
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
            healthComponent.UpdateHealth(maxHealth, monsterSlots[slot].currentHealth);
        }

        public void AddToDiscoveredMonsters(Monster monster)
        {
            try
            {
                discoveredMonsters.Add(monster.ID, monster);
            }
            catch
            {
                //ignored
            }
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
            var rotation = playerTransform.rotation;
            var projectile = GameManager.instance.pooler.
                SpawnFromPool(range ? null : projectileReleases.front, monAttacking.basicAttackObjects.projectile.name,
                    monAttacking.basicAttackObjects.projectile, range ? projectileReleases.front.position : _zeroVector, range ? rotation : Quaternion.Euler(-90, rotation.y, rotation.z));

            var rangeProjectile = projectile.GetComponent<IDamageDetection>() ?? projectile.AddComponent<Projectile>();
            var target = selectionManager.selectables.Count > 0 ? selectionManager.selectables[0] : null;
            var deathTime = .25f;
            var speed = range ? 30f : 20f;

            rangeProjectile.ProjectileData(true, range, monAttacking.basicAttackObjects.targetObject, monAttacking.basicAttackObjects.impact,
                monAttacking.basicAttackObjects.muzzle, false, true, playerTransform, target,
                _zeroVector, deathTime, speed, .5f, monAttacking.basicAttackSkill);
        }

        public void SpawnSwitchFX()
        {
            GameManager.instance.pooler.
                SpawnFromPool(playerTransform, playerSettings.tameBeam.projectileGraphics.targetObject.name,
                    playerSettings.tameBeam.projectileGraphics.targetObject, _zeroVector,
                    Quaternion.identity);
        }

        public void ReleaseTameBeam()
        {
            if (_inputHandler.currentMonster >= 0) return;
            if (selectionManager.selectables.Count <= 0) return;

            var tameable = selectionManager.selectables[0].GetComponent<ITameable>();
            if (tameable == null)
            {
                Debug.Log("Target has to be a wild mythica.");
                return;
            }

            //spawn projectile
            var projectile = GameManager.instance.pooler.
                SpawnFromPool(null, playerSettings.tameBeam.projectileGraphics.projectile.name,
                playerSettings.tameBeam.projectileGraphics.projectile, projectileReleases.front.position,
                Quaternion.identity);
            var rangeProjectile = projectile.GetComponent<IDamageDetection>() ?? projectile.AddComponent<Projectile>();
            rangeProjectile.ProjectileData(true, true, playerSettings.tameBeam.projectileGraphics.targetObject, playerSettings.tameBeam.projectileGraphics.impact,
                playerSettings.tameBeam.projectileGraphics.muzzle, true, false, playerTransform, selectionManager.selectables[0],
                _zeroVector, 10, 30, 1f, playerSettings.tameBeam.skill);
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
                if (_inputHandler.currentMonster < 0) return;
                _inputHandler.currentMonster = -1;
                monsterManager.SwitchToTamer();
            }
            else
            {
                if (monsterManager == null) return;
                _inputHandler.currentMonster = monsterSlot.slotNumber;
                monsterManager.SwitchMonster(monsterSlot.slotNumber);
            }
        }

        public void AddExperience(int experienceToAdd, int slotNum)
        {
            GameManager.instance.audioManager.PlaySFX("Experience Orb");
            expOrbReceiveFx.SetActive(true);
            var nextLevel = GameSettings.Level(monsterSlots[slotNum].currentExp) + 1;
            var nextLevelExp = GameSettings.Experience(nextLevel);
            monsterSlots[slotNum].currentExp += experienceToAdd;

            if (monsterSlots[slotNum].currentExp > nextLevelExp)
            {
                GameManager.instance.audioManager.PlaySFX("Level Up");
                lvlUpFx.SetActive(true);
            }

            if (_inputHandler.currentMonster != slotNum) return;
            GameManager.instance.uiManager.UpdateExpUI(slotNum, experienceToAdd);
        }

        #endregion

        #region Health

        public void TakeDamage(int damageToTake)
        {
            healthComponent.ReduceHealth(damageToTake);
            var currentMonster = _inputHandler.currentMonster;
            if (currentMonster < 0)
            {
                GameManager.instance.Screenshake(damageToTake >= healthComponent.health.maxHealth * .25 ? 8f : 4f, .5f);
                playerHealth.currentHealth = healthComponent.health.currentHealth;
                GameManager.instance.uiManager.UpdateHealthUI(currentMonster, playerHealth.currentHealth);
                
                if (playerHealth.currentHealth > 0) return;

                _inputHandler.movementInput = Vector2.zero;
                _inputHandler.activate = false;
                HandlePauseGameFeel(.5f);
                StartCoroutine(DelayAction(.5f, Die, true));

                return;
            }
            monsterSlots[currentMonster].currentHealth = healthComponent.health.currentHealth;
            GameManager.instance.uiManager.UpdateHealthUI(currentMonster, monsterSlots[currentMonster].currentHealth);

            var bigHit = damageToTake >= healthComponent.health.maxHealth * .25;
            var shakeIntensity =  bigHit ? 8f : 4f;
            GameManager.instance.Screenshake(shakeIntensity, .5f);
            
            if (bigHit)
            {
                HandlePauseGameFeel(.25f);
            }

            if (monsterSlots[currentMonster].currentHealth > 0) return;
            monsterSlots[currentMonster].fainted = true;
            monsterSlots[currentMonster].currentLives--;
            FindAliveMonsterOrPlayer();
        }

        public void Heal(int amountToHeal)
        {
            healthComponent.AddHealth(amountToHeal);
            if (_inputHandler.currentMonster < 0)
            {
                playerHealth.currentHealth = healthComponent.health.currentHealth;
                return;
            }
            monsterSlots[_inputHandler.currentMonster].currentHealth = healthComponent.health.currentHealth;
        }

        public void RecordDamager(MonsterSlot slot)
        {
            monsterAttacker = slot;
        }

        public void Die()
        {
            if (_tamerInvulnerable) return;

            var playerGameObject = gameObject;
            tamer.SetActive(false);
            rgdbody.useGravity = false;
            GameManager.instance.pooler.SpawnFromPool(null, playerSettings.deathParticles.name, playerSettings.deathParticles, playerTransform.position,
                Quaternion.identity);
            var playerOrigLayer = playerGameObject.layer;
            playerGameObject.layer = 0;
            skillManager.targeting = false;
            GameManager.instance.saveManager.activated = false;

            void Reset()
            {
                GameManager.instance.uiManager.modal.CloseModal();
                StopAllCoroutines();
                GameManager.instance.uiManager.loadingScreen.thisGameObject.SetActive(true);
                StartCoroutine(DelayAction(2, () => ResetGame(playerGameObject, playerOrigLayer), false));
                StartCoroutine(DelayAction(2, GameManager.instance.uiManager.loadingScreen.tweener.Disable, false));
            }

            GameManager.instance.uiManager.modal.OpenModal("<color=#f48989>Game Over</color>", playerSettings.deathIcon, _white, Reset);
        }

        public void FullyRestoreAllMonsters()
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

        public void TickTamerInvulnerability()
        {
            _tamerInvulnerable = !_tamerInvulnerable;
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

        private void HandlePauseGameFeel(float timeToTake)
        {
            GameManager.instance.gameStateController.TransitionToState(playerSettings.gameFeelState);
            GameManager.instance.pauseManager.PauseGameplay(0);
            StopAllCoroutines();
            StartCoroutine(DelayAction(timeToTake,
                () => GameManager.instance.pauseManager.PauseGameplay(1),
                true));
            StartCoroutine(DelayAction(timeToTake, () => GameManager.instance.gameStateController.TransitionToState(playerSettings.gameplayState), true));
        }

        private void ResetGame(GameObject player, int layer)
        {
            player.layer = layer;
            _inputHandler.activate = true;
            GameManager.instance.saveManager.activated = true;
            tamer.SetActive(true);
            rgdbody.useGravity = true;
            playerHealth.maxHealth = GameSettings.MonstersAvgHealth(monsterSlots.ToList()) <= 0 ?
                GameManager.instance.saveManager.defaultPlayerHealth :
                GameSettings.Stats(GameSettings.MonstersAvgHealth(monsterSlots.ToList()),
                    GameSettings.MonstersAvgStabilityValue(monsterSlots.ToList()),
                    GameSettings.MonstersAvgLevel(monsterSlots));
            
            playerHealth.currentHealth = playerHealth.maxHealth;
            healthComponent.UpdateHealth(playerHealth.maxHealth, playerHealth.currentHealth);
            GameManager.instance.uiManager.UpdateHealthUI(_inputHandler.currentMonster, playerHealth.currentHealth);

            FullyRestoreAllMonsters();
            HandleItems(true);

            TransferPlayerPositionRotation(savedData.playerWorldPlacement);
        }

        private void HandleItems(bool dead)
        {
            if(!dead) return;
            
            var inventorySlotsCount = playerInventory.inventorySlots.Count;
            for (var i = 0; i < inventorySlotsCount; i++)
            {
                var slot = playerInventory.inventorySlots[i];
                if(slot.inventoryItem == null) continue;

                if (!slot.inventoryItem.losable) continue;
                
                slot.inventoryItem = null;
                slot.amountOfItems = 0;
            }

            var monstersCount = monsterSlots.Count;
            for (var i = 0; i < monstersCount; i++)
            {
                var itemsCount = monsterSlots[i].inventory.Length;
                if(monsterSlots[i].monster == null) continue;
                for (var j = 0; j < itemsCount; j++)
                {
                    var slot = monsterSlots[i].inventory[j];
                    if(slot == null) continue;
                    if(slot.inventoryItem == null) continue;
                    if(!slot.inventoryItem.losable) continue;

                    slot.inventoryItem = null;
                    slot.amountOfItems = 0;
                }
            }
        }

        IEnumerator DelayAction(float delay, UnityAction action, bool unscaledTime)
        {
            if (unscaledTime)
            {
                yield return new WaitForSecondsRealtime(delay);
            }
            else
            {
                yield return new WaitForSeconds(delay);
            }
            action?.Invoke();
        }

        private void TransferPlayerPositionRotation(WorldPlacementData placementData)
        {
            if(placementData == null) return;

            playerTransform.SetPositionAndRotation(placementData.position, placementData.rotation);
        }
        public bool SamePositionFromSaved()
        {
            try
            {
                return savedData.playerWorldPlacement.position.Approximately(playerTransform.position + playerSettings.savePositionOffset);
            }
            catch
            {
                return false;
            }
        }

        #endregion

    }
}
