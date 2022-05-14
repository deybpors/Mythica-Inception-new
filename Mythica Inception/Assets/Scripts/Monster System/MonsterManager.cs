using System;
using System.Collections.Generic;
using System.Linq;
using _Core.Managers;
using _Core.Others;
using _Core.Player;
using Items_and_Barter_System.Scripts;
using MyBox;
using Skill_System;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;

namespace Monster_System
{
    [RequireComponent(typeof(IHaveMonsters))]
    public class MonsterManager : MonoBehaviour
    {
        [SerializeField] private Transform _charPortraitCam;

        #region Hidden Fields

        [ReadOnly][SerializeField] private bool _activated;
        [ReadOnly][SerializeField] private List<Monster> _monsters;
        [HideInInspector] public bool isPlayer;
        private SkillManager _skillManager;
        private List<GameObject> _monsterGameObjects = new List<GameObject>();
        private IHaveMonsters _haveMonsters;
        private List<Animator> _monsterAnimators = new List<Animator>();
        private GameObject _tamerPrefab;
        private Animator _tamerAnimator;
        private float _timer;
        [ReadOnly] [SerializeField]
        private int _currentMonster;
        private float _tamerTameRadius;
        private Player _player;
        private Dictionary<GameObject, Vector3> _charPortraitAlign = new Dictionary<GameObject, Vector3>();
        private Dictionary<GameObject, Renderer[]> _monsterRenderers = new Dictionary<GameObject, Renderer[]>();
        private Dictionary<GameObject, Outline> _monsterOutlines = new Dictionary<GameObject, Outline>();
        private readonly Vector3 _zero = Vector3.zero;
        [HideInInspector] public Outline currentOutline;

        #endregion
        
        public void ActivateMonsterManager(IHaveMonsters haveMonsters, SkillManager skillManager)
        {
            _currentMonster = -1;
            _haveMonsters = haveMonsters;
            
            //if tamerPrefab is not null, meaning it is a player since only the player has a tamer
            _tamerPrefab = _haveMonsters.GetTamer();
            if (_tamerPrefab != null)
            {
                isPlayer = true;
                _player = GetComponent<Player>();
                _tamerTameRadius = _player.playerSettings.tameRadius;
                _tamerAnimator = _tamerPrefab.GetComponent<Animator>();
                currentOutline = _tamerPrefab.GetComponent<Outline>();
                if (currentOutline != null)
                {
                    _monsterOutlines.Add(_tamerPrefab, currentOutline);
                }
                _player.playerName = _player.playerName.Equals(string.Empty) ? _tamerPrefab.name : _player.playerName;
            }

            _skillManager = skillManager;
            _monsterGameObjects = new List<GameObject>();
            RequestPoolMonstersPrefab();
            GetMonsterAnimators();
            
            _activated = true;
            SwitchToTamer(true);
        }

        public void RequestPoolMonstersPrefab()
        {
            if (_monsterGameObjects.Count > 0)
            {
                foreach (var monsterObj in _monsterGameObjects)
                {
                    if(monsterObj == null) continue;
                    GameManager.instance.pooler.BackToPool(monsterObj);
                }
                _monsterGameObjects.Clear();
            }
            
            _monsters = _haveMonsters.GetMonsters();
            for (var i = 0; i < _monsters.Count; i++)
            {
                if (_monsters[i]==null)
                {
                    _monsterGameObjects.Add(null);
                    continue;
                }
                
                var monsterObj = GameManager.instance.pooler.SpawnFromPool(transform,
                    _monsters[i].monsterName, _monsters[i].monsterPrefab, _zero, Quaternion.identity);
                var outline = monsterObj.GetComponent<Outline>();
                currentOutline = outline;
                _monsterOutlines.Add(monsterObj, outline);
                monsterObj.SetActive(false);

                if (isPlayer)
                {
                    HandleMonsterRenderers(monsterObj);
                }
                _monsterGameObjects.Add(monsterObj);
            }
        }

        private void HandleMonsterRenderers(GameObject monsterObj)
        {
            if (_monsterRenderers.TryGetValue(monsterObj, out var renderers))
            {
                var renderersCount = renderers.Length;
                for (var j = 0; j < renderersCount; j++)
                {
                    renderers[j].gameObject.layer = 11;
                }
                return;
            }

            renderers = monsterObj.GetComponentsInChildren<Renderer>();
            try
            {
                _monsterRenderers.Add(monsterObj, renderers);
                var renderersCount = renderers.Length;
                for (var j = 0; j < renderersCount; j++)
                {
                    renderers[j].gameObject.layer = 11;
                }
            }
            catch
            {
                //ignored
            }
        }

        public void GetMonsterAnimators()
        {
            if (_monsterAnimators.Count > 0)
            {
                _monsterAnimators.Clear();
            }

            for (var i = 0; i < _monsterGameObjects.Count; i++)
            {
                if (_monsterGameObjects[i] == null)
                {
                    _monsterAnimators.Add(null);
                    continue;
                }
                _monsterAnimators.Add(_monsterGameObjects[i].GetComponent<Animator>());
            }
        }
        
