using Assets.Scripts._Core;
using Assets.Scripts.UI;
using UnityEngine;

namespace Assets.Scripts.Monster_System
{
    public class TameValue : MonoBehaviour, ITameable
    {
        public int maxTameValue;
        public int currentTameValue;
        [HideInInspector] public ProgressBarUI tameValueBarUI;
        private int _monsterLvl;
        private bool _activated;
        private Health _healthComponent;
        private Animator _animator;

        public void ActivateTameValue(int wildMonsterLvl, Health health)
        {
            _animator = tameValueBarUI.GetComponent<Animator>();
            _monsterLvl = wildMonsterLvl;
            _healthComponent = health;
            maxTameValue =
                GameCalculations.TameValue(
                    wildMonsterLvl, 
                    false, 
                    _healthComponent.health.currentHealth, 
                    _healthComponent.health.maxHealth
                    );
            currentTameValue = 0;
            tameValueBarUI.maxValue = maxTameValue;
            tameValueBarUI.currentValue = currentTameValue;
            _activated = true;
        }

        void Update()
        {
            if(!_activated) return;
            
            //TODO: get if there is statusfx in monsterTamer AI
            var newTameValue = GameCalculations.TameValue(_monsterLvl, false, _healthComponent.health.currentHealth, _healthComponent.health.maxHealth);

            if (maxTameValue == newTameValue) return;
            if(tameValueBarUI.gameObject.activeInHierarchy) _animator.Play("Change");
            maxTameValue = newTameValue;
        }

        public void AddCurrentTameValue(int tameBeamValue)
        {
            if (!tameValueBarUI.gameObject.activeInHierarchy) { tameValueBarUI.gameObject.SetActive(true); }
            
            currentTameValue += tameBeamValue;
            tameValueBarUI.currentValue = currentTameValue;
            
            if (currentTameValue >= maxTameValue)
            {
                Tamed();
            }
        }

        public void Tamed()
        {
            //TODO: make this monster captured
        }
    }
}
