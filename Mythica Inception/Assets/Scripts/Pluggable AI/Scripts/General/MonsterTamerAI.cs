using System.Collections.Generic;
using System.Linq;
using _Core.Managers;
using _Core.Others;
using Combat_System;
using Monster_System;
using MyBox;
using Skill_System;
using UI;
using UnityEngine;

namespace Pluggable_AI.Scripts.General
{
    [RequireComponent(typeof(StateController))]
    public class MonsterTamerAI : GenericAI, IHaveMonsters, IHaveHealth, ISelectable
    {
        [Foldout("Monster Tamer AI Fields", true)]
        public bool tamer;
        public bool initializeOnStart;
        public ProgressBarUI healthBar;
        public ProgressBarUI tameValueBarUI;
        public List<MonsterSlot> monsterSlots;
        public GameObject deathParticles;
        private TameValue _tameValue;
        public ProjectileRelease projectileReleases;
        public GameObject experienceOrbSpawner;
        public bool expOrbs = true;

        [ReadOnly] public MonsterSlot monsterAttacker;

        #region Hidden Fields

        [HideInInspector] public WildMonsterSpawner spawner;
        [HideInInspector] public Transform thisTransform;
        [HideInInspector] public Health healthComponent;
        [HideInInspector] public int currentMonster = 0;
        private List<GameObject> _monsterGameObjects = new List<GameObject>();
        private SkillManager _skillManager;
        private MonsterManager _monsterManager;
        private Renderer _monsterRenderer;
        private GameObject _thisGameObject;
        private Transform _gameManagerTransform;

        #endregion

        void Start()
        {
            if (initializeOnStart)
            {
                ActivateMonsterAi();
            }
        }

        private void Init()
        {
            stateController.active = false;
            agent.speed = aiData.movementSpeed;
            _monsterManager = GetComponent<MonsterManager>();
            _skillManager = GetComponent<SkillManager>();
            healthComponent = GetComponent<Health>();
            if (healthComponent == null) { healthComponent = gameObject.AddComponent<Health>(); }
            InitializeCurrentMonsterHealth();
            InitializeMonstersData();
            _skillManager.ActivateSkillManager(this);
            _monsterManager.ActivateMonsterManager(this, _skillManager);
            _monsterGameObjects.AddRange(_monsterManager.GetMonsterGameObjects());
            currentAnimator = _monsterGameObjects[0].GetComponent<Animator>();
            _monsterRenderer = _monsterGameObjects[0].GetComponent<Renderer>();
            _monsterGameObjects[0].SetActive(true);
            CorrectRotations();
            stateController.active = true;
        }

        void Update()
        {
            if (GameManager.instance.gameStateController.currentState == GameManager.instance.gameplayState)
            {
                if(_monsterManager.currentOutline == null) return;
                _monsterManager.currentOutline.enabled = true;
            }
            else
            {
                if (_monsterManager.currentOutline == null) return;
                _monsterManager.currentOutline.enabled = false;
            }

            if (_monsterRenderer != null && !_monsterRenderer.isVisible) return;

            GameManager.instance.player.AddToDiscoveredMonsters(monsterSlots[0].monster);
        }

        public void ActivateMonsterAi(List<MonsterSlot> newMonsters, List<Transform> waypointsList, WildMonsterSpawner spawnerRef)
        {
            tamer = spawnerRef == null;
            
            if (thisTransform == null)
            {
                thisTransform = transform;
            }

            monsterSlots.Clear();
            monsterSlots.AddRange(newMonsters);
            Init();
            gameObject.SetActive(true);
            spawner = spawnerRef;
            stateController.ActivateAI(true, waypointsList, null);
        }

        private void ActivateMonsterAi()
        {
            if (thisTransform == null)
            {
                thisTransform = transform;
            }
            Init();
            stateController.ActivateAI(true, waypoints, null);
            agent.Warp(thisTransform.position);
        }

        private void InitializeCurrentMonsterHealth()
        {
            healthComponent.health.maxHealth = GameSettings.Stats(
                monsterSlots[currentMonster].monster.stats.baseHealth,
                monsterSlots[currentMonster].stabilityValue,
                GameSettings.Level(monsterSlots[currentMonster].currentExp));
            healthComponent.health.currentHealth = healthComponent.health.maxHealth;
            monsterSlots[currentMonster].currentHealth = healthComponent.health.maxHealth;
        }

        private void InitializeMonstersData()
        {
            if (!tamer)
            {
                _tameValue = GetComponent<TameValue>();
                _tameValue.tameValueBarUI = tameValueBarUI;
                _tameValue.ActivateTameValue(GameSettings.Level(monsterSlots[0].currentExp), healthComponent, this);
            }
            else
            {
                //initialize data for tamers
            }
        }

        private void CorrectRotations()
        {
            for (var i = 0; i < _monsterGameObjects.Count; i++)
            {
                _monsterGameObjects[i].transform.localRotation = Quaternion.identity;
            }
        }

        public StateController GetStateController()
        {
            return stateController;
        }

        public Animator GetEntityAnimator()
        {
            return currentAnimator;
        }

        public float GetMonsterSwitchRate()
        {
            return .5f;
        }

        public List<Monster> GetMonsters()
        {
            return monsterSlots.Count <= 0 ? null : monsterSlots.Select(monsterSlot => monsterSlot.monster).ToList();
        }

        public void AddNewMonsterSlotToParty(int slotNum, MonsterSlot newSlot) { }

        public void AddNewMonsterSlotToStorage(MonsterSlot newSlot, out int slotNum) { slotNum = 0; }

        public int GetCurrentSlotNumber()
        {
            return currentMonster;
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
            return monsterSlots[currentMonster].monster;
        }

