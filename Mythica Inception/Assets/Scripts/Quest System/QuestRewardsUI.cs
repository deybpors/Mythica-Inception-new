using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestRewardsUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _amountText;
    [SerializeField] private Image _rewardIcon;

    public void SetupRewardsUI(string amountText, Sprite rewardSprite)
    {
        _amountText.text = amountText;
        _rewardIcon.sprite = rewardSprite;
    }

    public void DisableReward()
    {
        gameObject.SetActive(false);
    }
}
