using System.Collections;
using System.Collections.Generic;
using _Core.Managers;
using Monster_System;
using MyBox;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MonsterTamedUI : MonoBehaviour
{
    struct AnimOutline
    {
        public Animator animator;
        public Outline outline;

        public AnimOutline(Animator animator, Outline outline)
        {
            this.animator = animator;
            this.outline = outline;
        }
    }

    [SerializeField] private Transform _parentTrans;
    [SerializeField] private GameObject _monsterTamedUI;
    [SerializeField] private Button _confirmButton;
    [SerializeField] private GameObject _overlay;
    [SerializeField] private Image _monsterTypeImage;
    [SerializeField] private TMP_InputField _monsterNicknameInput;
    [SerializeField] private TextMeshProUGUI _info;
    [SerializeField] private bool _isPlaying;

    private Monster _monsterDisplayed;
    private GameObject _monsterPrefab;
    private UITweener _uiTweener;
    private UITweener _overlayTweener;
    private readonly Vector3 _zero = Vector3.zero;
    private GameObject _thisGameObject;
    private bool _initializationDone;
    private UIManager _ui;
    private bool _inParty;
    private int _slotNum;
    private bool _outlineEnabled;
    private Dictionary<GameObject, AnimOutline> _animators = new Dictionary<GameObject, AnimOutline>();
    private GameObject _parentGameObject;

    void Awake()
    {
        _parentGameObject = _parentTrans.gameObject;
        LeanTween.rotateAround(_parentGameObject, Vector3.up, 360, 4f).setLoopClamp().setIgnoreTimeScale(true);
        
        if (_initializationDone) return;
        if(GameManager.instance == null) return;

        _ui = GameManager.instance.uiManager;
        _uiTweener = _monsterTamedUI.GetComponent<UITweener>();
        _overlayTweener = _overlay.GetComponent<UITweener>();
        _thisGameObject = gameObject;
        _confirmButton.onClick.RemoveAllListeners();
        _confirmButton.onClick.AddListener(MinimizeChangeToGameplay);
        _initializationDone = true;
    }

    void OnDisable()
    {
        _isPlaying = false;
        _monsterNicknameInput.text = string.Empty;
        HandleAnimator(AnimatorUpdateMode.Normal);
    }

    void Update()
    {
        if(_monsterTamedUI.activeInHierarchy) return;
        _thisGameObject.SetActive(false);
    }

    public bool FanfarePlaying()
    {
        return _isPlaying;
    }

    private void MinimizeChangeToGameplay()
    {
        GameManager.instance.gameStateController.TransitionToState(GameManager.instance.gameplayState);
        _uiTweener.Disable();
        _overlayTweener.Disable();
        HandleNickname();
        GameManager.instance.audioManager.StopSFX(GameManager.instance.audioManager.GetSFX("Fanfare"));
    }

    private void HandleNickname()
    {
        if (_monsterNicknameInput.text != string.Empty)
        {
            var monsterNickname = _monsterNicknameInput.text;
            monsterNickname = monsterNickname.Replace(" ", string.Empty).ToLowerInvariant();
            monsterNickname = char.ToUpperInvariant(monsterNickname[0]) + monsterNickname.Substring(1);
            _monsterNicknameInput.text = monsterNickname;
        }

        if (_inParty)
        {
            var party = GameManager.instance.player.monsterSlots;
            party[_slotNum].name = _monsterNicknameInput.text;
            return;
        }

        var storage = GameManager.instance.player.storageMonsters;
        storage[_slotNum].name = _monsterNicknameInput.text;
    }

    public void PlayFanfare(MonsterSlot newMonsterSlot, bool inParty, int slotNum)
    {
        if (_thisGameObject == null)
        {
            _thisGameObject = gameObject;
        }

        _isPlaying = true;

        GameManager.instance.audioManager.PlaySFX("Fanfare");
        ChangeMonster(newMonsterSlot.monster);
        GameManager.instance.gameStateController.TransitionToState(GameManager.instance.UIState);
        _monsterTamedUI.SetActive(true);
        _overlay.SetActive(true);
        _thisGameObject.SetActive(true);
        HandleText(newMonsterSlot, inParty);
        _inParty = inParty;
        _slotNum = slotNum;
        HandleAnimator(AnimatorUpdateMode.UnscaledTime);
        HandleConfirmButton();
    }

    private void HandleConfirmButton()
    {
        _confirmButton.interactable = false;
        StopAllCoroutines();
        StartCoroutine(DelayAction(5f, (() => _confirmButton.interactable = true)));
    }

    private void HandleAnimator(AnimatorUpdateMode updateMode)
    {
        if (!_animators.TryGetValue(_monsterPrefab, out var animOutline))
        {
            try
            {
                animOutline = new AnimOutline(_monsterPrefab.GetComponent<Animator>(),
                    _monsterPrefab.GetComponent<Outline>());
                _animators.Add(_monsterPrefab, animOutline);
            }
            catch
            {
                //ignored
            }
        }

        if (updateMode == AnimatorUpdateMode.UnscaledTime)
        {
            _outlineEnabled = animOutline.outline.enabled;
            animOutline.outline.enabled = false;
        }
        else
        {
            animOutline.outline.enabled = _outlineEnabled;
        }
        animOutline.animator.updateMode = updateMode;
    }

    IEnumerator DelayAction(float seconds, UnityAction action)
    {
        yield return new WaitForSecondsRealtime(seconds);
        action?.Invoke();
    }

    private void ChangeMonster(Monster newMonster)
    {
        _monsterDisplayed = newMonster;
        if (_monsterPrefab != null)
        {
            GameManager.instance.pooler.BackToPool(_monsterPrefab);
        }

        _monsterPrefab = GameManager.instance.pooler.SpawnFromPool(_parentTrans, 
            _monsterDisplayed.monsterName, 
            _monsterDisplayed.monsterPrefab,
            _zero, Quaternion.identity);
    }

    private void HandleText(MonsterSlot monsterSlot, bool inParty)
    {
        var text = "<b><size=200%>" + monsterSlot.monster.monsterName + "</size></b>\n" +
                   "<size=90%>Type: <b>" + monsterSlot.monster.type + "</b>\n" +
                   "Basic Attack: <b>" + monsterSlot.monster.basicAttackType + "</b>\n" +
                   "Stability Value: <b>" + monsterSlot.stabilityValue.ToString("#.00") + "</b></size>\n" +
                   monsterSlot.monster.description + "\n\n" +
                   "<size=120%><b>Base Stats</b></size>\n" +
                   "<size=90%>HP: <b>" + monsterSlot.monster.stats.baseHealth + "</b>\n" +
                   "Max Lives: <b>" + monsterSlot.monster.stats.maxLives + "</b>\n" +
                   "Physical Attack: <b>" + monsterSlot.monster.stats.physicalAttack + "</b>\n" +
                   "Physical Defense: <b>" + monsterSlot.monster.stats.physicalDefense + "</b>\n" +
                   "Special Attack: <b>" + monsterSlot.monster.stats.specialAttack + "</b>\n" +
                   "Special Defense: <b>" + monsterSlot.monster.stats.specialDefense + "</b>\n" +
                   "Experience Yield: <b>" + monsterSlot.monster.stats.baseExpYield + "</b>\n" +
                   "Tame Resistance: <b>" + monsterSlot.monster.stats.tameResistance + "</b>\n" +
                   "Critical Chance: <b>" + (monsterSlot.monster.stats.criticalChance * 100).ToString("#.00") + "%</b>\n";

        if (!inParty)
        {
            text += "\n<align=\"center\">STORED IN THE MYTHICA STORAGE";
        }

        _info.text = text;

        _monsterTypeImage.sprite = monsterSlot.monster.type switch
        {
            MonsterType.Piercer => _ui.piercer,
            MonsterType.Brawler => _ui.brawler,
            MonsterType.Slasher => _ui.slasher,
            MonsterType.Charger => _ui.charger,
            MonsterType.Emitter => _ui.emitter,
            MonsterType.Keeper => _ui.keeper,
            _ => _monsterTypeImage.sprite
        };
    }
}
