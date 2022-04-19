using System;
using System.Collections.Generic;
using System.Linq;
using _Core.Managers;
using Pluggable_AI.Scripts.States;
using Quest_System;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    [Header("Quest Info Panel")] 
    [SerializeField] private UITweener _infoPanelTweener;
    [SerializeField] private TextMeshProUGUI _questTitle;
    [SerializeField] private TextMeshProUGUI _questDescription;
    [SerializeField] private Image _questGiverImage;
    [SerializeField] private List<QuestRewardsUI> _questRewards;
    [SerializeField] private Sprite _experienceRewardIcon;
    [SerializeField] private Button _acceptButton;
    private GameObject _acceptGameObject;
    private Vector3 _zeroVector = Vector3.zero;

    [Header("Quest Icon Panel")]
    [SerializeField] private List<QuestIconItemUI> _questIconItems;

    public void UpdateQuestIcons()
    {
        var iconItemsCount = _questIconItems.Count;
        var acceptedQuests = GameManager.instance.player.playerQuestManager.activeQuests.Values.ToList();

        for (var i = 0; i < iconItemsCount; i++)
        {
            _questIconItems[i].gameObject.SetActive(true);
            try
            {
                var index = i;
                void OpenQuest() =>
                    OpenPanelFromIcon(acceptedQuests[index], acceptedQuests[index].questGiver.facePicture);
                
                _questIconItems[i].SetupQuestIcon(acceptedQuests[i], OpenQuest);
            }
            catch
            {
                _questIconItems[i].DisableQuestIcon();
            }
        }
    }

    private void OpenQuestInfoPanel(PlayerAcceptedQuest active, Sprite questGiverPicture)
    {
        _questTitle.text = active.quest.title;
        _questGiverImage.sprite = questGiverPicture;
        var rewardsCount = _questRewards.Count;
        
        for (var i = 0; i < rewardsCount; i++)
        {
            _questRewards[i].DisableReward();
            _questRewards[i].gameObject.SetActive(true);
            try
            {
                var icon = active.quest.rewards[i].rewardsType.typeEnumOfReward == RewardTypesEnum.Items
                    ? active.quest.rewards[i].rewardsType
                        .rewardItem.itemIcon
                    : _experienceRewardIcon;
                _questRewards[i].SetupRewardsUI(active.quest.rewards[i].rewardsType.typeEnumOfReward,
                    active.quest.rewards[i].rewardsType.rewardItem, active.quest.rewards[i].value, icon);
            }
            catch
            {
                _questRewards[i].DisableReward();
            }
        }

        _infoPanelTweener.gameObject.SetActive(true);
    }

    private void OpenPanelFromIcon(PlayerAcceptedQuest active, Sprite questGiverPicture)
    {
        if (_acceptGameObject == null)
        {
            _acceptGameObject = _acceptButton.gameObject;
        }
        _acceptGameObject.SetActive(false);
        var description = active.quest.description;
        if (active.completed)
        {
            description += "\n\n<align=\"center\"><color=#b3f47a>This quest is completed. You may go to " + active.questGiver.fullName + " to claim your reward.";
        }
        _questDescription.text = description;
        OpenQuestInfoPanel(active, questGiverPicture);
    }

    public void OpenPanelFromGiver(PlayerAcceptedQuest active, Sprite questGiverPicture)
    {
        _acceptButton.interactable = false;
        if (_acceptGameObject == null)
        {
            _acceptGameObject = _acceptButton.gameObject;
        }
        _acceptGameObject.SetActive(true);
        _questDescription.text = active.quest.description;
        OpenQuestInfoPanel(active, questGiverPicture);
        
        if (!active.completed) return;
        InitiateAcceptButton(active);
    }

    private void InitiateAcceptButton(PlayerAcceptedQuest active)
    {
        _acceptButton.interactable = true;
        _acceptButton.onClick.RemoveAllListeners();
        _acceptButton.onClick.AddListener(() => GameManager.instance.player.playerQuestManager.GetQuestRewards(active));
        _acceptButton.onClick.AddListener(() => GameManager.instance.uiManager.questUI.PlayerInputActivate(true));
        _acceptButton.onClick.AddListener(() => GameManager.instance.gameStateController.TransitionToState(GameManager.instance.gameplayState));
        _acceptButton.onClick.AddListener(() => DisableTweener(_infoPanelTweener));
        _acceptButton.onClick.AddListener(() => GameManager.instance.player.playerQuestManager.RemoveQuestToPlayerAcceptedQuest(active.quest));
        _acceptButton.onClick.AddListener(() => GameManager.instance.uiManager.questUI.UpdateQuestIcons());
        _acceptButton.onClick.AddListener(() => GameManager.instance.audioManager.PlaySFX("Confirmation"));
        for (var i = 0; i < 4; i++)
        {
            var player = GameManager.instance.player;
            var index = i;
            _acceptButton.onClick.AddListener((() => GameManager.instance.uiManager.UpdatePartyUI(player.monsterSlots[index])));
        }
    }

    public void DisableTweener(UITweener tweener)
    {
        tweener.Disable();
    }

    public void PlayerInputActivate(bool toActivate)
    {
        GameManager.instance.inputHandler.movementInput = _zeroVector;
        GameManager.instance.inputHandler.activate = toActivate;
    }

    public void ChangeGameState(State stateToChange)
    {
        GameManager.instance.gameStateController.TransitionToState(stateToChange);
    }
}
