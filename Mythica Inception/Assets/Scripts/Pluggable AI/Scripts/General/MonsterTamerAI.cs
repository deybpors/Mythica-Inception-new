using System.Collections.Generic;
using System.Linq;
using Assets.Scripts._Core;
using Assets.Scripts.Monster_System;
using Assets.Scripts.Skill_System;
using Assets.Scripts.UI;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts.Pluggable_AI.Scripts.General
{
    [RequireComponent(typeof(StateController))]
    public class MonsterTamerAI : MonoBehaviour, IEntity, IHaveMonsters, IHaveHealth, ISelectable
    {
        public bool tamer;
        public AIStats aiStats;
        public NavMeshAgent agent;
        public FieldOfView fieldOfView;
        public GameObject unitIndicator;
        public List<Transform> waypoints;
        public List<MonsterSlot> monsterSlots;
        public GameObject deathParticles;

        #region Hidden Fields
        
        [HideInInspector] public Health healthComponent;
        [HideInInspector] public Animator currentAnimator = null;
        [HideInInspector] public int nextWaypoint;
        [HideInInspector] public Transform target;
        [HideInInspector] public Vector3 lastKnownTargetPosition;
        [HideInInspector] public int currentMonster;
        private StateController _stateController;
        [HideInInspector] private List<GameObject> _monsterGameObjects;
        private SkillManager _skillManager;
        private MonsterManager _monsterManager;
        #endregion

        void Start()
        {
            // if (tamer)
            // {
                Init();
            // }
        }

        private void Init()
        {
            _stateController = GetComponent<StateController>();
            _stateController.isActive = false;
            
            if (!tamer)
            {
                //if Wild monsters
                InitializeMonstersData();
            }
            SpawnMonstersFromPool();
            currentAnimator = _monsterGameObjects[0].GetComponent<Animator>();
            healthComponent = GetComponent<Health>();
            if (healthComponent == null)
            {
                Debug.LogWarning("Added new Health component to " + this.name + ". Please setup this first.");
                healthComponent = gameObject.AddComponent<Health>();
            }

            _monsterManager = GetComponent<MonsterManager>();
            _stateController.isActive = true;
        }

        private void InitializeMonstersData()
        {
            //TODO: initialize monster's data here (put Monsters in the monsters list)
            //use the monster's place and get random monsters from the place's monster's list
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
            return _stateController;
        }

        public Animator GetEntityAnimator()
        {
            return currentAnimator;
        }

        public float GetMonsterSwitchRate()
        {
            return .5f;
        }

        public int MonsterSwitched() { return currentMonster; }

        public List<Monster> GetMonsters()
        {
            if (monsterSlots.Count <= 0) return null;

            return monsterSlots.Select(monsterSlot => monsterSlot.monster).ToList();
        }

        public List<MonsterSlot> GetMonsterSlots()
        {
            return monsterSlots;
        }

        public Monster GetCurrentMonster()
        {
            return monsterSlots[currentMonster].monster;
        }

        public bool isPlayerSwitched()
        {
            return false;
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

        public void ReleaseMonAttackProjectile()
        {
            
        }
        
        public void SpawnSwitchFX() { }

        public void ActivateSkillManager(bool isActivated)
        {
            if (_skillManager == null)
            {
                _skillManager = GetComponent<SkillManager>();
            }

            _skillManager.enabled = isActivated;
        }

        #region Health
        public void TakeDamage(int damageToTake)
        {
            Debug.Log(damageToTake);
            healthComponent.ReduceHealth(damageToTake);
            monsterSlots[currentMonster].currentHealth = healthComponent.health.currentHealth;
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
            gameObject.SetActive(false);
        }
        
        #endregion
        
    }
}