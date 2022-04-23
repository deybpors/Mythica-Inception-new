using System.Collections.Generic;
using _Core.Managers;
using _Core.Others;
using Items_and_Barter_System.Scripts;
using UnityEngine;

[CreateAssetMenu(menuName = "Combat System/Healer and Merchant")]
public class HealerMerchant : ScriptableObject
{
    public List<ItemObject> itemToSell;

    public void HealMonsters()
    {
        var player = GameManager.instance.player;
        player.FullyRestoreAllMonsters();
        var playerHealth = player.playerHealth;
        playerHealth.maxHealth = GameSettings.MonstersAvgHealth(player.GetMonsterSlots()) <= 0 ?
            GameManager.instance.saveManager.defaultPlayerHealth :
            GameSettings.Stats(GameSettings.MonstersAvgHealth(player.GetMonsterSlots()),
                GameSettings.MonstersAvgStabilityValue(player.GetMonsterSlots()),
                GameSettings.MonstersAvgLevel(player.GetMonsterSlots()));

        playerHealth.currentHealth = playerHealth.maxHealth;
        player.healthComponent.UpdateHealth(playerHealth.maxHealth, playerHealth.currentHealth);
        GameManager.instance.uiManager.UpdateHealthUI(GameManager.instance.inputHandler.currentMonster, playerHealth.currentHealth);
        GameManager.instance.audioManager.PlaySFX("Confirmation");
    }

    public void Barter()
    {
        GameManager.instance.uiManager.dialogueUI.cutscene = true;
        GameManager.instance.uiManager.merchantUi.WantToBuy(itemToSell);
        GameManager.instance.inputHandler.EnterOptions();
    }

    void OnValidate()
    {
        for (var i = 0; i < itemToSell.Count; i++)
        {
            for (var j = 0; j < itemToSell.Count; j++)
            {
                if(i == j) continue;
                if (itemToSell[i] == itemToSell[j])
                {
                    itemToSell.RemoveAt(j);
                }
            }
        }
    }
}
