using System.Collections;
using System.Collections.Generic;
using _Core.Player;
using Dialogue_System;
using UnityEngine;

namespace Assets.Scripts.Dialogue_System
{
    public class NPCDialogue : MonoBehaviour, IInteractable
    {
        [SerializeField] private Character _character;
        [SerializeField] private Conversation _conversation;
        [SerializeField] private float _rotateTime = .25f;

        public void Interact(Player player)
        {
            var lookPosition = player.transform.position - transform.position;
            lookPosition.y = 0;
            var rotation = Quaternion.LookRotation(lookPosition);
            
            StopAllCoroutines();
            StartCoroutine(LookTowards(player, rotation));
        }

        private IEnumerator LookTowards(Player player, Quaternion rotateTo)
        {
            var currentRotation = transform.rotation;
            var timeElapsed = 0f;

            while (timeElapsed < _rotateTime)
            {
                transform.rotation = Quaternion.Lerp(currentRotation, rotateTo, timeElapsed / _rotateTime);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
        }
    }
}
