using System.Collections;
using System.Collections.Generic;
using _Core.Managers;
using _Core.Player;
using Dialogue_System;
using UI;
using UnityEngine;

namespace Assets.Scripts.Dialogue_System
{
    public class NPCDialogue : MonoBehaviour, IInteractable
    {
        [SerializeField] private Conversation _conversation;
        [SerializeField] private float _rotateTime = .25f;
        [SerializeField] private float _interactableDistance = 1f;

        private bool _isInteractable = false;
        private Outline _outline;
        private Transform _playerTransform;
        private Transform _npcTransform;

        void Start()
        {
            _outline = GetComponentInChildren<Outline>();
            try
            {
                _playerTransform = GameManager.instance.player.transform;
            }
            catch
            {
                // ignored
            }

            _npcTransform = transform;
        }

        void Update()
        {
            var player = GameManager.instance.player;
            if (_playerTransform == null)
            {
                try
                {
                    _playerTransform = player.transform;
                }
                catch
                {
                    return;
                }
            }

            var distanceToPlayer = Vector3.Distance(_playerTransform.position, _npcTransform.position);
            if (distanceToPlayer <= _interactableDistance)
            {
                _outline.enabled = true;
                _outline.OutlineMode = Outline.Mode.OutlineVisible;
                
                if(player.inputHandler.currentMonster >= 0) return;

                _isInteractable = true;
            }
            else
            {
                _outline.enabled = false;
                _isInteractable = false;
            }

            if (!player.inputHandler.interact) return;
            
            player.inputHandler.interact = false;
            Interact(player);
        }

        public void Interact(Player player)
        {
            if(!_isInteractable) return;
            if(_conversation == null) return;

            GameManager.instance.gameStateController.TransitionToState(GameManager.instance.dialogueState);
            GameManager.instance.player.inputHandler.GetPlayerInputSettings().SwitchCurrentActionMap("Dialogue");

            var npcLookPosition = _playerTransform.position - _npcTransform.position;
            npcLookPosition.y = 0;
            var playerLookPosition = _npcTransform.position - _playerTransform.position;
            playerLookPosition.y = 0;

            var npcRotateTo = Quaternion.LookRotation(npcLookPosition);
            var playerRotateTo = Quaternion.LookRotation(playerLookPosition);

            StopAllCoroutines();
            StartCoroutine(LookTowards(npcRotateTo, playerRotateTo));
            GameManager.instance.uiManager.gameplayTweener.Disable();
            GameManager.instance.uiManager.dialogueUI.StartDialogue(_conversation);
        }

        private IEnumerator LookTowards(Quaternion npcRotateTo, Quaternion playerRotateTo)
        {
            var npcRotation = _npcTransform.rotation;
            var playerRotation = _playerTransform.rotation;
            
            var timeElapsed = 0f;

            while (timeElapsed < _rotateTime)
            {
                _npcTransform.rotation = Quaternion.Slerp(npcRotation, npcRotateTo, timeElapsed / _rotateTime);
                _playerTransform.rotation = Quaternion.Slerp(playerRotation, playerRotateTo, timeElapsed / _rotateTime);
                timeElapsed += Time.deltaTime;
                yield return null;
            }
        }
    }
}
