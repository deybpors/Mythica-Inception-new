using _Core.Managers;
using Items_and_Barter_System.Scripts;
using UnityEngine;

namespace Quest_System.Goals
{
    [CreateAssetMenu(menuName = "Quest System/Quest Goal/Gather Goal")]
    public class GatherGoal : QuestGoal
    {
        public ItemObject itemToGather;

        public void ItemGathered(PlayerAcceptedQuest acceptedQuest, out int updatedAmount)
        {
            acceptedQuest.currentAmount = GameManager.instance.player.playerInventory.GetTotalAmountItems(itemToGather);
            updatedAmount = acceptedQuest.currentAmount;
        }
    }
}