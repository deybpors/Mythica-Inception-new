using Assets.Scripts._Core;
using Assets.Scripts._Core.Others;
using Assets.Scripts._Core.Player;
using UnityEngine;

namespace Assets.Scripts.Items_and_Barter_System.Scripts
{
    [CreateAssetMenu(menuName = "Items and Crafting System/New Consumable Item")]
    public class ConsumableItem : ItemObject
    {
        public int healthToAdd;
        public int staminaToAdd;

        public void Awake()
        {
            itemType = ItemType.Basics;
            usable = true;
            stackable = true;
        }

        public override void Use(GameObject user)
        {
            if (healthToAdd > 0)
            {
                AddHealth(user);
            }

            if (staminaToAdd > 0)
            {
                AddStamina(user);
            }
        }

        private void AddStamina(GameObject toUse)
        {
            IHaveStamina entityWithStamina = toUse.GetComponent<IHaveStamina>();
            if(entityWithStamina == null) return;
            entityWithStamina.AddStamina(staminaToAdd);
        }

        private void AddHealth(GameObject toUse)
        {
            IHaveHealth entityWithHealth = toUse.GetComponent<IHaveHealth>();
            if(entityWithHealth == null) return;
            entityWithHealth.Heal(healthToAdd);
        }
    }
}