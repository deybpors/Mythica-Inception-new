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
        private string _saveKey;
        [SerializeField] private bool _savePosition;
        [SerializeField] private bool _randomName;
        [SerializeField] private Conversation _conversation;
        [SerializeField] private bool _repeatConversation = true;
        [SerializeField] private float _rotateTime = .25f;
        [SerializeField] private float _interactableDistance = 1f;
        [SerializeField] private bool _rotateOnInteract = true;
        [ConditionalField(nameof(_rotateOnInteract))]
        [DefinedValues("Both", "NPC", "Player")]
        [SerializeField] private string _rotation;


        private Outline _outline;
        [ReadOnly] [SerializeField] private bool _isInteractable;
        private Transform _playerTransform;
        private Transform _npcTransform;
        private bool _alreadyInteracted;
        private List<Quest> _questsInConversation = new List<Quest>();
        private Character _questGiver;
        private int _currentState;
        private string _npcName;

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


            if (_randomName)
            {
                HandleRandomNPC();
            }


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

        private void HandleRandomNPC()
        {
            if (!GameManager.instance.saveManager.LoadDataObject(_saveKey, out string npcName))
            {
                npcName = GameManager.instance.databaseManager.GetRandomNPCName();
                GameManager.instance.saveManager.SaveOtherData(_saveKey, npcName);
            }

            _npcName = npcName;

            var linesLength = _conversation.lines.Length;
            var newLines = new Line[linesLength];

            for (var i = 0; i < linesLength; i++)
            {
                newLines[i].emotion = _conversation.lines[i].emotion;
                newLines[i].speakerDirection = _conversation.lines[i].speakerDirection;
                newLines[i].text = _conversation.lines[i].text;

                if (_conversation.lines[i].character == null) continue;

                var conversationChar = _conversation.lines[i].character;
                var newChar = ScriptableObject.CreateInstance<Character>();
                newChar.moods = conversationChar.moods;
                newChar.dialoguePitch = conversationChar.dialoguePitch;
                newChar.facePicture = conversationChar.facePicture;
                newChar.sexOfCharacter = conversationChar.sexOfCharacter;
                newChar.fullName = _npcName;
                newLines[i].character = newChar;
            }

            var newConversation = ScriptableObject.CreateInstance<Conversation>();
            name = _npcName;
            hideFlags = HideFlags.None;
            newConversation.ID = null;
            newConversation.lines = newLines;
            newConversation.choices = _conversation.choices;

            _conversation = newConversation;
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
                        out _)) continue;
                
                gameObject.SetActive(false);
                return;
            }
        }

        void OnDisable()
        {
            var player = GameManager.instance.player;
            player.UnsubscribeInteractable(this);
        }

        private void GetQuestInConversation()
        {
            if (_conversation.choices == null)
            {
                return;
            }
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
                player.SubscribeInteractable(this);

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
                player.UnsubscribeInteractable(this);
            }

            if (!GameManager.instance.inputHandler.interact || !_isInteractable) return;
            if (GameManager.instance.enemiesSeePlayer.Count > 0)
            {
                GameManager.instance.uiManager.debugConsole.DisplayLogUI("You can't interact while a mythica sees you.");
                return;
            }
            GameManager.instance.inputHandler.interact = false;
            _isInteractable = false;
            player.UnsubscribeInteractable(this);
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
            var playerLookPosition = _npcTransform.position - _playerTransform.position;
            npcLookPosition.y = 0;
            playerLookPosition.y = 0;

            npcRotateTo = Quaternion.LookRotation(npcLookPosition);
            playerRotateTo = Quaternion.LookRotation(playerLookPosition);
        }

        private IEnumerator LookTowards(Quaternion npcRotateTo, Quaternion playerRotateTo)
        {
            var npcRotation = _npcTransform.rotation;
            var playerRotation = _playerTransform.rotation;
            
            var timeElapsed = 0f;

            switch (_rotation)
            {
                case "Player":
                    npcRotateTo = npcRotation;
                    break;
                case "NPC":
                    playerRotateTo = playerRotation;
                    break;
            }

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
