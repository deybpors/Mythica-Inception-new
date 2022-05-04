using System.Collections;
using System.Collections.Generic;
using _Core.Managers;
using _Core.Player;
using Dialogue_System;
using MyBox;
using Pluggable_AI.Scripts.General;
using Quest_System;
using ToolBox.Serialization;
using UnityEngine;

namespace Assets.Scripts.Dialogue_System
{
    public class NPCAI : GenericAI, IInteractable
    {
        [Foldout("NPC AI Fields", true)]
        private string _saveKey;
        [SerializeField] private bool _savePosition;
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
        private int _currentState;

        void Start()
        {
            GetQuestInConversation();
            CheckQuestDone();

            _saveKey = gameObject.name;
            if (GameManager.instance.saveManager.LoadDataObject(_saveKey, out int state))
            {
                _currentState = state;
            }

            _saveKey += _currentState;


            var interactedKey = _saveKey + nameof(_alreadyInteracted);
            GameManager.instance.saveManager.LoadDataObject(interactedKey, out bool interacted);
            _alreadyInteracted = interacted;
            
            _npcTransform = transform;
            if (_savePosition)
            {
                var positionKey = _saveKey + "Position";
                if (GameManager.instance.saveManager.LoadDataObject(positionKey, out Vector3 position))
                {
                    _npcTransform.position = position;
                }
            }

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
                _playerTransform = GameManager.instance.player.playerTransform;
            }
            catch
            {
                // ignored
            }

            _questGiver = _conversation.lines[_conversation.lines.Length - 1].character;
        }


        public void ResetNPC(Conversation newConversation)
        {
            _conversation = newConversation;
            _questsInConversation.Clear();
            GetQuestInConversation();
            _alreadyInteracted = false;
            _currentState++;
            _saveKey = gameObject.name + _currentState;
            GameManager.instance.saveManager.SaveOtherData(_saveKey, _currentState);
        }

        private void CheckQuestDone()
        {
            var questManager = GameManager.instance.player.playerQuestManager;
            var count = _questsInConversation.Count;
            for (var i = 0; i < count; i++)
            {
                if (!questManager.PlayerHaveQuest(questManager.finishedQuests, _questsInConversation[i],
                        out var accepted)) continue;
                
                gameObject.SetActive(false);
                return;
            }
        }

        private void GetQuestInConversation()
        {
            var choicesCount = _conversation.choices.Length;

            for (var i = 0; i < choicesCount; i++)
            {
                var quest = _conversation.choices[i].quest;
                if (quest == null) continue;

                _questsInConversation.Add(quest);
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
                if (_outline != null)
                {
                    _outline.enabled = false;
                }
                _isInteractable = false;
            }

            if (!GameManager.instance.inputHandler.interact || !_isInteractable) return;
            GameManager.instance.inputHandler.interact = false;
            _isInteractable = false;

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

                GameManager.instance.uiManager.questUI.OpenPanelFromGiver(this, accepted, _questGiver.facePicture);
                GameManager.instance.uiManager.questUI.PlayerInputActivate(false);
                GameManager.instance.gameStateController.TransitionToState(GameManager.instance.dialogueState);
                CheckQuestDone();
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
            GameManager.instance.uiManager.dialogueUI.StartDialogue(_conversation);
            GameManager.instance.uiManager.gameplayTweener.Disable();

            if(!_rotateOnInteract) return;
            GetNPCPlayerToRotateTo(out var npcRotateTo, out var playerRotateTo);
            StopAllCoroutines();
            StartCoroutine(LookTowards(npcRotateTo, playerRotateTo));
            _alreadyInteracted = true;
            var saveKey = _saveKey + nameof(_alreadyInteracted);
            GameManager.instance.saveManager.SaveOtherData(saveKey, _alreadyInteracted);
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

        void OnApplicationQuit()
        {
            if(!_savePosition) return;
            var positionKey = _saveKey + "Position";
            GameManager.instance.saveManager.SaveOtherData(positionKey, _npcTransform.position);
        }

    }
}
