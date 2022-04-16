using Items_and_Barter_System.Scripts;

namespace Quest_System.Goals
{
    public class GatherGoal : QuestGoal
    {
        public ItemObject itemToGather;

        public void ItemGathered(PlayerAcceptedQuest acceptedQuest, InventorySlot itemGathered, out int updatedAmount)
        {
            updatedAmount = itemGathered.amountOfItems;

            //checks if any monster is true or if false when monsterToKill and the monsterKilled is true
            if (itemToGather != itemGathered.inventoryItem) return;

            acceptedQuest.currentAmount = itemGathered.amountOfItems;
            updatedAmount = acceptedQuest.currentAmount;
        }
    }
}