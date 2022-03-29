using _Core.Managers;
using MyBox;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [ReadOnly] [SerializeField] private string _title;
    [ReadOnly] [SerializeField] private string _content;

    public void SetTitleContent(string title, string content)
    {
        _title = title;
        _content = content;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameManager.instance.uiManager.tooltip.ShowToolTip(_title, _content);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GameManager.instance.uiManager.tooltip.HideToolTip();
    }
}
