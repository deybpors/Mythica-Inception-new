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
        public ProgressBarUI healthBar;
        public ProgressBarUI tameValueBarUI;
        public List<MonsterSlot> monsterSlots;
        public GameObject deathParticles;
        private TameValue _tameValue;
        public ProjectileRelease projectileReleases;
        public GameObject experienceOrbSpawner;

        [ReadOnly] public MonsterSlot monsterAttacker;

        #region Hidden Fields

        [HideInInspector] public WildMonsterSpawner spawner;
        [HideInInspector] public Health healthComponent;
        [HideInInspector] public int currentMonster = 0;
        private List<GameObject> _monsterGameObjects;
        private SkillManager _skillManager;
        private MonsterManager _monsterManager;
        private Renderer _monsterRenderer;

        #endregion

        void OnEnable()
        {
            if (tamer)
            {
                Init();
            }
        }

        void Update()
        {
            if (_monsterRenderer != null && !_monsterRenderer.isVisible) return;
            
            GameManager.instance.player.AddToDiscoveredMonsters(monsterSlots[0].monster);
        }

        public void ActivateWildMonster(MonsterSlot newWildMonster, List<Transform> waypointsList, WildMonsterSpawner spawnerRef)
        {
            monsterSlots.Clear();
            monsterSlots.Add(newWildMonster);
            Init();
            gameObject.SetActive(true);
            spawner = spawnerRef;
            stateController.ActivateAI(true, waypointsList, null);
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
            SpawnMonstersFromPool();
            currentAnimator = _monsterGameObjects[0].GetComponent<Animator>();
            _monsterRenderer = _monsterGameObjects[0].GetComponent<Renderer>();
            stateController.active = true;
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

        private void SpawnMonstersFromPool()
        {
            for (int i = 0; i < monsterSlots.Count; i++)
            {
                GameObject monsterObj = GameManager.instance.pooler.SpawnFromPool(transform, monsterSlots[i].monster.monsterName,
                    monsterSlots[i].monster.monsterPrefab, Vector3.zero, Quaternion.identity);
                if (_monsterGameObjects == null)
                {
                    _monsterGameObjects = new List<GameObject>();
                }
                _monsterGameObjects.Add(monsterObj);
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

        public int CurrentMonsterSlotNumber() { return currentMonster; }

        public List<Monster> GetMonsters()
        {
            return monsterSlots.Count <= 0 ? null : monsterSlots.Select(monsterSlot => monsterSlot.monster).ToList();
        }

        public void AddNewMonsterSlot(int slotNum, MonsterSlot newSlot) { }
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
            
            if (monsterSlots[currentMonster].currentHealth <= 0)
            {
                var slotToSwitch = 9999999;
                ExtractExpOrbs();
                for (int i = 0; i < monsterSlots.Count; i++)
                {
                    if (monsterSlots[i].currentHealth > 0)
                    {
                        slotToSwitch = i;
                    }
                }

                if (slotToSwitch > monsterSlots.Count)
                {
                    Die();
                }
                else
                {
                    if (_monsterManager != null)
                    {
                        _monsterManager.SwitchMonster(slotToSwitch);
                        currentMonster = slotToSwitch;
                    }
                }
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
            var monsterTransform = transform;
            var position = monsterTransform.position;
            var pos = new Vector3(position.x, position.y + 1.5f, position.z);
            GameManager.instance.pooler.SpawnFromPool(null, deathParticles.name, deathParticles, pos,
                Quaternion.identity);
            if (spawner != null) { spawner.currentNoOfMonsters--; }

            GameManager.instance.UpdateEnemiesSeePlayer(monsterTransform, out var enemyCount);
            
            //if this object is a wild mythica
            if (!tamer)
            {
                //TODO: Update Kill Quest Type here
                //GameManager.instance.questManager.UpdateKillQuest();
            }
            
            
            if (enemyCount > 0) return;
            
            //whenever we cleared an encountered
            GameManager.instance.DifficultyUpdateAdd("Failed Encounters", 0);
            var player = GameManager.instance.player;
            GameManager.instance.DifficultyUpdateChange("Average Party Level", GameSettings.MonstersAvgLevel(player.monsterSlots));
            
            gameObject.SetActive(false);
        }

        private void ExtractExpOrbs()
        {
            var type = GameSettings.TypeComparison(monsterAttacker.monster.type,
                monsterSlots[currentMonster].monster.type) < 1;
            var exp = GameSettings.ExperienceGain(!tamer, monsterAttacker, type);
            var position = transform.position;
            var newPos = new Vector3(position.x, position.y + 1f, position.z);
            
            var expOrbSpawner = GameManager.instance.pooler.SpawnFromPool(null, experienceOrbSpawner.name, experienceOrbSpawner, newPos,
                Quaternion.identity);
            var spawnerComponent = expOrbSpawner.GetComponent<ExperienceOrbSpawner>();
            spawnerComponent.slotNum = monsterAttacker.slotNumber;
            spawnerComponent.SpawnerSpawned(exp);
        }
        
        #endregion


        public void ReleaseBasicAttack()
        {
            var monAttacking = GetCurrentMonster();
            var range = monAttacking.basicAttackType != BasicAttackType.Melee;
            var projectile = GameManager.instance.pooler.
                SpawnFromPool(range ? null : projectileReleases.front, monAttacking.basicAttackObjects.projectile.name,
                    monAttacking.basicAttackObjects.projectile, range ? projectileReleases.front.position : Vector3.zero, range ? transform.rotation : Quaternion.identity);
            var rangeProjectile = projectile.GetComponent<IDamageDetection>() ?? projectile.AddComponent<Projectile>();
            var deathTime = range ? 1f : .25f;
            var speed = range ? 30f : 20f;
            rangeProjectile.ProjectileData(true, range,monAttacking.basicAttackObjects.targetObject,monAttacking.basicAttackObjects.impact, 
                monAttacking.basicAttackObjects.muzzle,false, true, transform, stateController.aI.fieldOfView.visibleTargets[0], stateController.aI.fieldOfView.visibleTargets[0].position, deathTime, speed,.5f,monAttacking.basicAttackSkill);
        }
    }
}