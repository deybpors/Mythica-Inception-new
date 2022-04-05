
using UnityEngine;

namespace Assets.Scripts.Items_and_Barter_System.Scripts
{
    [CreateAssetMenu(menuName = "Items and Crafting System/Both Upgrade Item")]
    public class UpgradeItemBoth : UpgradeItem
    {
        public int attackToAdd;
        public int defenseToAdd;

        public override UpgradeValues GetUpgradeValues()
        {
            return new UpgradeValues(attackToAdd, defenseToAdd);
        }
    }
}
