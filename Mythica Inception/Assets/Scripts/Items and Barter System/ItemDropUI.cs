using System.Collections.Generic;
using _Core.Managers;
using _Core.Player;
using Items_and_Barter_System.Scripts;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

public class ItemDropUI : MonoBehaviour
{
    public List<Button> buttons;

    private Dictionary<Button, GameObject> _objects = new Dictionary<Button, GameObject>();
    private Dictionary<Button, UITweener> _tweeners = new Dictionary<Button, UITweener>();
    private Dictionary<Button, TextMeshProUGUI> _texts = new Dictionary<Button, TextMeshProUGUI>();
    private Dictionary<Button, Image> _images = new Dictionary<Button, Image>();
    private Dictionary<Button, TooltipTrigger> _tooltipTriggers = new Dictionary<Button, TooltipTrigger>();
    private Dictionary<ItemDrop, Button> _itemDrops = new Dictionary<ItemDrop, Button>();
    private int _buttonsCount;
    
    void Awake()
    {
        if(_objects.Count > 0) return;

        _buttonsCount = buttons.Count;

        for (var i = 0; i < _buttonsCount; i++)
        {
            var obj = buttons[i].gameObject;
            _objects.Add(buttons[i], obj);
            _tweeners.Add(buttons[i], buttons[i].GetComponent<UITweener>());
            _texts.Add(buttons[i], buttons[i].GetComponentInChildren<TextMeshProUGUI>());
            _images.Add(buttons[i], (Image)buttons[i].targetGraphic);
            _tooltipTriggers.Add(buttons[i], buttons[i].GetComponent<TooltipTrigger>());
            obj.SetActive(false);
        }
    }

    public void Subscribe(ItemDrop itemDrop, ItemObject item, int amount)
    {
        if(_itemDrops.ContainsKey(itemDrop)) return;

        for (var i = 0; i < _buttonsCount; i++)
        {
            var button = buttons[i];

            var tweener = _tweeners[button];
            var obj = _objects[button];
            
            if (obj.activeInHierarchy) continue;


            _texts[button].text = "x" + amount;
            _images[button].sprite = item.itemIcon;
            _tooltipTriggers[button].SetTitleContent(item.itemName, item.itemDescription);
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener((() => AddItem(itemDrop)));
            _itemDrops.Add(itemDrop, button);

            obj.SetActive(true);
            return;
        }
    }

    public void Unsubscribe(ItemDrop itemDrop)
    {
        if (!_itemDrops.TryGetValue(itemDrop, out var button)) return;
        
        GameManager.instance.uiManager.tooltip.HideToolTip();
        _itemDrops.Remove(itemDrop);
        _tweeners[button].Disable();
    }

    private void AddItem(ItemDrop drop)
    {
        drop.Interact(GameManager.instance.player);
        Unsubscribe(drop);
    }
}
