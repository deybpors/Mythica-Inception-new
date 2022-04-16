using System;
using _Core.Managers;
using _Core.Player;
using MyBox;
using UnityEngine;

namespace Monster_System
{
    public class ExperienceOrb : MonoBehaviour
    {
        public int experience;
        public int slotNum;

        [SerializeField]
        private float followSpeed = 10;
        [SerializeField]
        private SphereCollider orbCollider;

        public Rigidbody orbRigidbody;
        private bool _moveToPlayer;
        private Transform _playerTransform;
        private Player _player;
        [HideInInspector]
        public bool activated;
        private Collider[] results = new Collider[5];
        private Transform _thisTransform;

        private void OnEnable()
        {
            if (GameManager.instance == null)
            {
                orbRigidbody.useGravity = false;
                return;
            }

            _player = GameManager.instance.player;
            if (_playerTransform == null || _thisTransform == null)
            {
                _playerTransform = _player.transform;
                _thisTransform = transform;
            }
            activated = true;
        }

        private void OnDisable()
        {
            orbCollider.isTrigger = false;
            orbRigidbody.useGravity = true;
            _moveToPlayer = false;
        }

        private void FixedUpdate()
        {
            if(!activated) return;
            if (!_moveToPlayer)
            {
                PlayerCheck();
                return;
            }

            var thisPos = _thisTransform.position;
            var playerPos = _playerTransform.position;


            _thisTransform.position = Vector3.MoveTowards(thisPos, playerPos, Time.deltaTime * followSpeed);
            var dist = Vector3.Distance(thisPos, playerPos);
            if (dist > .5f) return;
            
            _player.AddExperience(experience, slotNum);
            gameObject.SetActive(false);
        }

        void PlayerCheck()
        {
            var size = Physics.OverlapSphereNonAlloc(_thisTransform.position, orbCollider.radius + .5f, results);
            for (var i = 0; i < size; i++)
            {
                if (_player.gameObject != results[i].gameObject) continue;
                
                orbCollider.isTrigger = true;
                orbRigidbody.useGravity = false;
                _moveToPlayer = true;
                break;
            }
        }

    }
}
