using _Core.Others;
using UnityEngine;

namespace Items_and_Barter_System.Scripts
{
    [CreateAssetMenu(menuName = "Items and Crafting System/New Consumable Item")]
    public class ConsumableItem : ItemObject
    {
        public int healthToAdd;

        public void Awake()
        {
            itemType = ItemType.Basics;
            usable = true;
            stackable = true;
        }

        public override bool TryUse(IEntity entity)
        {
            if (!(entity is IHaveHealth haveHealth && usable)) return false;
            
            haveHealth.Heal(healthToAdd);
            return true;
        }
    }
}