using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts._Core;
using Assets.Scripts.Monster_System;
using Assets.Scripts.Skill_System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Combat_System
{
    public class ProjectileMove : MonoBehaviour, IRange
    {
        private bool _isTame;
        private bool _isDamage;
        private GameObject _impactProj;
        private ParticleSystem _impactPart;
        private GameObject _muzzleProj;
        private ParticleSystem _muzzlePart;
        private GameObject _targetObject;
        private ParticleSystem _targetObjPart;
        private Transform _spawner;
        private Transform _target;
        private Vector3 _toPosition;
        private float _secondsToDeath = 5;
        private float _velocity = 5;
        private float _radius = 1;
        private ITameable _tameable;
        private float _timer;
        private bool _destroyOnCollide;
        private bool _activated;
        private Monster _spawnerMonster;
        private Skill _spawnerSkill;
        private float _spawnerSV;
        private int _spawnerLevel;
        private IHaveMonsters _haveMonsters;
        private int _tameBeamPower;

        void OnEnable()
        {
            if (_isTame)
            {
                _tameable = _target.GetComponent<ITameable>();
                if (_tameable == null)
                {
                    //TODO: display in UI that the target has to be a Wild monster
                    gameObject.SetActive(false);
                }
            }
            Init();
        }

        private void Init()
        {
            if (gameObject.activeInHierarchy)
            {
                StartCoroutine("DestroyAfter", _secondsToDeath);
            }
        }

        void Update()
        {
            if(!_activated) return;
            Vector3 position = _target == null ? _toPosition : _target.position;
            position = new Vector3(position.x, position.y + 3f, position.z);
            transform.position = Vector3.MoveTowards(transform.position, position, _velocity * Time.deltaTime);
            
            _timer += Time.deltaTime;
            
            if(_timer < .1f) return;
            _timer = 0;

            if (!Check()) return;
            
            if (_destroyOnCollide)
            {
                OnDeactivate();
            }

        }

        private bool Check()
        {
            if (_target == null)
            {
                if (_spawner.CompareTag("Player"))
                {
                    return CheckByTag("Enemy");
                }

                if (_spawner.CompareTag("Enemy"))
                {
                    return CheckByTag("Player");
                }
            }
            else
            {
                if (_isTame)
                {
                    return CheckTameTarget();
                }

                return CheckByTarget();
            }

            return false;
        }

        private bool CheckTameTarget()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, _radius);
            if (hits.Length <= 0) return false;
            
            foreach (var hit in hits)
            {
                if (hit.transform != _target) continue;
                
                if(_tameable == null) continue;
                Monster monsterToTame = hit.GetComponent<IHaveMonsters>().GetCurrentMonster();
                _tameable.AddCurrentTameValue(CalculateTameBeamValue(monsterToTame));
                return true;
            }
            return false;
        }

        private int CalculateTameBeamValue(Monster monsterToTame)
        {
            var monsterLevels = new List<int>();
            for (int i = 0; i < _haveMonsters.GetMonsterSlots().Count; i++)
            {
                monsterLevels.Add(GameCalculations.Level(_haveMonsters.GetMonsterSlots()[i].currentExp));
            }
            var avgLevel = (int)monsterLevels.Average();
            return GameCalculations.TameBeam(avgLevel, _tameBeamPower, monsterToTame.stats.tameResistance, 1);
        }

        private bool CheckByTarget()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, _radius);
            if (hits.Length <= 0) return false;
            
            foreach (var hit in hits)
            {
                if (hit.transform != _target) continue;
                
                IHaveHealth damageable = hit.transform.gameObject.GetComponent<IHaveHealth>();
                if (damageable == null) continue;
                IHaveMonsters hitHaveMonster = hit.GetComponent<IHaveMonsters>();
                Monster monsterHit = hitHaveMonster.GetCurrentMonster();
                if (_isDamage)
                {
                    damageable.TakeDamage(CalculateDamage(monsterHit, hitHaveMonster));
                    return true;
                }
            }

            return false;
        }

        private bool CheckByTag(string opponentTag)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, _radius);
            if (hits.Length <= 0) return false;
            
            foreach (var hit in hits)
            {
                if (!hit.CompareTag(opponentTag)) continue;
                
                IHaveHealth damageable = hit.transform.gameObject.GetComponent<IHaveHealth>();
                if (damageable == null) continue;
                IHaveMonsters hitHaveMonster = hit.GetComponent<IHaveMonsters>();
                Monster monsterHit = hitHaveMonster.GetCurrentMonster();
                if (_isDamage)
                {
                    damageable.TakeDamage(CalculateDamage(monsterHit, hitHaveMonster));
                    return true;
                }
            }

            return false;
        }

        private int CalculateDamage(Monster monsterHit, IHaveMonsters hitHaveMonster)
        {
            //if the skill used is the spawner monster's basic attack
            if (_spawnerMonster.basicAttack == _spawnerSkill)
            {
                var attackerAttack = 0;
                var hitDefense = 0;
                if (_spawnerMonster.basicAttackType == BasicAttackType.Melee)
                {
                    attackerAttack = GameCalculations.Stats(_spawnerMonster.stats.physicalAttack, _spawnerSV, _spawnerLevel);
                    int hitLevel =
                        GameCalculations.Level(hitHaveMonster.GetMonsterSlots()[_haveMonsters.MonsterSwitched()]
                            .currentExp);
                    hitDefense = GameCalculations.Stats(monsterHit.stats.physicalDefense, hitHaveMonster.GetMonsterSlots()[_haveMonsters.MonsterSwitched()].stabilityValue, hitLevel);
                    
                }
                else
                {
                    attackerAttack = GameCalculations.Stats(_spawnerMonster.stats.specialAttack, _spawnerSV, _spawnerLevel);
                    int hitLevel =
                        GameCalculations.Level(hitHaveMonster.GetMonsterSlots()[_haveMonsters.MonsterSwitched()]
                            .currentExp);
                    hitDefense = GameCalculations.Stats(monsterHit.stats.specialDefense, hitHaveMonster.GetMonsterSlots()[_haveMonsters.MonsterSwitched()].stabilityValue, hitLevel);
                }

                float typeComparison = GameCalculations.TypeComparison(_spawnerMonster.type, monsterHit.type);
                float modifier = GameCalculations.Modifier(true, _spawnerSV, typeComparison, Random.Range(0f, 1f) <= _spawnerMonster.stats.criticalChance);
                int damage = GameCalculations.Damage(1, _spawnerLevel, attackerAttack, hitDefense, _spawnerSkill.power,
                    255, modifier);
                
                return damage;
            }
            //if the skill used is a skill of the spawner (not basic attack)
            else
            {
                var attackerAttack = 0;
                var hitDefense = 0;
                if (_spawnerSkill.skillCategory == SkillCategory.Physical)
                {
                    attackerAttack = _spawnerMonster.stats.physicalAttack;
                    hitDefense = monsterHit.stats.physicalDefense;
                }
                else
                {
                    attackerAttack = _spawnerMonster.stats.specialAttack;
                    hitDefense = monsterHit.stats.specialDefense;
                }
                float typeComparison = GameCalculations.TypeComparison(_spawnerMonster.type, monsterHit.type);
                float modifier = GameCalculations.Modifier(_spawnerSkill.skillType == _spawnerMonster.type, _spawnerSV, typeComparison, Random.Range(0f, 1f) <= _spawnerMonster.stats.criticalChance);
                return GameCalculations.Damage(1, _spawnerLevel, attackerAttack, hitDefense, _spawnerSkill.power, 255, modifier);
            }
        }

        IEnumerator DestroyAfter(float sec)
        {
            yield return new WaitForSeconds(_secondsToDeath);
            OnDeactivate();
        }

        private void OnDeactivate()
        {
            if (GameManager.instance == null || GameManager.instance.Equals(null)) return;
            gameObject.SetActive(false);
            
            if (_impactProj != null)
            {
                
                
                GameObject impact = GameManager.instance.pooler.SpawnFromPool(null, _impactProj.name, _impactProj,
                    transform.position, Quaternion.identity);
            }

            if (_muzzleProj != null)
            {
                GameObject muzzle = GameManager.instance.pooler.SpawnFromPool(null, _muzzleProj.name, _muzzleProj,
                    transform.position, Quaternion.identity);
            }

            if (_targetObject != null)
            {
                GameObject targetObj = GameManager.instance.pooler.SpawnFromPool(_target, _targetObject.name,
                    _targetObject, Vector3.zero, Quaternion.identity);
            }
            
            
        }

        public void ProjectileData(bool destroyOnCollide, GameObject targetObject,GameObject impactParticle, GameObject muzzleParticle, bool isTameBeam, bool canDamage, Transform whoSpawned, Transform whatTarget, Vector3 toPosition, float secondsToDie, float howFast, float whatRadius, Skill skill)
        {
            gameObject.SetActive(false);
            _isTame = isTameBeam;
            _isDamage = canDamage;
            _spawner = whoSpawned;
            _target = whatTarget;
            _toPosition = toPosition;
            _secondsToDeath = secondsToDie;
            _velocity = howFast;
            _radius = whatRadius;
            _impactProj = impactParticle;
            if (_impactProj != null)
            {
                _impactPart = impactParticle.GetComponent<ParticleSystem>();
            }
            _muzzleProj = muzzleParticle;
            if (_muzzleProj != null)
            {
                _muzzlePart = muzzleParticle.GetComponent<ParticleSystem>();
            }
            _targetObject = targetObject;
            if (targetObject != null)
            {
                _targetObjPart = targetObject.GetComponent<ParticleSystem>();
            }
            _destroyOnCollide = destroyOnCollide;
            
            _haveMonsters = _spawner.GetComponent<IHaveMonsters>();
            
            if (isTameBeam)
            {
                TameBeamData(skill);
            }
            else
            {
                
                _spawnerMonster = _haveMonsters.GetCurrentMonster();
                _spawnerSV = _haveMonsters.GetMonsterSlots()[_haveMonsters.MonsterSwitched()].stabilityValue;
                _spawnerSkill = skill;
                _spawnerLevel = GameCalculations.Level(_haveMonsters.GetMonsterSlots()[_haveMonsters.MonsterSwitched()].currentExp);
            }
            
            gameObject.SetActive(true);
            _activated = true;
        }

        private void TameBeamData(Skill skill)
        {
            _tameBeamPower = skill.power;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }
}
