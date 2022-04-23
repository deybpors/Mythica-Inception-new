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
    public Button thisButton;
    public TextMeshProUGUI itemName;
    public TooltipTrigger tooltipTrigger;
    public TextMeshProUGUI amountText;
    public GameObject disabledObject;

    public Button addButton;
    public Button subtractButton;

    [Space]
    [ReadOnly] public bool active;
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

        active = false;
        disabledObject.SetActive(false);
        addButton.interactable = false;

        item = itemObject;
        amount = 0;
        amountText.text = "00";
        subtractButton.interactable = false;
        itemName.text = itemObject.itemName;
        itemImage.sprite = itemObject.itemIcon;

        var requirementsCount = itemObject.itemBarterRequirements.Count;
        var sufficient = true;

        for (var i = 0; i < requirementsCount; i++)
        {
            var itemRequired = itemObject.itemBarterRequirements[i].itemToBarter;
            var amountRequired = itemObject.itemBarterRequirements[i].amountOfItems;

            sufficient = sufficient && _player.playerInventory.HasSufficientItem(itemRequired, amountRequired);
        }

        active = sufficient;

        if (sufficient)
        {
            disabledObject.SetActive(false);
            addButton.interactable = true;
            tooltipTrigger.SetTitleContent(itemObject.itemName, item.itemDescription);
        }
        else
        {
            disabledObject.SetActive(true);
            addButton.interactable = false;
            tooltipTrigger.SetTitleContent(string.Empty, string.Empty);
        }

        GameManager.instance.uiManager.merchantUi.UpdateItemsToTradeText();
    }

    public void AddAmount()
    {
        addButton.interactable = false;

        amount++;
        amountText.text = amount.ToString("00");
        addButton.interactable = true;

        if (amount > 0)
        {
            subtractButton.interactable = true;
        }

        var requirementsCount = item.itemBarterRequirements.Count;
        var sufficient = true;

        for (var i = 0; i < requirementsCount; i++)
        {
            var itemToBarter = item.itemBarterRequirements[i].itemToBarter;
            var amountRequired = item.itemBarterRequirements[i].amountOfItems * (amount + 1);

            sufficient = sufficient && _player.playerInventory.HasSufficientItem(itemToBarter, amountRequired);
        }

        if (!sufficient)
        {
            GameManager.instance.audioManager.PlaySFX("Error");
            addButton.interactable = false;
        }

        GameManager.instance.uiManager.merchantUi.UpdateItemsToTradeText();

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

        var requirementsCount = item.itemBarterRequirements.Count;
        var sufficient = true;

        for (var i = 0; i < requirementsCount; i++)
        {
            var itemToBarter = item.itemBarterRequirements[i].itemToBarter;
            var amountRequired = item.itemBarterRequirements[i].amountOfItems * amount;

            sufficient = sufficient && _player.playerInventory.HasSufficientItem(itemToBarter, amountRequired);
        }

        if (!sufficient)
        {
            addButton.interactable = false;
        }

        GameManager.instance.uiManager.merchantUi.UpdateItemsToTradeText();

        if (GameManager.instance == null) return;
        GameManager.instance.audioManager.PlaySFX("Button Click", .5f);
    }
}
