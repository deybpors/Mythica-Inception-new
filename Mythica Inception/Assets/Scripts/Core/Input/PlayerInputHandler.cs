using System.Collections;
using Assets.Scripts.Core.Player_FSM;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Core.Player.Input
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputHandler : MonoBehaviour
    {
        public Player Player { get; private set; }

        [HideInInspector] public Vector2 movementInput;
        [HideInInspector] public bool dashInput;
        [HideInInspector] public bool attackInput;
        [HideInInspector] public bool canAttack = true;
        [HideInInspector] public bool firstSkillInput;
        [HideInInspector] public bool secondSkillInput;
        [HideInInspector] public bool thirdSkillInput;
        [HideInInspector] public bool fourthSkillInput;
        [HideInInspector] public bool cancelSkill;
        [HideInInspector] public bool activateSkill;

        void Awake()
        {
            Player = GetComponent<Player>();
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
            if(!canAttack) return;
            
            attackInput = true;
            canAttack = false;
            StartCoroutine("CanAttack");
        }
        
        IEnumerator CanAttack()
        {
            yield return new WaitForSeconds(Player.playerData.attackRate);
            canAttack = true;
        }
        

        #endregion
        

        public void OnFirstSkill(InputAction.CallbackContext context)
        {
            if(context.started)
            {
                firstSkillInput = true;
            }
        }

        public void OnSecondSkill(InputAction.CallbackContext context)
        {
            if(context.started)
            {
                secondSkillInput = true;
            }
        }

        public void OnThirdSkill(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                thirdSkillInput = true;
            }
        }

        public void OnFourthSkill(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                fourthSkillInput = true;
            }
        }

        public void OnCancelSkill(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                cancelSkill = true;
            }
        }
        
        public void OnActivateSkill(InputAction.CallbackContext context)
        {
            if(!Player.skillManager.targeting) return;
            
            if (context.started)
            {
                activateSkill = true;
            }
        }
    }
}
