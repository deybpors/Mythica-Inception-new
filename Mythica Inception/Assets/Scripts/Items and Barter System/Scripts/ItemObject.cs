using System.Collections.Generic;
using _Core.Managers;
using _Core.Others;
using UnityEngine;

namespace Items_and_Barter_System.Scripts
{
    public enum ItemType
    {
        Basics,
        Upgrades,
        Others
    }
    public abstract class ItemObject : ScriptableObjectWithID
    {
        public string itemName;
        public GameObject itemPrefab;
        public Sprite itemIcon;
        public ItemType itemType;
        [TextArea(15,10)]
        public string itemDescription;
        public bool usable;
        public bool stackable;
        public bool losable = false;
        public List<ItemBarterRequirement> itemBarterRequirements;

        public abstract bool TryUse(IEntity entity);
    }
    
    [System.Serializable]
    public class ItemBarterRequirement
    {
        public ItemObject itemToBarter;
        public int amountOfItems;

        public ItemBarterRequirement(ItemObject item, int amount)
        {
            itemToBarter = item;
            amountOfItems = amount;
        }
    }
    
    [System.Serializable]
    public class InventorySlot
    {
        public ItemObject inventoryItem;
        public int amountOfItems;

        public InventorySlot(ItemObject item, int amount)
        {
            inventoryItem = item;
            amountOfItems = amount;
        }
        
        public bool AddInSlot(int amountToAdd)
        {
            if (!inventoryItem.stackable) return false;
            amountOfItems += amountToAdd;
            var sound = inventoryItem is Gold ? "Coins" : "Equip";

            GameManager.instance.audioManager.PlaySFX(sound);

            return true;
        }

        public bool RemoveInSlot(int amountToRemove)
        {
            amountOfItems -= amountToRemove;

            var sound = inventoryItem is Gold ? "Coins" : "Equip";

            GameManager.instance.audioManager.PlaySFX(sound);

            if (amountOfItems > 0) return true;
            
            amountOfItems = 0;
            inventoryItem = null;

            return true;
        }
    }
}
