using _Core.Managers;
using Items_and_Barter_System.Scripts;
using UnityEngine;

[CreateAssetMenu(menuName = "Combat System/Healer and Merchant")]
public class HealerMerchant : ScriptableObject
{
    public ItemObject[] itemToSell;

    public void HealMonsters()
    {
        GameManager.instance.player.FullyRestoreAllMonsters();
    }
}
