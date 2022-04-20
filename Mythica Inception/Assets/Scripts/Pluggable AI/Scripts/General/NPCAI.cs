using System.Collections;
using System.Collections.Generic;
using _Core.Managers;
using _Core.Player;
using Dialogue_System;
using MyBox;
using Pluggable_AI.Scripts.General;
using Quest_System;
using UnityEngine;

namespace Assets.Scripts.Dialogue_System
{
    public class NPCAI : GenericAI, IInteractable
    {
        [Foldout("NPC AI Fields", true)]
        [SerializeField] private Conversation _conversation;
        [SerializeField] private bool _repeatConversation = true;
        [SerializeField] private float _rotateTime = .25f;
        [SerializeField] private float _interactableDistance = 1f;
        [SerializeField] private bool _rotateOnInteract = true;

        private Outline _outline;
        [ReadOnly] [SerializeField] private bool _isInteractable;
        private Transform _playerTransform;
        private Transform _npcTransform;
        private bool _alreadyInteracted;
        private List<Quest> _questsInConversation = new List<Quest>();
        private Character _questGiver;

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
            GetQuestInConversation();
            _questGiver = _conversation.lines[_conversation.lines.Length - 1].character;
        }

        private void GetQuestInConversation()
        {
            var choicesCount = _conversation.choices.Length;

            for (var i = 0; i < choicesCount; i++)
            {
                if (_conversation.choices[i].quest == null) continue;
                
                try
                {
                    _questsInConversation.Add(_conversation.choices[i].quest);
                }
                catch
                {
                    //ignored
                }
            }
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

            if (!GameManager.instance.inputHandler.interact || !_isInteractable) return;
            GameManager.instance.inputHandler.interact = false;

            Interact(player);
        }

        private void HandleQuestInConversation(Player player)
        {
            var count = _questsInConversation.Count;
            for (var i = 0; i < count; i++)
            {
                if (!player.playerQuestManager.PlayerHaveQuest(
                        player.playerQuestManager.activeQuests,
                        _questsInConversation[i], out var accepted)) continue;

                GameManager.instance.uiManager.questUI.OpenPanelFromGiver(accepted, _questGiver.facePicture);
                GameManager.instance.uiManager.questUI.PlayerInputActivate(false);
                GameManager.instance.gameStateController.TransitionToState(GameManager.instance.dialogueState);
                GetNPCPlayerToRotateTo(out var npcRotate, out var playerRotate);
                StopAllCoroutines();
                StartCoroutine(LookTowards(npcRotate, playerRotate));
                break;
            }
        }

        public void Interact(Player player)
        {
            if (!_repeatConversation && _alreadyInteracted)
            {
                HandleQuestInConversation(player);
                return;
            }


            if(_conversation == null) return;

            GameManager.instance.gameStateController.TransitionToState(GameManager.instance.dialogueState);
            GameManager.instance.inputHandler.SwitchActionMap("Dialogue");

            GetNPCPlayerToRotateTo(out var npcRotateTo, out var playerRotateTo);

            if(!_rotateOnInteract) return;

            StopAllCoroutines();
            StartCoroutine(LookTowards(npcRotateTo, playerRotateTo));
            GameManager.instance.uiManager.gameplayTweener.Disable();
            GameManager.instance.uiManager.dialogueUI.StartDialogue(_conversation);
            _alreadyInteracted = true;
        }

        private void GetNPCPlayerToRotateTo(out Quaternion npcRotateTo, out Quaternion playerRotateTo)
        {
            var npcLookPosition = _playerTransform.position - _npcTransform.position;
            npcLookPosition.y = 0;
            var playerLookPosition = _npcTransform.position - _playerTransform.position;
            playerLookPosition.y = 0;

            npcRotateTo = Quaternion.LookRotation(npcLookPosition);
            playerRotateTo = Quaternion.LookRotation(playerLookPosition);
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
