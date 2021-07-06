using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts._Core.Input
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputHandler : MonoBehaviour
    {
        public _Core.Player.Player Player { get; private set; }

        [HideInInspector] public Vector2 movementInput;
        [HideInInspector] public bool dashInput;
        [HideInInspector] public bool attackInput;
        [HideInInspector] public bool firstSkillInput;
        [HideInInspector] public bool secondSkillInput;
        [HideInInspector] public bool thirdSkillInput;
        [HideInInspector] public bool fourthSkillInput;
        [HideInInspector] public bool cancelSkill;
        [HideInInspector] public bool activateSkill;
        [HideInInspector] public int currentMonster;
        [HideInInspector] public int previousMonster;
        [HideInInspector] public bool playerSwitch;

        private bool _canAttack = true;

        void Awake()
        {
            Player = GetComponent<_Core.Player.Player>();
            previousMonster = -1;
            playerSwitch = true;
        }

        #region Move
        public void OnMoveInput(InputAction.CallbackContext context)
        {
            movementInput = context.ReadValue<Vector2>();
        }
        #endregion

        #region Dash

        public void OnDashInput(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                dashInput = true;
            }
        }

        #endregion

        #region Attack
        public void OnAttack(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                Player.unitIndicator.SetActive(true);
                if(!_canAttack) return;
                attackInput = true;
                _canAttack = false;
                StartCoroutine("CanAttack");
            }

            if (context.canceled)
            {
                Player.unitIndicator.SetActive(false);
            }
        }
        
        IEnumerator CanAttack()
        {
            yield return new WaitForSeconds(Player.playerData.attackRate);
            _canAttack = true;
        }
        

        #endregion

        #region Skills
        public void OnFirstSkill(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            
            firstSkillInput = true;
        }

        public void OnSecondSkill(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            
            secondSkillInput = true;
        }

        public void OnThirdSkill(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            thirdSkillInput = true;
        }

        public void OnFourthSkill(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            fourthSkillInput = true;
        }

        public void OnCancelSkill(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            cancelSkill = true;
        }
        
        public void OnActivateSkill(InputAction.CallbackContext context)
        {
            if(!Player.skillManager.targeting) return;

            if (!context.started) return;
            activateSkill = true;
        }
        

        #endregion

        #region MonsterSwitching
        
        public void SwitchTamer(InputAction.CallbackContext context)
        {
            if(!context.started) return;
            SwitchMonster(-1, true);
        }
        public void SwitchMonster1(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            
            SwitchMonster(0, false);
        }
        public void SwitchMonster2(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            
            SwitchMonster(1,false);
        }
        public void SwitchMonster3(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            
            SwitchMonster(2, false);
        }
        public void SwitchMonster4(InputAction.CallbackContext context)
        {
            if (!context.started) return;

            SwitchMonster(3, false);
        }

        private void SwitchMonster(int slot, bool player)
        {
            previousMonster = currentMonster;
            currentMonster = slot;
            playerSwitch = player;
        }

        #endregion
        
    }
}
