using System.Collections.Generic;
using _Core.Others;
using UnityEngine;

namespace Items_and_Barter_System.Scripts
{
    public enum ItemType
    {
        Basics,
        Upgrades,
        GoldBarterable,
        Others
    }
    public class ItemObject : ScriptableObjectWithID
    {
        public string itemName;
        public GameObject itemPrefab;
        public Sprite itemIcon;
        public ItemType itemType;
        [TextArea(15,10)]
        public string itemDescription;
        public bool usable;
        public bool stackable;
        public List<ItemBarterRequirement> itemBarterRequirements;

        public virtual void Use(GameObject user){}
    }
    
    [System.Serializable]
    public class ItemBarterRequirement
    {
        public ItemObject itemToBarter;
        public int amountOfItems;
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
            if(!inventoryItem.stackable) return false;

            amountOfItems += amountToAdd;
            return true;
        }
    }
}
