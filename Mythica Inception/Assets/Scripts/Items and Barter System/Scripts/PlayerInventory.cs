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

            var slotCount = inventorySlots.Count;

            for (var i = 0; i < slotCount; i++)
            {
                var slot = inventorySlots[i];
                if (slot.inventoryItem != item) continue;

                if (item is Gold)
                {
                    slot.AddInSlot(amountToAdd);
                    amountToAdd = 0;
                    break;
                }
               
                var targetAmount = slot.amountOfItems + amountToAdd;

                if (targetAmount <= _itemLimitPerSlot)
                {
                    slot.AddInSlot(amountToAdd);
                    amountToAdd = 0;
                    break;
                }

                targetAmount -= _itemLimitPerSlot;
                slot.amountOfItems = _itemLimitPerSlot;
                amountToAdd = targetAmount;
            }

            if (amountToAdd > 0)
            {
                var newSlot = new InventorySlot(item, amountToAdd);
                AddInEmptySlot(newSlot);
            }

            UpdateTotalInventory();
            GameManager.instance.questManager.UpdateGatherQuest();
            GameManager.instance.uiManager.UpdateGoldUI();
            var player = GameManager.instance.player;
            GameManager.instance.uiManager.UpdateItemsUI(player.monsterManager, -1, player.monsterSlots);
        }

        public void RemoveItemInInventory(ItemObject item, int amountToRemove)
        {

            foreach (var slot in inventorySlots.Where(slot => slot.inventoryItem == item))
            {
                if (item is Gold)
                {
                    slot.RemoveInSlot(amountToRemove);
                    amountToRemove = 0;
                    break;
                }

                var targetAmount = slot.amountOfItems - amountToRemove;
                if (targetAmount >= 0)
                {
                    slot.RemoveInSlot(amountToRemove);
                    amountToRemove = 0;
                    break;
                }

                targetAmount *= -1;
                slot.amountOfItems = 0;
                amountToRemove = targetAmount;
            }

            if (amountToRemove <= 0)
            {
                UpdateTotalInventory();
                GameManager.instance.questManager.UpdateGatherQuest();
                GameManager.instance.uiManager.UpdateGoldUI();
                var player = GameManager.instance.player;
                GameManager.instance.uiManager.UpdateItemsUI(player.monsterManager, -1, player.monsterSlots);
                return;
            }

            RemoveItemInInventory(item, amountToRemove);
        }

        private void AddInEmptySlot(InventorySlot newSlot)
        {
            var slotsCount = inventorySlots.Count;
            for (var i = 0; i < slotsCount; i++)
            {
                if (inventorySlots[i].inventoryItem != null) continue;
                
                inventorySlots[i].inventoryItem = newSlot.inventoryItem;
                inventorySlots[i].amountOfItems = newSlot.amountOfItems;
                GameManager.instance.questManager.UpdateGatherQuest();
                GameManager.instance.audioManager.PlaySFX("Equip");
                break;
            }
            UpdateTotalInventory();
        }

        public int GetTotalAmountItems(ItemObject item)
        {
            return _totalInventory.TryGetValue(item, out var amount) ? amount : 0;
        }

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

        public bool CanAdd(ItemObject item, int amount)
        {
            if (item is Gold)
            {
                return true;
            }

            var canAdd = false;
            var slotCount = inventorySlots.Count;
            for (var i = 0; i < slotCount; i++)
            {
                var slot = inventorySlots[i];
                
                if (slot.inventoryItem == null)
                {
                    canAdd = true;
                    break;
                }

                if (!item.stackable) continue;
                
                if (slot.inventoryItem != item) continue;
                    
                if (amount + slot.amountOfItems > _itemLimitPerSlot) continue;

                canAdd = true;
            }

            return canAdd;
        }
    }
}