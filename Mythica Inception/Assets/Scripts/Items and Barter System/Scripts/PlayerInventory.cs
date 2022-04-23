using System.Collections.Generic;
using System.Linq;
using _Core.Managers;
using _Core.Others;
using Monster_System;
using UnityEngine;

namespace Items_and_Barter_System.Scripts
{
    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField] private int _itemLimitPerSlot = 69;
        public List<InventorySlot> inventorySlots;

        public bool HasSufficientItem(ItemObject item, int amount)
        {
            var slotCount = inventorySlots.Count;
            for (var i = 0; i < slotCount; i++)
            {
                if (inventorySlots[i].inventoryItem != item || inventorySlots[i].amountOfItems < amount) continue;
                
                return true;
            }
            return false;
        }

        public void AddItemInPlayerInventory(ItemObject item, int amountToAdd)
        {
            var itemAdded = false;

            foreach (var slot in inventorySlots.Where(slot => slot.inventoryItem == item))
            {
                var targetAmount = slot.amountOfItems + amountToAdd;
                
                if (targetAmount > _itemLimitPerSlot && !(item is Gold))
                {
                    targetAmount -= _itemLimitPerSlot;
                    slot.amountOfItems = _itemLimitPerSlot;
                    amountToAdd = targetAmount;
                    break;
                }

                itemAdded = slot.AddInSlot(amountToAdd);
                GameManager.instance.questManager.UpdateGatherQuest(slot);
                break;
            }

            GameManager.instance.uiManager.UpdateGoldUI();

            if (itemAdded) return;

            var newSlot = new InventorySlot(item, amountToAdd);
            AddInEmptySlot(newSlot);
        }

        private void AddInEmptySlot(InventorySlot newSlot)
        {
            var slotsCount = inventorySlots.Count;
            for (var i = 0; i < slotsCount; i++)
            {
                if (inventorySlots[i].inventoryItem != null) continue;
                
                inventorySlots[i].inventoryItem = newSlot.inventoryItem;
                inventorySlots[i].amountOfItems = newSlot.amountOfItems;
                GameManager.instance.questManager.UpdateGatherQuest(newSlot);
                break;
            }
        }
    }
}