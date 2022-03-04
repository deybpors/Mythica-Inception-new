using System.Collections;
using _Core.Managers;
using _Core.Others;
using Monster_System;
using Skill_System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Combat_System
{
    public class Projectile : MonoBehaviour, IDamageDetection
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
        private float spawnerSv;
        private int _spawnerLevel;
        private IHaveMonsters _haveMonsters;
        private float _tameBeamPower;
        private bool _range;
        private IMovingProjectile _movingProjectile;
        private bool _hitOnTarget;
        private Collider[] _colliders = new Collider[5];
        private Vector3 zero = Vector3.zero;
        private MonsterSlot _spawnerSlot;
        void OnEnable()
        {
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
            
            _movingProjectile.Move(_range, _target, _velocity , 0f);

            //limit checking of colliders
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
                return _isTame ? CheckTameTarget() : CheckByTarget();
            }

            return false;
        }

        private bool CheckTameTarget()
        {
            var size = Physics.OverlapSphereNonAlloc(transform.position, _radius, _colliders);
            if (size <= 0) return false;

            for (var i = 0; i < size; i++)
            {
                var hit = _colliders[i];
                if (hit.gameObject.layer == LayerMask.NameToLayer("Ground") ||
                    hit.gameObject.layer == LayerMask.NameToLayer("Obstacles")) continue;

                if (hit.transform != _target) continue;
                if (_tameable == null) continue;
                var monsterToTame = hit.GetComponent<IHaveMonsters>().GetCurrentMonster();
                _hitOnTarget = true;
                _tameable.AddCurrentTameValue(CalculateTameBeamValue(monsterToTame), _haveMonsters);
                return true;
            }

            return true;
        }

        private bool CheckByTarget()
        {
            var size = Physics.OverlapSphereNonAlloc(transform.position, _radius, _colliders);
            if (size <= 0) return false;
            var hitSomething = false;
            
            for (var i = 0; i < size; i++)
            {
                var hit = _colliders[i];
                if (hit.gameObject.layer == LayerMask.NameToLayer("Ground") ||
                    hit.gameObject.layer == LayerMask.NameToLayer("Obstacles"))
                {
                    hitSomething = true;
                    continue;
                }

                if (hit.transform != _target)
                {
                    if (hitSomething && i == size - 1)
                    {
                        return true;
                    }
                    continue;
                }
                var damageable = hit.transform.gameObject.GetComponent<IHaveHealth>();
                var hitHaveMonster = hit.GetComponent<IHaveMonsters>();
                if (hitHaveMonster == null) continue;
                var monsterHit = hitHaveMonster.GetCurrentMonster();
                if (_isDamage)
                {
                    if (damageable == null)
                    {
                        continue;
                    }

                    _hitOnTarget = true;
                    damageable.TakeDamage(CalculateDamage(monsterHit, hitHaveMonster));
                    damageable.RecordDamager(_spawnerSlot);
                    return true;
                }
            }

            return hitSomething;
        }

        private bool CheckByTag(string opponentTag)
        {
            var size = Physics.OverlapSphereNonAlloc(transform.position, _radius, _colliders);
            if (size <= 0) return false;

            for (var i = 0; i < size; i++)
            {
                var hit = _colliders[i];
                if (hit.gameObject.layer == LayerMask.NameToLayer("Ground") ||
                    hit.gameObject.layer == LayerMask.NameToLayer("Obstacles")) continue;
                if (!hit.CompareTag(opponentTag)) continue;
                var damageable = hit.transform.gameObject.GetComponent<IHaveHealth>();
                if (damageable == null) continue;
                var hitHaveMonster = hit.GetComponent<IHaveMonsters>();
                if (hitHaveMonster == null) continue;
                var monsterHit = hitHaveMonster.GetCurrentMonster();
                if (_isDamage)
                {
                    _target = hit.transform;
                    _hitOnTarget = true;
                    damageable.TakeDamage(CalculateDamage(monsterHit, hitHaveMonster));
                    damageable.RecordDamager(_spawnerSlot);
                    return true;
                }
            }

            return true;
        }

        private int CalculateTameBeamValue(Monster monsterToTame)
        {
            var avgLevel = GameCalculations.MonstersAvgLevel(_haveMonsters.GetMonsterSlots());
            return GameCalculations.TameBeam(avgLevel, _tameBeamPower, monsterToTame.stats.tameResistance);
        }

        private int CalculateDamage(Monster monsterHit, IHaveMonsters hitHaveMonster)
        {
            int hitLevel;
            int hitDefense;
            float typeComparison;
            //if the basic attack skill of attacker monster is used or attacker skill type is same with the attacker monster's type
            var stab = _spawnerMonster.basicAttackSkill == _spawnerSkill || _spawnerSkill.skillType == _spawnerMonster.type;

            var attackerAttack = GameCalculations.Stats(
                _spawnerSkill.skillCategory == SkillCategory.Physical ? 
                    _spawnerMonster.stats.physicalAttack : 
                    _spawnerMonster.stats.specialAttack, 
                spawnerSv, 
                _spawnerLevel);

            if (monsterHit == null)
            {
                hitLevel = GameCalculations.MonstersAvgLevel(_haveMonsters.GetMonsterSlots());
                monsterHit = _haveMonsters.GetMonsterWithHighestExp().monster;
                typeComparison = 1f;
                hitDefense = GameCalculations.Stats(
                    _spawnerSkill.skillCategory == SkillCategory.Physical ? 
                        monsterHit.stats.physicalDefense : monsterHit.stats.specialDefense, 
                    hitHaveMonster.GetMonsterSlots()[_haveMonsters.GetCurrentSlotNumber()].stabilityValue, 
                    hitLevel);
            }
            else
            {
                hitLevel = GameCalculations.Level(hitHaveMonster.GetMonsterSlots()[hitHaveMonster.GetCurrentSlotNumber()].currentExp);
                typeComparison = GameCalculations.TypeComparison(_spawnerMonster.type, monsterHit.type);
                hitDefense = GameCalculations.Stats(
                    _spawnerSkill.skillCategory == SkillCategory.Physical ? 
                    monsterHit.stats.physicalDefense : monsterHit.stats.specialDefense, 
                    hitHaveMonster.GetMonsterSlots()[hitHaveMonster.GetCurrentSlotNumber()].stabilityValue, 
                    hitLevel);
            }
            
            var modifier = GameCalculations.Modifier(stab, spawnerSv, typeComparison, Random.Range(0f, 1f) <= _spawnerMonster.stats.criticalChance);
            return GameCalculations.Damage(_spawnerLevel, attackerAttack, hitDefense, _spawnerSkill.power, 255, modifier);
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
                var impact = GameManager.instance.pooler.SpawnFromPool(null, _impactProj.name, _impactProj,
                    transform.position, Quaternion.identity);
            }

            if (_muzzleProj != null)
            {
                var muzzle = GameManager.instance.pooler.SpawnFromPool(null, _muzzleProj.name, _muzzleProj,
                    transform.position, Quaternion.identity);
            }

            if (_targetObject == null) return;
            
            if (_target != null && _hitOnTarget)
            {
                var targetObj = GameManager.instance.pooler.SpawnFromPool(_target, _targetObject.name,
                    _targetObject, zero, Quaternion.identity);
            }
            
            
        }

        public void ProjectileData(bool destroyOnCollide, bool range, GameObject targetObject,GameObject impactParticle, GameObject muzzleParticle, bool isTameBeam, bool canDamage, Transform whoSpawned, Transform whatTarget, Vector3 toPosition, float secondsToDie, float howFast, float whatRadius, Skill skill)
        {
            gameObject.SetActive(false);
            _movingProjectile = GetComponent<IMovingProjectile>();
            _isTame = isTameBeam;
            _isDamage = canDamage;
            _spawner = whoSpawned;
            _target = whatTarget;
            _toPosition = toPosition;
            _secondsToDeath = secondsToDie;
            _velocity = howFast;
            _radius = whatRadius;
            _impactProj = impactParticle;
            _range = range;
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
                TameBeamData(skill, whatTarget.GetComponent<ITameable>());
            }
            else
            {
                _spawnerSlot = _haveMonsters.GetMonsterSlots()[_haveMonsters.GetCurrentSlotNumber()];
                _spawnerMonster = _haveMonsters.GetCurrentMonster();
                spawnerSv = _spawnerSlot.stabilityValue;
                _spawnerSkill = skill;
                _spawnerLevel = GameCalculations.Level(_spawnerSlot.currentExp);
            }
            
            gameObject.SetActive(true);
            _activated = true;
        }

        private void TameBeamData(Skill skill, ITameable targetMonster)
        {
            _tameBeamPower = skill.power;
            _tameable = targetMonster;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }
}
