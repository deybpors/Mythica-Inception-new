using System;
using System.Collections.Generic;
using System.Linq;
using _Core.Managers;
using UnityEngine;

namespace Items_and_Barter_System.Scripts
{
    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField] private int _itemLimitPerSlot = 69;
        public List<InventorySlot> inventorySlots;

        private readonly Dictionary<ItemObject, int> _totalInventory = new Dictionary<ItemObject, int>();

        public void UpdateTotalInventory()
        {
            var slotCount = inventorySlots.Count;
            _totalInventory.Clear();

            for (var i = 0; i < slotCount; i++)
            {
                if(inventorySlots[i].inventoryItem == null) continue;
                try
                {
                    _totalInventory.Add(inventorySlots[i].inventoryItem, inventorySlots[i].amountOfItems);
                }
                catch
                {
                    _totalInventory[inventorySlots[i].inventoryItem] += inventorySlots[i].amountOfItems;
                }
            }
        }

        public bool HasSufficientItem(ItemObject item, int amount)
        {
            if (!_totalInventory.TryGetValue(item, out var currentAmount))
            {
                return false;
            }

            return currentAmount >= amount;
        }

        public void AddItemInPlayerInventory(ItemObject item, int amountToAdd)
        {
            if (!item.stackable)
            {
                for (var i = 0; i < amountToAdd; i++)
                {
                    var newInventorySlot = new InventorySlot(item, 1);
                    AddInEmptySlot(newInventorySlot);
                }
                return;
            }

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

            if (itemAdded)
            {
                GameManager.instance.uiManager.UpdateGoldUI();
                UpdateTotalInventory();
                return;
            }

            var newSlot = new InventorySlot(item, amountToAdd);

            AddInEmptySlot(newSlot);
            GameManager.instance.uiManager.UpdateGoldUI();
            UpdateTotalInventory();
        }

        public void RemoveItemInInventory(ItemObject item, int amountToRemove)
        {
            var itemRemoved = false;

            foreach (var slot in inventorySlots.Where(slot => slot.inventoryItem == item))
            {
                var targetAmount = slot.amountOfItems - amountToRemove;

                if (targetAmount < 0 && !(item is Gold))
                {
                    targetAmount -= _itemLimitPerSlot;
                    slot.amountOfItems = 0;
                    amountToRemove = targetAmount;
                    break;
                }

                itemRemoved = slot.RemoveInSlot(amountToRemove);
                break;
            }

            if (itemRemoved)
            {
                GameManager.instance.uiManager.UpdateGoldUI();
                UpdateTotalInventory();
                return;
            }

            if(amountToRemove <= 0) return;

            RemoveItemInInventory(item, amountToRemove);
            GameManager.instance.uiManager.UpdateGoldUI();
            UpdateTotalInventory();
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
                GameManager.instance.audioManager.PlaySFX("Equip");
                break;
            }
        }
    }
}