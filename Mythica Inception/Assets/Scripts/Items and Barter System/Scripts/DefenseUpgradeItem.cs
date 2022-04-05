using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Items_and_Barter_System.Scripts
{
    [CreateAssetMenu(menuName = "Items and Crafting System/Defense Item")]
    public class DefenseUpgradeItem : UpgradeItem
    {
        public int defenseToAdd;
        public override int GetValueToUpgrade()
        {
            return defenseToAdd;
        }
    }
}
