using _Core.Managers;
using _Core.Others;
using Pluggable_AI.Scripts.General;
using UI;
using UnityEngine;

namespace Monster_System
{
    public class TameValue : MonoBehaviour, ITameable
    {
        public int maxTameValue;
        public int currentTameValue;
        [HideInInspector] public ProgressBarUI tameValueBarUI;
        private GameObject _tameValueBar;
        private int _monsterLvl;
        private bool _activated;
        private Health _healthComponent;
        private Animator _animator;
        private IHaveMonsters _tamer;
        private IHaveMonsters _haveMonster;
        private MonsterTamerAI ai;
        private Transform _thisTransform;

        public void ActivateTameValue(int wildMonsterLvl, Health health, IHaveMonsters mon)
        {
            ai = GetComponent<MonsterTamerAI>();
            _animator = tameValueBarUI.GetComponent<Animator>();
            _monsterLvl = wildMonsterLvl;
            _healthComponent = health;
            maxTameValue =
                GameSettings.TameValue(
                    wildMonsterLvl, 
                    false, 
                    _healthComponent.health.currentHealth, 
                    _healthComponent.health.maxHealth
                    );
            _haveMonster = mon;
            currentTameValue = 0;
            tameValueBarUI.maxValue = maxTameValue;
            tameValueBarUI.currentValue = currentTameValue;
            _activated = true;
            _thisTransform = transform;
            _tameValueBar = tameValueBarUI.gameObject;
        }

        void Update()
        {
            if(!_activated) return;
            
            var newTameValue = GameSettings.TameValue(_monsterLvl, false, _healthComponent.health.currentHealth, _healthComponent.health.maxHealth);

            if (maxTameValue == newTameValue) return;
            if(_tameValueBar.activeInHierarchy) _animator.Play("Change");
            maxTameValue = newTameValue;
        }

        public void AddCurrentTameValue(int tameBeamValue, IHaveMonsters tamer)
        {
            if (!tameValueBarUI.gameObject.activeInHierarchy) { tameValueBarUI.gameObject.SetActive(true); }
            
            _tamer ??= tamer;
            
            currentTameValue += tameBeamValue;
            tameValueBarUI.currentValue = currentTameValue;
            
            if (currentTameValue >= maxTameValue)
            {
                Tamed();
            }
        }

        private void Tamed()
        {
            var monsterSlots = _tamer.GetMonsterSlots();
            var slotToFill = 99999;
            for(var i = 0; i < monsterSlots.Count; i++)
            {
                if (monsterSlots[i].monster != null) continue;
                slotToFill = i;
                break;
            }
            var newSlot = _haveMonster.GetMonsterSlots()[_haveMonster.GetCurrentSlotNumber()];
            
            if (slotToFill >= 4)
            {
                _tamer.AddNewMonsterSlotToStorage(newSlot, out var slotNumber);
                GameManager.instance.uiManager.monsterTamedUi.PlayFanfare(newSlot, false, slotNumber);
            }
            else
            {
                _tamer.AddNewMonsterSlotToParty(slotToFill, newSlot);
                GameManager.instance.uiManager.monsterTamedUi.PlayFanfare(newSlot, true, slotToFill);
            }

            if (ai.spawner != null)
            {
                ai.spawner.currentNoOfMonsters--;
            }

            GameManager.instance.UpdateEnemiesSeePlayer(ai, out var enemyCount);

            if (enemyCount <= 0)
            {
                GameManager.instance.DifficultyUpdateAdd("Failed Encounters", -1);
                if (currentTameValue > 0)
                {
                    GameManager.instance.DifficultyUpdateAdd("Failed Tame Attempts", -1);
                }
            }

            GameManager.instance.activeEnemies.Remove(_thisTransform);
            gameObject.SetActive(false);
        }
    }
}
