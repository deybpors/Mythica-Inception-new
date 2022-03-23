using System.Collections;
using _Core.Managers;
using Pluggable_AI.Scripts.States;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Core.Input
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputHandler : MonoBehaviour
    {
        private _Core.Player.Player _player;
        [HideInInspector] public bool activate;
        [HideInInspector] public Vector2 movementInput;
        [HideInInspector] public bool dashInput;
        [HideInInspector] public bool attackInput;
        [HideInInspector] public bool firstSkillInput;
        [HideInInspector] public bool secondSkillInput;
        [HideInInspector] public bool thirdSkillInput;
        [HideInInspector] public bool fourthSkillInput;
        [HideInInspector] public bool cancelSkill;
        [HideInInspector] public bool activateSkill;
        [HideInInspector] public bool interact;
        [SerializeField] private PlayerInput _playerInputSettings;
        public int previousMonster;
        public int currentMonster;
        [HideInInspector] public bool playerSwitchDisabled;

        private Vector3 _zeroVector = new Vector3(0, 0, 0);
        private bool _canAttack = true;

        public void ActivatePlayerInputHandler(Player.Player player)
        {
            previousMonster = -1;
            currentMonster = -1;
            _player = player;
            activate = true;
        }

        public PlayerInput GetPlayerInputSettings()
        {
            return _playerInputSettings;
        }

        #region Move
        public void OnMoveInput(InputAction.CallbackContext context)
        {
            if (!activate) { return; }
            movementInput = context.ReadValue<Vector2>();
        }
        #endregion

        #region Dash

        public void OnDashInput(InputAction.CallbackContext context)
        {
            if(!activate) return;
            if (context.started)
            {
                dashInput = true;
            }
        }

        #endregion

        #region Attack
        public void OnAttack(InputAction.CallbackContext context)
        {
            if(!activate) return;
            if (context.started)
            {
                _player.unitIndicator.SetActive(true);
                if(!_canAttack) return;
                attackInput = true;
                _canAttack = false;
                StartCoroutine("CanAttack");
            }

            if (context.canceled)
            {
                _player.unitIndicator.SetActive(false);
            }
        }
        
        IEnumerator CanAttack()
        {
            yield return new WaitForSeconds(_player.playerData.attackRate);
            _canAttack = true;
        }
        

        #endregion

        #region Skills
        public void OnFirstSkill(InputAction.CallbackContext context)
        {
            if(!activate) return;
            if (!context.started) return;
            
            firstSkillInput = true;
        }

        public void OnSecondSkill(InputAction.CallbackContext context)
        {
            if(!activate) return;
            if (!context.started) return;
            
            secondSkillInput = true;
        }

        public void OnThirdSkill(InputAction.CallbackContext context)
        {
            if(!activate) return;
            if (!context.started) return;
            thirdSkillInput = true;
        }

        public void OnFourthSkill(InputAction.CallbackContext context)
        {
            if(!activate) return;
            if (!context.started) return;
            fourthSkillInput = true;
        }

        public void OnCancelSkill(InputAction.CallbackContext context)
        {
            if(!activate) return;
            if (!context.started) return;
            cancelSkill = true;
        }
        
        public void OnActivateSkill(InputAction.CallbackContext context)
        {
            if(!activate) return;
            if(!_player.skillManager.targeting) return;

            if (!context.started) return;
            activateSkill = true;
        }
        

        #endregion

        #region MonsterSwitching
        
        public void SwitchTamer(InputAction.CallbackContext context)
        {
            if(!activate) return;
            if(playerSwitchDisabled) return;
            if(!context.started) return;
            Switch(-1);
        }
        public void SwitchMonster1(InputAction.CallbackContext context)
        {
            if(!activate) return;
            if (!context.started) return;
            
            Switch(0);
        }
        public void SwitchMonster2(InputAction.CallbackContext context)
        {
            if(!activate) return;
            if (!context.started) return;
            
            Switch(1);
        }
        public void SwitchMonster3(InputAction.CallbackContext context)
        {
            if(!activate) return;
            if (!context.started) return;
            
            Switch(2);
        }
        public void SwitchMonster4(InputAction.CallbackContext context)
        {
            if(!activate) return;
            if (!context.started) return;

            Switch(3);
        }

        private void Switch(int slot)
        {
            if(slot == currentMonster) return;
            var tempPrev = previousMonster;
            previousMonster = currentMonster;
            currentMonster = slot;

            if (_player.SwitchMonster(slot, out var message)) return;

            if (!message.Equals(string.Empty))
            {
                Debug.Log(message);
                GameManager.instance.uiManager.debugConsole.DisplayLogUI(message);
            }

            currentMonster = previousMonster;
            previousMonster = tempPrev;
        }

        #endregion

        #region Interaction

        public void Interact(InputAction.CallbackContext context)
        {
            if (!activate) return;
            if (!context.started) return;
            if(currentMonster >= 0) return;
            interact = true;
        }

        public void Select(InputAction.CallbackContext context)
        {
            if(!activate) return;
            if(!context.started) return;
            if(_player.GetCurrentSlotNumber() >= 0) return;
            if (_player.selectionManager.interactables.Count <= 0) return;
            
            attackInput = false;
            _player.selectionManager.Select();
            _player.unitIndicator.SetActive(false);
            movementInput = _zeroVector;
        }

        #endregion

        #region UI

        public void OnNextLineDialogue(InputAction.CallbackContext context)
        {
            if (!activate) return;
            if (!context.started) return;
            var dialogueUi = GameManager.instance.uiManager.dialogueUI;

            if (dialogueUi.TextJuicerPlaying())
            {
                dialogueUi.CompleteTextJuicer();
            }
            else
            {
                if (dialogueUi.IsEnd())
                {
                    if (dialogueUi.CurrentConversationHasChoice()) return;
                    
                    dialogueUi.mainDialogueTweener.Disable();
                    OnEnterGameplay(context);
                    return;
                }

                dialogueUi.ContinueExistingDialogue();
            }
        }

        public void OnEnterSettings(InputAction.CallbackContext context)
        {
            if (!activate) return;
            if (!context.started) return;

            GameManager.instance.gameStateController.TransitionToState(GameManager.instance.UIState);
            _playerInputSettings.SwitchCurrentActionMap("UI");
            GameManager.instance.uiManager.gameplayTweener.Disable();
        }

        public void OnEnterGameplay(InputAction.CallbackContext context)
        {
            if (!activate) return;
            if (!context.started) return;

            EnterGameplay();
        }

        public void EnterGameplay()
        {
            GameManager.instance.gameStateController.TransitionToState(GameManager.instance.gameplayState);
            GameManager.instance.uiManager.gameplayUICanvas.SetActive(true);
            GameManager.instance.uiManager.tooltip.tooltipTweener.gameObject.SetActive(false);
            _playerInputSettings.SwitchCurrentActionMap("Gameplay");
        }

        #endregion
    }
}
