using UnityEngine;

namespace Items_and_Barter_System.Scripts
{
    [CreateAssetMenu(menuName = "Items and Crafting System/New Others/Gold")]
    public class Gold: ItemObject
    {
        public void Awake()
        {
            itemName = "Gold";
            itemType = ItemType.Others;
            itemDescription = "Pearl Isle's mode of currency.";
            stackable = true;
        }
    }
}