using _Core.Others;
using UnityEngine;

namespace Assets.Scripts.Items_and_Barter_System.Scripts
{
    [CreateAssetMenu(menuName = "Items and Crafting System/New Attack Item")]
    public class AttackUpgradeItem : UpgradeItem
    {
        public int attackToAdd;

        public override int GetValueToUpgrade()
        {
            return attackToAdd;
        }
    }
}
