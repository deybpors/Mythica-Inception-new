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
        private Transform playerTransform;
        private Player player;
        [HideInInspector]
        public bool activated;
        private Collider[] results = new Collider[5];
        private void OnEnable()
        {
            player = GameManager.instance.player;
            playerTransform = player.transform;
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

            var thisPos = transform.position;
            var playerPos = playerTransform.position;
            
            
            transform.position = Vector3.MoveTowards(thisPos, playerPos, Time.deltaTime * followSpeed);
            var dist = Vector3.Distance(thisPos, playerPos);
            if (dist > .5f) return;
            
            player.AddExperience(experience, slotNum);
            gameObject.SetActive(false);
        }

        void PlayerCheck()
        {
            var size = Physics.OverlapSphereNonAlloc(transform.position, orbCollider.radius + .5f, results);
            for (var i = 0; i < size; i++)
            {
                if (player.gameObject != results[i].gameObject) continue;
                
                orbCollider.isTrigger = true;
                orbRigidbody.useGravity = false;
                _moveToPlayer = true;
                break;
            }
        }

    }
}
