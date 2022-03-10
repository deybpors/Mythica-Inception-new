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
        private int _monsterLvl;
        private bool _activated;
        private Health _healthComponent;
        private Animator _animator;
        private IHaveMonsters _tamer;
        private IHaveMonsters _haveMonster;
        private MonsterTamerAI tamerAI;
        public void ActivateTameValue(int wildMonsterLvl, Health health, IHaveMonsters mon)
        {
            tamerAI = GetComponent<MonsterTamerAI>();
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
        }

        void Update()
        {
            if(!_activated) return;
            
            //TODO: get if there is statusfx in monsterTamer AI
            var newTameValue = GameSettings.TameValue(_monsterLvl, false, _healthComponent.health.currentHealth, _healthComponent.health.maxHealth);

            if (maxTameValue == newTameValue) return;
            if(tameValueBarUI.gameObject.activeInHierarchy) _animator.Play("Change");
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

        public void Tamed()
        {
            var monsterSlots = _tamer.GetMonsterSlots();
            var slotToFill = 99999;
            for(var i = 0; i < monsterSlots.Count; i++)
            {
                if (monsterSlots[i].monster != null) continue;
                slotToFill = i;
                break;
            }

            if (slotToFill >= 4)
            {
                //TODO: store in box or somewhere hehe
            }
            else
            {
                var newSlot = _haveMonster.GetMonsterSlots()[_haveMonster.GetCurrentSlotNumber()];
                _tamer.AddNewMonsterSlot(slotToFill, newSlot);
                if (tamerAI.spawner != null) { tamerAI.spawner.currentNoOfMonsters--; }
                //play animation something screen to celebrate new monster tamed
                //ask for nickname of monster
            }
            
            GameManager.instance.UpdateEnemiesSeePlayer(transform, out var enemyCount);
            
            var player = GameManager.instance.player;
            GameManager.instance.DifficultyUpdateChange("Average Party Level", GameSettings.MonstersAvgLevel(player.monsterSlots));
            
            gameObject.SetActive(false);
        }
        
        private void UpdateEnemiesSeePlayer(Object monsterTransform)
        {
            var enemyCount = GameManager.instance.enemiesSeePlayer.Count;
            for (var i = 0; i < enemyCount; i++)
            {
                var enemy = GameManager.instance.enemiesSeePlayer[i];
                if (monsterTransform != enemy) continue;
                GameManager.instance.enemiesSeePlayer.Remove(enemy);
                break;
            }
        }
    }
}
