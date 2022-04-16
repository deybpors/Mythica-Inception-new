using Items_and_Barter_System.Scripts;
using Quest_System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestRewardsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _amountText;
    [SerializeField] private Image _rewardIcon;
    [SerializeField] private TooltipTrigger _tooltipTrigger;

    public void SetupRewardsUI(RewardTypesEnum typeEnum, ItemObject item, int amount, Sprite rewardSprite)
    {
        _amountText.text = amount.ToString();
        _rewardIcon.sprite = rewardSprite;
        if (typeEnum == RewardTypesEnum.Items)
        {
            _tooltipTrigger.SetTitleContent(item.itemName, item.itemDescription);
        }
        else
        {
            _tooltipTrigger.SetTitleContent( amount + " Experience Points", amount + " EXP will be given to you as a reward. This will be divided to your party.");
        }
    }

    public void DisableReward()
    {
        gameObject.SetActive(false);
    }
}
