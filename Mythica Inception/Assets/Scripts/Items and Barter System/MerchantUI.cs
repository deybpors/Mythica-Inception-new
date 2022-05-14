using System;
using System.Collections.Generic;
using System.Linq;
using _Core.Managers;
using Items_and_Barter_System.Scripts;
using MyBox;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class MerchantUI : MonoBehaviour
{
    public Button minimizeButton;
    [SerializeField] private GameObject _parentCanvas;
    [SerializeField] private RectTransform _contentParent;
    [SerializeField] private ItemBuyUI itemBuyUI;
    [SerializeField] private Sprite _goldSprite;
    public TextMeshProUGUI itemsToTradeText;

    [ReadOnly] public bool buying;
    private UITweener _thisTweener;
    [HideInInspector] public GameObject thisObject;
    private readonly Dictionary<GameObject, ItemBuyUI> _itemsSlot = new Dictionary<GameObject, ItemBuyUI>();
    private List<ItemBuyUI> _itemBuyUis = new List<ItemBuyUI>();
    private readonly List<ItemBarterRequirement> _itemToTrade = new List<ItemBarterRequirement>();
    private readonly Color32 _yellow = new Color32(255, 239, 125, 255);
    private List<ItemObject> _itemsTrading = new List<ItemObject>();

    void Start()
    {
        if (thisObject == null)
        {
            Initialize();
        }
    }

    public void Initialize()
    {
        _thisTweener = GetComponent<UITweener>();
        thisObject = gameObject;
    }

    public void DisableMerchantUI()
    {
        try
        {
            GameManager.instance.uiManager.dialogueUI.cutscene = false;
            GameManager.instance.inputHandler.EnterGameplay();
            GameManager.instance.audioManager.PlaySFX("UI Close");
        }
        catch
        {
            //ignored
        }

        if (_thisTweener == null)
        {
            _thisTweener = GetComponent<UITweener>();
        }
        _thisTweener.Disable();
    }

    public void ConfirmTrade()
    {
        var message = "Are you sure you want to trade:\n";
        var itemReqCount = _itemToTrade.Count;

        if (itemReqCount <= 0)
        {
            GameManager.instance.audioManager.PlaySFX("Error");
            return;
        }

        var itemsSufficient = true;
        var inventory = GameManager.instance.player.playerInventory;

        for (var i = 0; i < itemReqCount; i++)
        {
            message += _itemToTrade[i].amountOfItems + " pcs. of ";
            message += _itemToTrade[i].itemToBarter.itemName;
            message += i < itemReqCount - 1 ? ", " : " ";
            itemsSufficient = itemsSufficient &&
                                     inventory.HasSufficientItem(_itemToTrade[i].itemToBarter,
                                         _itemToTrade[i].amountOfItems);
        }

        if (!itemsSufficient)
        {
            message = "<size=80%><color=#f48989>" +
                "Items to trade not sufficient in your inventory. Try to decrease items you want to receive or get more items in your inventory.";
            GameManager.instance.uiManager.modal.OpenModal(message, _goldSprite, _yellow, (() => TradeItems(itemsSufficient)));
            return;
        }

        message += "for ";

        var itemsCount = _itemBuyUis.Count;

        for (var i = 0; i < itemsCount; i++)
        {
            if(_itemBuyUis[i].amount <= 0) continue;
            message += _itemBuyUis[i].amount + " pcs. of ";
            message += _itemBuyUis[i].item.itemName;
            message += i == itemsCount - 1 ? "?" : ", ";
        }

        GameManager.instance.uiManager.modal.OpenModal(message, _goldSprite, _yellow, (() => TradeItems(itemsSufficient)));
    }

    private void TradeItems(bool itemsSufficient)
    {
        GameManager.instance.uiManager.modal.CloseModal();

        if (!itemsSufficient) return;

        var inventory = GameManager.instance.player.playerInventory;

        ChangeBuying(false);
        var itemsCount = _itemBuyUis.Count;
        for (var i = 0; i < itemsCount; i++)
        {
            if (_itemBuyUis[i].amount <= 0) continue;
            inventory.AddItemInPlayerInventory(_itemBuyUis[i].item, _itemBuyUis[i].amount);
        }

        var itemsToBarterCount = _itemToTrade.Count;
        for (var i = 0; i < itemsToBarterCount; i++)
        {
            inventory.RemoveItemInInventory(_itemToTrade[i].itemToBarter, _itemToTrade[i].amountOfItems);
        }

        WantToTrade(_itemsTrading);
    }

    public void ChangeBuying(bool isBuying)
    {
        buying = isBuying;
    }

    public void UpdateItemsToTrade()
    {
        itemsToTradeText.text = "Items to trade:\n";
        var itemsCount = _itemBuyUis.Count;
        _itemToTrade.Clear();

        for (var i = 0; i < itemsCount; i++)
        {
            var amountOfItem = _itemBuyUis[i].amount;
            if (amountOfItem <= 0) continue;
            var count = _itemBuyUis[i].item.itemBarterRequirements.Count;
            
            for (var j = 0; j < count; j++)
            {
                var itemToBarter = _itemBuyUis[i].item.itemBarterRequirements[j].itemToBarter;
                var economyValue = GameManager.instance.difficultyManager.GetParameterValue("Economy");
                var amountRequired = _itemBuyUis[i].item.itemBarterRequirements[j].amountOfItems;
                var amountItems = (int)Math.Round(amountRequired * amountOfItem * economyValue, MidpointRounding.AwayFromZero);

                var barter = new ItemBarterRequirement(itemToBarter, amountItems);
                _itemToTrade.Add(barter);
            }
        }

        var itemToTradeCount = _itemToTrade.Count;

        //check for duplicates
        for (var i = 0; i < itemToTradeCount; i++)
        {
            itemsToTradeText.text += _itemToTrade[i].itemToBarter.itemName + " - ";
            for (var j = 0; j < itemToTradeCount; j++)
            {
                if(i == j) continue;
                if (_itemToTrade[i].itemToBarter != _itemToTrade[j].itemToBarter) continue;

                _itemToTrade[i] = new ItemBarterRequirement(_itemToTrade[i].itemToBarter, _itemToTrade[j].amountOfItems + _itemToTrade[i].amountOfItems);
                _itemToTrade.RemoveAt(j);
                itemToTradeCount--;
            }
            itemsToTradeText.text += _itemToTrade[i].amountOfItems + " pcs.\n";
        }
    }

    public void WantToTrade(List<ItemObject> items)
    {
        if (thisObject == null)
        {
            Initialize();
        }

        _parentCanvas.SetActive(true);
        thisObject.SetActive(true);
        ChangeBuying(true);
        _itemsTrading = items;

        var itemsCount = items.Count;
        _itemBuyUis = _itemsSlot.Values.ToList();
        var itemsUiCount = _itemBuyUis.Count;


        if (itemsCount <= itemsUiCount)
        {
            for (var i = 0; i < itemsUiCount; i++)
            {
                if (i >= itemsCount)
                {
                    _itemBuyUis[i].DisableSlot();
                    continue;
                }
                _itemBuyUis[i].SetupButton(items[i]);
            }
        }
        else
        {
            for (var i = 0; i < itemsCount; i++)
            {
                if (i >= itemsUiCount)
                {
                    var itemSlot = Instantiate(itemBuyUI, _contentParent);
                    _itemsSlot.Add(itemSlot.gameObject, itemSlot);
                    itemSlot.SetupButton(items[i]);
                }
                else
                {
                    _itemBuyUis[i].SetupButton(items[i]);
                }
            }
            _itemBuyUis = _itemsSlot.Values.ToList();
        }
    }
}
