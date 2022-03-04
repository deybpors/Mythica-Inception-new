using _Core.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Quest_System
{
    public class QuestGiver : MonoBehaviour
    {
        public Quest questToGive;
        public GameObject rewardUIObject;
        public void OpenQuestWindow(bool accepted)
        {
            GameManager.instance.uiManager.questAccept.text = "Accept";
            GameManager.instance.uiManager.questDecline.text = "Decline";
            GameManager.instance.uiManager.questAccept.alpha = 1;
            var acceptButton = GameManager.instance.uiManager.questAccept.GetComponent<Button>();
            var buttonInteractable = true;

            if (accepted)
            {
                GameManager.instance.uiManager.questAccept.text = "Finish";
                GameManager.instance.uiManager.questDecline.text = "Close";
                buttonInteractable = GameManager.instance.player.playerQuestManager.IsQuestSucceeded(questToGive);
                GameManager.instance.uiManager.questAccept.alpha = .5f;
            }
            
            GameManager.instance.uiManager.questTitle.text = questToGive.title;
            GameManager.instance.uiManager.questDescription.text = questToGive.description;
            var rewardParentCount = GameManager.instance.uiManager.questReward.childCount;
            var rewardCount = questToGive.rewards.Count;
            for (var i = 0; i < rewardCount; i++)
            {
                AddRewardsUI(rewardParentCount, i);
            }

            GameManager.instance.questManager.questSelected = questToGive;
            GameManager.instance.uiManager.questUICanvas.SetActive(true);
            acceptButton.interactable = buttonInteractable;
        }

        private void AddRewardsUI(int rewardParentCount, int rewardNumber)
        {
            GameObject rewardObj;
            if (rewardNumber >= rewardParentCount)
            {
                rewardObj = Instantiate(rewardUIObject, GameManager.instance.uiManager.questReward);
            }
            else
            {
                rewardObj = GameManager.instance.uiManager.questReward.GetChild(rewardNumber).gameObject;
            }
            
            var rewardComp = rewardObj.GetComponent<QuestRewardUI>();
            if (questToGive.rewards[rewardNumber].rewardsType.rewardType == RewardTypes.items)
            {
                questToGive.rewards[rewardNumber].rewardsType.icon = questToGive.rewards[rewardNumber].rewardsType.rewardItem.itemIcon;
            }

            rewardComp.ChangeRewardInfo(questToGive.rewards[rewardNumber].rewardsType.icon, questToGive.rewards[rewardNumber].rewardsType.rewardName,
                questToGive.rewards[rewardNumber].value.ToString());
        }
    }
}
