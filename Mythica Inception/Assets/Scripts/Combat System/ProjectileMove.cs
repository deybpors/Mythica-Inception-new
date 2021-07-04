using System.Collections;
using Assets.Scripts._Core;
using Assets.Scripts.Core;
using Assets.Scripts.Monster_System;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Combat_System
{
    public class ProjectileMove : MonoBehaviour, IRange
    {
        private bool _isTame;
        private bool _isDamage;
        private int _value;
        private GameObject _impactProj;
        private ParticleSystem _impactPart;
        private GameObject _muzzleProj;
        private ParticleSystem _muzzlePart;
        private Transform _spawner;
        private Transform _target;
        private Vector3 _toPosition;
        private float _secondsToDeath = 5;
        private float _velocity = 5;
        private float _radius = 1;
        private ITameable _tameable;
        private UnityAction<string> _action;
        private float _timer;

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
            StartCoroutine("DestroyAfter", _secondsToDeath);
        }

        void Update()
        {
            Vector3 position = _target == null ? _toPosition : _target.position;
            transform.position = Vector3.MoveTowards(transform.position, position, _velocity * Time.deltaTime);
            
            _timer += Time.deltaTime;
            
            if(_timer < .1f) return;
            _timer = 0;
            Check();
            
        }

        private void Check()
        {
            if (_target == null)
            {
                if (_spawner.CompareTag("Player"))
                {
                    CheckByTag("Enemy");
                    return;
                }

                if (_spawner.CompareTag("Enemy"))
                {
                    CheckByTag("Player");
                }
            }
            else
            {
                if (_isTame)
                {
                    CheckTameTarget();
                    return;
                }

                CheckByTarget();
            }
        }

        private void CheckTameTarget()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, _radius);
            if (hits.Length <= 0) return;
            
            foreach (var hit in hits)
            {
                if (hit.transform != _target) continue;
                
                if(_tameable==null) continue;
                
                _tameable.AddCurrentTameValue(_value);
                OnDestroy();
            }
        }

        private void CheckByTarget()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, _radius);
            if (hits.Length <= 0) return;
            
            foreach (var hit in hits)
            {
                if (!hit.transform != _target) continue;
                IHaveHealth damageable = hit.transform.gameObject.GetComponent<IHaveHealth>();
                if (damageable == null) continue;
                if (_isDamage)
                {
                    damageable.TakeDamage(_value);
                }
            }
        }

        private void CheckByTag(string tag)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, _radius);
            if (hits.Length <= 0) return;
            
            foreach (var hit in hits)
            {
                if (!hit.CompareTag(tag)) continue;
                IHaveHealth damageable = hit.transform.gameObject.GetComponent<IHaveHealth>();
                if (damageable == null) continue;
                if (_isDamage)
                {
                    damageable.TakeDamage(_value);
                }
            }
        }

        IEnumerator DestroyAfter(float sec)
        {
            yield return new WaitForSeconds(_secondsToDeath);
            OnDestroy();
        }

        private void OnDestroy()
        {
            if (_impactProj != null)
            {
                GameObject impact = GameManager.instance.pooler.SpawnFromPool(null, _impactProj.name, _impactProj,
                    transform.position, Quaternion.identity);
                _impactPart.Play();
            }

            if (_muzzleProj != null)
            {
                GameObject muzzle = GameManager.instance.pooler.SpawnFromPool(null, _muzzleProj.name, _muzzleProj,
                    transform.position, Quaternion.identity);
                _muzzlePart.Play();
            }

            gameObject.SetActive(false);
        }

        public void ProjectileData(GameObject impactParticle, GameObject muzzleParticle, bool isTameBeam, bool canDamage,int whatValue, Transform whoSpawned, Transform whatTarget, Vector3 wherePosition, float secondsToDie, float howFast, float whatRadius)
        {
            gameObject.SetActive(false);
            _isTame = isTameBeam;
            _isDamage = canDamage;
            _value = whatValue;
            _spawner = whoSpawned;
            _target = whatTarget;
            _toPosition = wherePosition;
            _secondsToDeath = secondsToDie;
            _velocity = howFast;
            _radius = whatRadius;
            _impactProj = impactParticle;
            _impactPart = impactParticle.GetComponent<ParticleSystem>();
            _muzzleProj = muzzleParticle;
            _muzzlePart = muzzleParticle.GetComponent<ParticleSystem>();
            gameObject.SetActive(true);
        }
        
        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }
}
