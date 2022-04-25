using System;
using _Core.Managers;
using _Core.Player;
using Items_and_Barter_System.Scripts;
using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemBuyUI : MonoBehaviour
{
    public Image itemImage;
    public TextMeshProUGUI itemName;
    public TooltipTrigger tooltipTrigger;
    public TextMeshProUGUI amountText;
    public GameObject disabledObject;

    public Button addButton;
    public Button subtractButton;

    [Space]
    [ReadOnly] public ItemObject item;
    [ReadOnly] public int amount = 0;
    private GameObject _thisObject;
    private Player _player;

    void OnEnable()
    {
        _thisObject = gameObject;
        _player = GameManager.instance.player;
    }

    public void DisableSlot()
    {
        if (_thisObject == null)
        {
            _thisObject = gameObject;
        }
        _thisObject.SetActive(false);
    }

    public void EnableSlot()
    {
        if (_thisObject == null)
        {
            _thisObject = gameObject;
        }
        _thisObject.SetActive(true);
    }

    public void SetupButton(ItemObject itemObject)
    {
        EnableSlot();
        if (_player == null)
        {
            _player = GameManager.instance.player;
        }

        disabledObject.SetActive(false);
        addButton.interactable = false;

        item = itemObject;
        amount = 0;
        amountText.text = "00";
        subtractButton.interactable = false;
        itemName.text = itemObject.itemName;
        itemImage.sprite = itemObject.itemIcon;

        var sufficient = IsSufficient(itemObject, amount);

        if (sufficient)
        {
            disabledObject.SetActive(false);
            addButton.interactable = true;
            
            
            var description = item.itemDescription + "\n\nItem Required for Trading:\n";
            var economyValue = GameManager.instance.difficultyManager.GetParameterValue("Economy");

            if (economyValue < 1)
            {
                description += "<color=#b3f47a>Final amount mark down to " + (int)(economyValue * 100) + "%!</color>\n";
            }
            else if (economyValue > 1)
            {
                description += "<color=#f48989>Final amount mark up of " + (int)((economyValue * 100) - 100) + "%.</color>\n";
            }

            var count = item.itemBarterRequirements.Count;
            
            
            for (var i = 0; i < count; i++)
            {
                description += item.itemBarterRequirements[i].amountOfItems + "pcs. of " +
                               item.itemBarterRequirements[i].itemToBarter.itemName + "\n";
            }

            tooltipTrigger.SetTitleContent(itemObject.itemName, description);
        }
        else
        {
            disabledObject.SetActive(true);
            addButton.interactable = false;
            tooltipTrigger.SetTitleContent(string.Empty, string.Empty);
        }

        GameManager.instance.uiManager.merchantUi.UpdateItemsToTrade();
    }

    public void AddAmount()
    {
        amount++;
        amountText.text = amount.ToString("00");
        addButton.interactable = true;

        if (amount > 0)
        {
            subtractButton.interactable = true;
        }

        var sufficient = IsSufficient(item, amount + 1);

        if (!sufficient)
        {
            GameManager.instance.audioManager.PlaySFX("Error");
            addButton.interactable = false;
        }

        GameManager.instance.uiManager.merchantUi.UpdateItemsToTrade();

        if (GameManager.instance == null) return;
        GameManager.instance.audioManager.PlaySFX("Button Click", 1f);
    }

    public void SubtractAmount()
    {
        amount--;
        amountText.text = amount.ToString("00");
        addButton.interactable = true;

        if (amount <= 0)
        {
            subtractButton.interactable = false;
        }

        var sufficient = IsSufficient(item, amount);

        if (!sufficient)
        {
            addButton.interactable = false;
        }

        GameManager.instance.uiManager.merchantUi.UpdateItemsToTrade();

        if (GameManager.instance == null) return;
        GameManager.instance.audioManager.PlaySFX("Button Click", .5f);
    }

    private bool IsSufficient(ItemObject itemObject, int currentAmount)
    {
        var requirementsCount = itemObject.itemBarterRequirements.Count;
        var sufficient = true;

        for (var i = 0; i < requirementsCount; i++)
        {
            var itemRequired = itemObject.itemBarterRequirements[i].itemToBarter;
            var amountRequired = itemObject.itemBarterRequirements[i].amountOfItems * currentAmount;

            amountRequired = (int)Math.Round(amountRequired * GameManager.instance.difficultyManager.GetParameterValue("Economy"));

            sufficient = sufficient && _player.playerInventory.HasSufficientItem(itemRequired, amountRequired);
        }

        return sufficient;
    }
}
