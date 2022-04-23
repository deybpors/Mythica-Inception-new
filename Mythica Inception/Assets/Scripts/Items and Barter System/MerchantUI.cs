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
    private UITweener _thisTweener;
    private GameObject _thisObject;
    [ReadOnly] [SerializeField] private List<ItemObject> _itemsInMerchant = new List<ItemObject>();
    private Dictionary<GameObject, ItemBuyUI> _itemsSlot = new Dictionary<GameObject, ItemBuyUI>();
    public List<ItemBuyUI> _itemBuyUis = new List<ItemBuyUI>();
    public List<BarterRequirements> _itemBarterReq = new List<BarterRequirements>();
    private readonly Color32 _yellow = new Color32(255, 239, 125, 255);

    void Initialize()
    {
        _thisTweener = GetComponent<UITweener>();
        _thisObject = gameObject;
    }

    [Serializable]
    public struct BarterRequirements
    {
        public ItemObject item;
        public int amount;

        public BarterRequirements(ItemObject item, int amount)
        {
            this.item = item;
            this.amount = amount;
        }
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
        var itemReqCount = _itemBarterReq.Count;
        for (var i = 0; i < itemReqCount; i++)
        {
            message += _itemBarterReq[i].amount + " pcs. of ";
            message += _itemBarterReq[i].item.itemName;
            message += i < itemReqCount - 1 ? ", " : " ";
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

        GameManager.instance.uiManager.modal.OpenModal(message, _goldSprite, _yellow, TradeItems);
    }

    private void TradeItems()
    {

    }

    public void UpdateItemsToTradeText()
    {
        itemsToTradeText.text = "Items to trade:\n";
        var itemsCount = _itemBuyUis.Count;
        _itemBarterReq.Clear();

        for (var i = 0; i < itemsCount; i++)
        {
            var amount = _itemBuyUis[i].amount;
            if (amount <= 0) continue;
            var count = _itemBuyUis[i].item.itemBarterRequirements.Count;
            
            for (var j = 0; j < count; j++)
            {
                var itemToBarter = _itemBuyUis[i].item.itemBarterRequirements[j].itemToBarter;
                var amountItems = _itemBuyUis[i].item.itemBarterRequirements[j].amountOfItems * amount;
                BarterRequirements barter = new BarterRequirements(itemToBarter, amountItems);
                _itemBarterReq.Add(barter);
            }

            
        }

        var itemReqCount = _itemBarterReq.Count;

        //check for duplicates
        for (var i = 0; i < itemReqCount; i++)
        {
            itemsToTradeText.text += _itemBarterReq[i].item.itemName + " - ";
            for (var j = 0; j < itemReqCount; j++)
            {
                if(i == j) continue;
                if (_itemBarterReq[i].item != _itemBarterReq[j].item) continue;

                _itemBarterReq[i] = new BarterRequirements(_itemBarterReq[i].item, _itemBarterReq[j].amount + _itemBarterReq[i].amount);
                _itemBarterReq.RemoveAt(j);
                itemReqCount--;
            }
            itemsToTradeText.text += _itemBarterReq[i].amount + " pcs.\n";
        }
    }

    public void WantToBuy(List<ItemObject> items)
    {
        if (_thisObject == null)
        {
            Initialize();
        }

        _parentCanvas.SetActive(true);
        _thisObject.SetActive(true);
        _itemsInMerchant.Clear();
        _itemsInMerchant.AddRange(items);

        var itemsCount = items.Count;
        _itemBuyUis = _itemsSlot.Values.ToList();
        var itemsUiCount = _itemBuyUis.Count;


        if (itemsCount <= itemsUiCount)
        {
            Debug.Log("items: " + itemsCount + " <= Items UI: " + itemsUiCount);
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
            Debug.Log("items: " + itemsCount + " > Items UI: " + itemsUiCount);
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
