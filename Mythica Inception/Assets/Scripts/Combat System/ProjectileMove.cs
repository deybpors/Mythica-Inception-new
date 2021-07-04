using System.Collections;
using Assets.Scripts.Core;
using Assets.Scripts.Monster_System;
using UnityEngine;

namespace Assets.Scripts.Combat_System
{
    [RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
    public class ProjectileMove : MonoBehaviour, IRange
    {
        public bool isTame;
        public bool isDamage;
        public int value;
        [HideInInspector] public Transform spawner;
         public Transform target;
        [HideInInspector] public Vector3 toPosition;
        public float secondsToDeath = 5;
        public float velocity = 5;
        public float radius = 1;
        private Rigidbody _rigidbody;
        private SphereCollider _sphereCollider;
        private ITameable _tameable;
        private bool _fall;
        
        void OnEnable()
        {
            if (isTame)
            {
                _tameable = target.GetComponent<ITameable>();
                if (_tameable == null)
                {
                    //TODO: Update this instead of destroy, inactive or something
                    Destroy(gameObject);
                }
            }
            Init();
        }

        private void Init()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _sphereCollider = GetComponent<SphereCollider>();
            _rigidbody.useGravity = false;
            _sphereCollider.enabled = false;
            StartCoroutine("Fall", secondsToDeath/2);
            StartCoroutine("DestroyAfter", secondsToDeath);
        }

        void Update()
        {
            if(_fall) return;
            
            Vector3 position = target == null ? toPosition : new Vector3(target.position.x, target.position.y + .5f, target.position.z);
            transform.position = Vector3.MoveTowards(transform.position, position, velocity * Time.deltaTime);

            if (target == null)
            {
                if (spawner.CompareTag("Player"))
                {
                    CheckByTag("Enemy");
                    return;
                }

                if (spawner.CompareTag("Enemy"))
                {
                    CheckByTag("Player");
                }
            }
            else
            {
                if (isTame)
                {
                    CheckTameTarget();
                    return;
                }
                CheckByTarget();
            }
        }

        private void CheckTameTarget()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, radius);
            if (hits.Length <= 0) return;
            
            foreach (var hit in hits)
            {
                if (hit.transform != target) continue;
                
                if(_tameable==null) continue;
                
                _tameable.AddCurrentTameValue(value);
                Destroy(gameObject);
            }
        }

        private void CheckByTarget()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, radius);
            if (hits.Length <= 0) return;
            
            foreach (var hit in hits)
            {
                if (!hit.transform != target) continue;
                IHaveHealth damageable = hit.transform.gameObject.GetComponent<IHaveHealth>();
                if (damageable == null) continue;
                if (isDamage)
                {
                    damageable.TakeDamage(value);
                }
            }
        }

        private void CheckByTag(string tag)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, radius);
            if (hits.Length <= 0) return;
            
            foreach (var hit in hits)
            {
                if (!hit.CompareTag(tag)) continue;
                IHaveHealth damageable = hit.transform.gameObject.GetComponent<IHaveHealth>();
                if (damageable == null) continue;
                if (isDamage)
                {
                    damageable.TakeDamage(value);
                }
            }
        }

        IEnumerator DestroyAfter(float sec)
        {
            yield return new WaitForSeconds(secondsToDeath);
            //TODO: Destroy particle for range here
            gameObject.SetActive(false);
        }

        IEnumerator Fall(float sec)
        {
            yield return new WaitForSeconds(sec);
            _fall = true;
            _rigidbody.useGravity = true;
            _sphereCollider.enabled = true;
            _sphereCollider.radius = radius;
        }
        
        public void SetDataForProjectile(bool isTameBeam, bool canDamage,int whatValue, Transform whoSpawned, Transform whatTarget, Vector3 wherePosition, float secondsToDie, float howFast, float whatRadius)
        {
            gameObject.SetActive(false);
            isTame = isTameBeam;
            isDamage = canDamage;
            value = whatValue;
            spawner = whoSpawned;
            target = whatTarget;
            toPosition = wherePosition;
            secondsToDeath = secondsToDie;
            velocity = howFast;
            radius = whatRadius;
            gameObject.SetActive(true);
        }
        
        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}