        public void SwitchToTamer(bool initialize)
        {
            if(!_activated) return;
            if (_tamerPrefab == null) return;
            _skillManager.Deactivate();
            InactiveAllMonsters();
            _tamerPrefab.SetActive(true);
            _haveMonsters.SetAnimator(_tamerAnimator);
            _skillManager.activated = false;
            _player.unitIndicator.transform.localScale =
                new Vector3(_tamerTameRadius, _tamerTameRadius, _tamerTameRadius);
            _currentMonster = -1;

            if (!initialize)
            {
                _haveMonsters.SpawnSwitchFX();
                GameManager.instance.audioManager.PlaySFX("Switch");
            }

            EvaluateCharPortraitCam(_tamerPrefab);
            _monsterOutlines.TryGetValue(_tamerPrefab, out var outline);
            currentOutline = outline;
            _haveMonsters.ChangeStatsToMonster(_currentMonster);
            
            UpdateGameplayUI(_currentMonster);
        }

        public void SwitchMonster(int slot)
        {
            if(!_activated) return;
            if (_haveMonsters.GetMonsterSlots()[slot].monster == null)
            {
                GameManager.instance.inputHandler.currentMonster = GameManager.instance.inputHandler.previousMonster;
                GameManager.instance.audioManager.PlaySFX("Error");
                GameManager.instance.uiManager.debugConsole.DisplayLogUI("There is no monster in the selected slot.");
                return;    
            }

            _skillManager.Deactivate();
            InactiveAllMonsters();
            _monsterGameObjects[slot].transform.rotation = transform.rotation;
            _monsterGameObjects[slot].SetActive(true);
            _haveMonsters.SetAnimator(_monsterAnimators[slot]);
            _currentMonster = slot;
            _haveMonsters.ChangeMonsterUnitIndicatorRadius(_haveMonsters.GetMonsters()[slot].basicAttackSkill.castRadius);
            _haveMonsters.SpawnSwitchFX();

            _monsterOutlines.TryGetValue(_monsterGameObjects[slot], out var outline);
            currentOutline = outline;
            GameManager.instance.audioManager.PlaySFX("Switch");

            EvaluateCharPortraitCam(_monsterGameObjects[slot]);
            
            _skillManager.ActivateSkillManager(_haveMonsters);
            _haveMonsters.ChangeStatsToMonster(slot);
            
            if (isPlayer)
            {
                UpdateGameplayUI(slot);
            }
        }

        private void EvaluateCharPortraitCam(GameObject characterObject)
        {
            if (_charPortraitCam == null) return;
            
            if (!_charPortraitAlign.TryGetValue(characterObject, out var position))
            {
                var obj = characterObject.GetObjectsOfLayerInChilds(13)[0];
                position = new Vector3(_charPortraitCam.localPosition.x, obj.localPosition.y, _charPortraitCam.localPosition.z);
                _charPortraitAlign.Add(characterObject, position);
            }

            _charPortraitCam.localPosition = position;
        }

        private void InactiveAllMonsters()
        {
            var count = _monsterGameObjects.Count;
            for (var i = 0; i < count; i++)
            {
                var monster = _monsterGameObjects[i];
                if (monster == null) continue;
                monster.SetActive(false);
            }

            _tamerPrefab.SetActive(false);
        }

        private void UpdateGameplayUI(int slot)
        {
            var monsterSlots = _haveMonsters.GetMonsterSlots();

            var monSlots = _haveMonsters.GetMonsterSlots();

            GameManager.instance.uiManager.UpdateItemsUI(this, slot, monSlots);
            GameManager.instance.uiManager.UpdateSkillUI(slot, monSlots);

            if (slot >= 0)
            {
                var monsterSlot = monsterSlots[slot];
                var monsterName = monsterSlot.name == "" ? monsterSlot.monster.name : monsterSlot.name;
                var monsterLevel = GameSettings.Level(monsterSlot.currentExp);
                var maxHealth = GameSettings.Stats(monsterSlot.monster.stats.baseHealth, monsterSlot.stabilityValue, monsterLevel);
                var maxExp = (float) GameSettings.Experience(monsterLevel + 1) - GameSettings.Experience(monsterLevel);
                var currentExp = (float) monsterSlot.currentExp - GameSettings.Experience(monsterLevel);
                GameManager.instance.uiManager.UpdateCharSwitchUI(monsterName, monsterSlot.currentHealth, maxHealth, currentExp, maxExp, slot);
            }
            else
            {
                GameManager.instance.uiManager.UpdateCharSwitchUI(_player.playerName, _player.playerHealth.currentHealth, _player.playerHealth.maxHealth, 0, 1, slot);
            }
        }

        public void UseUsableItems(InventorySlot inventorySlot, int slot)
        {
            if (!inventorySlot.inventoryItem.TryUse(_player)) return;

            inventorySlot.RemoveInSlot(1);

            GameManager.instance.uiManager.UpdateItemsUI(this, slot, _haveMonsters.GetMonsterSlots());
        }

        public void AddMonstersExp(int expToAdd)
        {
            var monsterSlots = _haveMonsters.GetMonsterSlots();
            var count = monsterSlots.Count;
            var toDivide = 0f;
            for (var i = 0; i < count; i++)
            {
                if (monsterSlots[i].monster == null) continue;
                toDivide++;
            }
            var expAdded = (int) Math.Round(expToAdd/toDivide, 0, MidpointRounding.AwayFromZero);

            for (var i = 0; i < count; i++)
            {
                if (monsterSlots[i].monster == null) continue;
                monsterSlots[i].currentExp += expAdded;
            }
        }

        void OnDisable()
        {
            var objs = GetMonsterGameObjects();
            foreach (var obj in objs)
            {
                GameManager.instance.pooler.BackToPool(obj);
            }
        }

        public List<GameObject> GetMonsterGameObjects()
        {
            return _monsterGameObjects;
        }
    }
}
