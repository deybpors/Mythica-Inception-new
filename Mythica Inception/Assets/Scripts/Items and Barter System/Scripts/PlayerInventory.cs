using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Items_and_Barter_System.Scripts
{
    public class PlayerInventory : MonoBehaviour
    {
        public List<InventorySlot> inventorySlots;

        public void AddItemInPlayerInventory(ItemObject item, int amountToAdd)
        {
            bool itemAdded = false;

            foreach (var slot in inventorySlots.Where(slot => slot.inventoryItem == item))
            {
                itemAdded = slot.AddInSlot(amountToAdd);
            }

            if (itemAdded) return;
            
            var newSlot = new InventorySlot(item, amountToAdd);
            inventorySlots.Add(newSlot);
        }
    }
}