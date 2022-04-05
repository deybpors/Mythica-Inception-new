using System.Collections;
using _Core.Managers;
using _Core.Player;
using Dialogue_System;
using MyBox;
using Pluggable_AI.Scripts.General;
using UnityEngine;

namespace Assets.Scripts.Dialogue_System
{
    public class NPCAI : GenericAI, IInteractable
    {
        [Foldout("NPC AI Fields", true)]
        [SerializeField] private Conversation _conversation;
        [SerializeField] private float _rotateTime = .25f;
        [SerializeField] private float _interactableDistance = 1f;
        [SerializeField] private bool _rotateOnInteract = true;

        private Outline _outline;
        private bool _isInteractable;
        private Transform _playerTransform;
        private Transform _npcTransform;

        void Start()
        {
            currentAnimator = GetComponentInChildren<Animator>();

            if (_conversation == null)
            {
                stateController.active = false;
                enabled = false;
                return;
            }

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
            if(GameManager.instance == null) return;

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
            if (distanceToPlayer <= _interactableDistance && GameManager.instance.inputHandler.currentMonster < 0)
            {
                _isInteractable = true;

                if (_outline != null)
                {
                    _outline.enabled = true;
                    _outline.OutlineMode = Outline.Mode.OutlineVisible;
                }
            }
            else
            {
                _outline.enabled = false;
                _isInteractable = false;
            }

            if (!GameManager.instance.inputHandler.interact) return;
            
            GameManager.instance.inputHandler.interact = false;
            Interact(player);
        }

        public void Interact(Player player)
        {
            if(!_isInteractable) return;
            if(_conversation == null) return;

            GameManager.instance.gameStateController.TransitionToState(GameManager.instance.dialogueState);
            GameManager.instance.inputHandler.SwitchActionMap("Dialogue");

            var npcLookPosition = _playerTransform.position - _npcTransform.position;
            npcLookPosition.y = 0;
            var playerLookPosition = _npcTransform.position - _playerTransform.position;
            playerLookPosition.y = 0;

            var npcRotateTo = Quaternion.LookRotation(npcLookPosition);
            var playerRotateTo = Quaternion.LookRotation(playerLookPosition);

            if(!_rotateOnInteract) return;

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
