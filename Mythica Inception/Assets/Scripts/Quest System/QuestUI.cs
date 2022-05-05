using System.Collections.Generic;
using System.Linq;
using _Core.Managers;
using Assets.Scripts.Dialogue_System;
using Pluggable_AI.Scripts.States;
using Quest_System;
using Quest_System.Goals;
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

    public void OpenPanelFromIcon(PlayerAcceptedQuest active, Sprite questGiverPicture)
    {
        if (_acceptGameObject == null)
        {
            _acceptGameObject = _acceptButton.gameObject;
        }
        _acceptGameObject.SetActive(false);
        var description = active.quest.description;
        GameManager.instance.questManager.UpdateGatherQuest();
        if (active.completed)
        {
            description += "\n<align=\"center\"><color=#b3f47a>This quest is completed. You may go to " + active.questGiver.fullName + " to claim your reward.";
        }
        else
        {
            var progress = ((float) active.currentAmount / active.quest.goal.requiredAmount) * 100;
            description += "\n<align=\"center\">Progress: <color=#f48989>" + progress.ToString("00.00") + "%";
        }
        _questDescription.text = description;
        OpenQuestInfoPanel(active, questGiverPicture);
    }

    public void OpenPanelFromGiver(NPCAI ai, PlayerAcceptedQuest active, Sprite questGiverPicture)
    {
        _acceptButton.interactable = false;
        if (_acceptGameObject == null)
        {
            _acceptGameObject = _acceptButton.gameObject;
        }
        _acceptGameObject.SetActive(true);

        var description = active.quest.description;
        GameManager.instance.questManager.UpdateGatherQuest();
        if (active.completed)
        {
            description += "\n<align=\"center\"><color=#b3f47a>This quest is completed. You may accept the quest to get the rewards.";
        }
        else
        {
            var progress = ((float)active.currentAmount / active.quest.goal.requiredAmount) * 100;
            description += "\n<align=\"center\">Progress: <color=#f48989>" + progress.ToString("00.00") + "%";
        }

        _questDescription.text = description;
        OpenQuestInfoPanel(active, questGiverPicture);
        
        if (!active.completed) return;
        InitiateAcceptButton(ai, active);
    }

    private void InitiateAcceptButton(NPCAI ai, PlayerAcceptedQuest active)
    {
        _acceptButton.interactable = true;
        _acceptButton.onClick.RemoveAllListeners();
        var player = GameManager.instance.player;
        UnityAction actions = () => player.playerQuestManager.GetQuestRewards(active);
        actions += () => GameManager.instance.uiManager.questUI.PlayerInputActivate(true);
        actions += () => GameManager.instance.gameStateController.TransitionToState(GameManager.instance.gameplayState);
        actions += () => DisableTweener(_infoPanelTweener);
        actions += () => player.playerQuestManager.RemoveQuestToPlayerAcceptedQuest(active.quest);
        actions += () => GameManager.instance.uiManager.questUI.UpdateQuestIcons();
        actions += () => GameManager.instance.audioManager.PlaySFX("Confirmation");
        actions += () => GameManager.instance.uiManager.UpdateItemsUI(player.monsterManager, -1, player.monsterSlots);

        if (active.quest.goal is GatherGoal goal)
        {
            player.playerInventory.RemoveItemInInventory(goal.itemToGather, goal.requiredAmount);
        }

        if (active.quest.successConversation != null)
        {
            actions += () => ai.ResetNPC(active.quest.successConversation);
            actions += () => GameManager.instance.inputHandler.interact = true;
        }
        _acceptButton.onClick.AddListener(actions);
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