        public void SetAnimator(Animator animatorToChange)
        {
            currentAnimator = animatorToChange;
        }

        public GameObject GetTamer()
        {
            return null;
        }

        public void ChangeMonsterUnitIndicatorRadius(float radius) { }

        public void SpawnSwitchFX() { }

        public void ChangeStatsToMonster(int slot)
        {
            agent.speed = aiData.movementSpeed;
            agent.speed *= GetMonsters()[slot].stats.movementSpeed;
            healthComponent.health.maxHealth = GameSettings.Stats(
                monsterSlots[slot].monster.stats.baseHealth,
                monsterSlots[slot].stabilityValue,
                GameSettings.Level(monsterSlots[slot].currentExp));
            healthComponent.health.currentHealth = monsterSlots[slot].currentHealth;
        }

        #region Health
        public void TakeDamage(int damageToTake)
        {
            if (!healthBar.gameObject.activeInHierarchy)
            {
                healthBar.gameObject.SetActive(true);
            }
            healthComponent.ReduceHealth(damageToTake);
            monsterSlots[currentMonster].currentHealth = healthComponent.health.currentHealth;
            healthBar.maxValue = healthComponent.health.maxHealth;
            healthBar.currentValue = healthComponent.health.currentHealth;

            var shakeIntensity = damageToTake >= healthComponent.health.maxHealth * .25 ? 6f : 3f;
            GameManager.instance.Screenshake(shakeIntensity, .5f);

            if (monsterSlots[currentMonster].currentHealth > 0) return;
            
            var slotToSwitch = 9999999;

            if(expOrbs) ExtractExpOrbs();

            var monsterCount = monsterSlots.Count;
            
            for (var i = 0; i < monsterCount; i++)
            {
                if (monsterSlots[i].currentHealth > 0)
                {
                    slotToSwitch = i;
                }
            }

            if (slotToSwitch > monsterCount)
            {
                Die();
            }
            else
            {
                if (_monsterManager == null) return;
                
                _monsterManager.SwitchMonster(slotToSwitch);
                currentMonster = slotToSwitch;
            }
        }

        public void Heal(int amountToHeal)
        {
            healthComponent.AddHealth(amountToHeal);
        }

        public void RecordDamager(MonsterSlot slot)
        {
            monsterAttacker = slot;
        }

        public void Die()
        {
            var position = thisTransform.position;
            var pos = new Vector3(position.x, position.y + 1, position.z);
            GameManager.instance.pooler.SpawnFromPool(null, deathParticles.name, deathParticles, pos,
                Quaternion.identity);
            
            if (spawner != null)
            {
                spawner.currentNoOfMonsters--;
                spawner.DropItems(pos);
            }

            GameManager.instance.UpdateEnemiesSeePlayer(this, out var enemyCount);
            //if this object is a wild mythica
            if (!tamer)
            {
                GameManager.instance.questManager.UpdateKillQuest(monsterSlots[0].monster);
            }

            if (enemyCount <= 0)
            {
                GameManager.instance.DifficultyUpdateAdd("Failed Encounters", -1);
                if (_tameValue.currentTameValue > 0)
                {
                    GameManager.instance.DifficultyUpdateAdd("Failed Tame Attempts", -1);
                }
            }

            if (_thisGameObject == null)
            {
                _thisGameObject = gameObject;
                _gameManagerTransform = GameManager.instance.transform;
            }

            GameManager.instance.activeEnemies.Remove(thisTransform);
            _thisGameObject.SetActive(false);
            thisTransform.SetParent(_gameManagerTransform);
        }

        private void ExtractExpOrbs()
        {
            var type = GameSettings.TypeComparison(monsterAttacker.monster.type,
                monsterSlots[currentMonster].monster.type) < 1;
            var exp = GameSettings.ExperienceGain(!tamer, monsterAttacker, type);
            var position = thisTransform.position;
            var newPos = new Vector3(position.x, position.y + 1f, position.z);
            
            var expOrbSpawner = GameManager.instance.pooler.SpawnFromPool(null, experienceOrbSpawner.name, experienceOrbSpawner, newPos,
                Quaternion.identity);
            var spawnerComponent = expOrbSpawner.GetComponent<ExperienceOrbSpawner>();
            spawnerComponent.slotNum = monsterAttacker.slotNumber;
            spawnerComponent.SpawnerSpawned(exp);
        }
        
        #endregion

        public TameValue GetTameValue()
        {
            return _tameValue;
        }

        public void ReleaseBasicAttack()
        {
            var monAttacking = GetCurrentMonster();
            var range = monAttacking.basicAttackType != BasicAttackType.Melee;
            var projectile = GameManager.instance.pooler.
                SpawnFromPool(range ? null : projectileReleases.front, monAttacking.basicAttackObjects.projectile.name,
                    monAttacking.basicAttackObjects.projectile, range ? projectileReleases.front.position : Vector3.zero, range ? thisTransform.rotation : Quaternion.identity);
            var rangeProjectile = projectile.GetComponent<IDamageDetection>() ?? projectile.AddComponent<Projectile>();
            
            var deathTime = range ? 1f : .3f;
            var speed = range ? 30f : 20f;
            rangeProjectile.ProjectileData(true, range,monAttacking.basicAttackObjects.targetObject,monAttacking.basicAttackObjects.impact, 
                monAttacking.basicAttackObjects.muzzle,false, true, thisTransform, stateController.aI.fieldOfView.visibleTargets[0], stateController.aI.fieldOfView.visibleTargets[0].position, deathTime, speed,.5f,monAttacking.basicAttackSkill);
        }
    }
}