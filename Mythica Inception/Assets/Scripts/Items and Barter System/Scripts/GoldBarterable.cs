using UnityEngine;

namespace Items_and_Barter_System.Scripts
{
    [CreateAssetMenu(menuName = "Items and Crafting System/New Gold Barterable Item")]
    public class GoldBarterable : ItemObject
    {
        public void Awake()
        {
            stackable = true;
        }
    }
}