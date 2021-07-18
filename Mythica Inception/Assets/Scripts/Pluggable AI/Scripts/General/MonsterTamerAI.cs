using System.Collections.Generic;
using System.Linq;
using Assets.Scripts._Core;
using Assets.Scripts._Core.Managers;
using Assets.Scripts._Core.Others;
using Assets.Scripts.Combat_System;
using Assets.Scripts.Monster_System;
using Assets.Scripts.Skill_System;
using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Pluggable_AI.Scripts.General
{
    [RequireComponent(typeof(StateController))]
    public class MonsterTamerAI : GenericAI, IEntity, IHaveMonsters, IHaveHealth, ISelectable
    {
        public bool tamer;
        public ProgressBarUI healthBar;
        public ProgressBarUI tameValueBarUI;
        public List<MonsterSlot> monsterSlots;
        public GameObject deathParticles;
        private TameValue _tameValue;
        public Transform projectileRelease;
        
        #region Hidden Fields

        private WildMonsterSpawner _spawner;
        [HideInInspector] public Health healthComponent;
        [HideInInspector] public int currentMonster = 0;
        [HideInInspector] private List<GameObject> _monsterGameObjects;
        private SkillManager _skillManager;
        private MonsterManager _monsterManager;

        #endregion

        void OnEnable()
        {
            if (tamer)
            {
                Init();
            }
        }

        public void ActivateWildMonster(MonsterSlot newWildMonster, List<Transform> waypoints, WildMonsterSpawner spawner)
        {
            monsterSlots.Clear();
            monsterSlots.Add(newWildMonster);
            Init();
            gameObject.SetActive(true);
            _spawner = spawner;
            stateController.ActivateAI(true, waypoints, null);
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
            stateController.active = true;
        }

        private void InitializeCurrentMonsterHealth()
        {
            healthComponent.health.maxHealth = GameCalculations.Stats(
                monsterSlots[currentMonster].monster.stats.baseHealth,
                monsterSlots[currentMonster].stabilityValue,
                GameCalculations.Level(monsterSlots[currentMonster].currentExp));
            healthComponent.health.currentHealth = healthComponent.health.maxHealth;
            monsterSlots[currentMonster].currentHealth = healthComponent.health.maxHealth;
        }

        private void InitializeMonstersData()
        {
            if (!tamer)
            {
                _tameValue = GetComponent<TameValue>();
                _tameValue.tameValueBarUI = tameValueBarUI;
                _tameValue.ActivateTameValue(GameCalculations.Level(monsterSlots[0].currentExp), healthComponent, this);
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
        public int CurrentSlotNumber()
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
            healthComponent.health.maxHealth = GameCalculations.Stats(
                monsterSlots[slot].monster.stats.baseHealth,
                monsterSlots[slot].stabilityValue,
                GameCalculations.Level(monsterSlots[slot].currentExp));
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
                    }
                }
            }
        }

        public void Heal(int amountToHeal)
        {
            healthComponent.AddHealth(amountToHeal);
        }

        public void Die()
        {
            var pos = new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z);
            GameManager.instance.pooler.SpawnFromPool(null, deathParticles.name, deathParticles, pos,
                Quaternion.identity);
            if (_spawner != null) { _spawner.currentNoOfMonsters--; }
            gameObject.SetActive(false);
        }
        
        #endregion


        public void ReleaseBasicAttack()
        {
            var monAttacking = GetCurrentMonster();
            var range = monAttacking.basicAttackType != BasicAttackType.Melee;
            var projectile = GameManager.instance.pooler.
                SpawnFromPool(range ? null : projectileRelease.transform, monAttacking.basicAttackObjects.projectile.name,
                    monAttacking.basicAttackObjects.projectile, range ? projectileRelease.position : Vector3.zero, range ? transform.rotation : Quaternion.identity);
            var rangeProjectile = projectile.GetComponent<IDamageDetection>() ?? projectile.AddComponent<Projectile>();
            var deathTime = range ? 1f : .25f;
            var speed = range ? 50f : 30f;
            rangeProjectile.ProjectileData(true, range,monAttacking.basicAttackObjects.targetObject,monAttacking.basicAttackObjects.impact, 
                monAttacking.basicAttackObjects.muzzle,false, true, transform, stateController.aI.fieldOfView.visibleTargets[0], stateController.aI.fieldOfView.visibleTargets[0].position, deathTime, speed,1.5f,monAttacking.basicAttackSkill);
        }
    }
}