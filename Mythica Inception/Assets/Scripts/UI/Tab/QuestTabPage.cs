using System.Collections.Generic;
using System.Linq;
using _Core.Managers;
using Assets.Scripts.UI;
using MyBox;
using Quest_System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestTabPage : TabPage
{
    [Foldout("For Main Quest", true)]
    [InitializationField] [SerializeField] private GameObject _questSlot;
    [InitializationField] [SerializeField] private RectTransform _questSlotsParent;
    [InitializationField] [SerializeField] private ScrollRect _questScrollRect;
    [InitializationField] [SerializeField] private Sprite _finished;
    [InitializationField] [SerializeField] private Sprite _active;
    [InitializationField] [SerializeField] private Sprite _questLogo;


    [Foldout("For Other Info", true)]
    [InitializationField][SerializeField] private GameObject[] _patterns;
    [InitializationField][SerializeField] private TextMeshProUGUI _description;
    [InitializationField][SerializeField] private Image _logoImage;
    
    private PlayerAcceptedQuest _selectedQuest;
    private RectTransform _scrollRectTransform;
    private Dictionary<RectTransform, QuestSlot> _questSlots = new Dictionary<RectTransform, QuestSlot>();
    private Dictionary<string, PlayerAcceptedQuest> _totalQuests = new Dictionary<string, PlayerAcceptedQuest>();
    private Color32 _yellow = new Color32(255, 239, 125, 255);
    private Color32 _green = new Color32(179, 244, 122, 255);

    private struct QuestSlot
    {
        private Button _button;
        private TextMeshProUGUI _title;
        private Image _icon;
        private PlayerAcceptedQuest _activeQuest;

        public Button button => _button;
        public TextMeshProUGUI title => _title;
        public Image icon => _icon;
        public PlayerAcceptedQuest activeQuest => _activeQuest;


        public QuestSlot(TextMeshProUGUI title, Image icon, Button button, PlayerAcceptedQuest activeQuest)
        {
            _title = title;
            _icon = icon;
            _button = button;
            _activeQuest = activeQuest;
        }
    }

    protected override void OnActive()
    {
        Initialize();
        
        if(_questSlots.Count <= 0) return;
        _questSlots.Values.ToList()[0].button.onClick.Invoke();

        if (_scrollRectTransform == null)
        {
            _scrollRectTransform = _questScrollRect.GetComponent<RectTransform>();
        }
        CheckUIOrder();
    }

    private void CheckUIOrder()
    {
        if (_questSlotsParent.sizeDelta.y <= _scrollRectTransform.sizeDelta.y)
        {
            var newPos = _questSlotsParent.anchoredPosition;
            newPos.y = 0;
            _questSlotsParent.anchoredPosition = newPos;
        }
        else
        {
            _questScrollRect.verticalScrollbar.value = 1;
        }
    }


    private void Initialize()
    {
        _totalQuests = GameManager.instance.player.playerQuestManager.GetTotalQuests();
        var totalList = _totalQuests.Values.ToList();
        var questSlotList = _questSlots.Values.ToList();

        var totalQuestCount = _totalQuests.Count;
        var currentQuestCount = _questSlots.Count;

        for (var i = 0; i < totalQuestCount; i++)
        {
            if (i >= currentQuestCount)
            {
                var newQuestSlot = Instantiate(_questSlot, _questSlotsParent).GetComponent<RectTransform>();
                _questSlots.Add(newQuestSlot,
                    new QuestSlot(
                        newQuestSlot.GetComponentInChildren<TextMeshProUGUI>(),
                        newQuestSlot.GetChild(1).GetComponent<Image>(),
                        newQuestSlot.GetComponent<Button>(),
                        totalList[i]));
                questSlotList = _questSlots.Values.ToList();
            }

            questSlotList[i].title.text = questSlotList[i].activeQuest.quest.title;

            var questManager = GameManager.instance.player.playerQuestManager;

            if (questManager.PlayerHaveQuest(questManager.finishedQuests, questSlotList[i].activeQuest.quest))
            {
                questSlotList[i].icon.sprite = _finished;
                questSlotList[i].icon.color = _green;
            }
            else
            {
                questSlotList[i].icon.sprite = _active;
                questSlotList[i].icon.color = _yellow;
            }

            questSlotList[i].button.onClick.RemoveAllListeners();
            var index = i;
            questSlotList[i].button.onClick.AddListener(() => ChangeOtherInfo(questSlotList[index].activeQuest));
        }
    }

    private void ChangeOtherInfo(PlayerAcceptedQuest selectedQuest)
    {
        _selectedQuest = selectedQuest;

        foreach (var pattern in _patterns)
        {
            pattern.SetActive(true);
        }

        var text = "<align=\"center\"><b><size=200%>" + selectedQuest.quest.title + "</size></b>\n" + selectedQuest.quest.description;
        _description.text = text;
        _logoImage.sprite = _questLogo;
        _logoImage.color = _yellow;
    }

    void OnDisable()
    {
        foreach (var pattern in _patterns)
        {
            pattern.SetActive(false);
        }
    }
}
