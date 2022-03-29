using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using UnityEngine;

namespace UI
{
    public class TabGroup : MonoBehaviour
    {
        // These must be 1 to 1, same order in hierarchy
        [HideInInspector]
        public List<TabButton> tabButtons = new List<TabButton>();
        public List<GameObject> tabPages = new List<GameObject>();

        //In case I need to sort the lists by GetSiblingIndex
        //objListOrder.Sort((x, y) => x.OrderDate.CompareTo(y.OrderDate));

        public Color tabIdleColor;
        public Color tabHoverColor;

        [ConditionalField(nameof(selectorImage), true)]
        public Color tabSelectedColor;

        public RectTransform selectorImage;

        [ConditionalField(nameof(selectorImage))]
        public float smooth = .8f;

        private TabButton selectedTab;
        private Dictionary<TabButton, RectTransform> _tabButTransforms = new Dictionary<TabButton, RectTransform>();

        public void Start()
        {
            // Select first tab
            foreach (var tabButton in tabButtons.Where(tabButton => tabButton.transform.GetSiblingIndex() == 0))
            {
                OnTabSelected(tabButton);
            }
        }

        public void Subscribe(TabButton tabButton)
        {
            tabButtons.Add(tabButton);
            // Sort by order in hierarchy
            tabButtons.Sort((x, y) => x.transform.GetSiblingIndex().CompareTo(y.transform.GetSiblingIndex()));
        }

        public void OnTabEnter(TabButton tabButton)
        {
            ResetTabs();
            if ((selectedTab == null) || (tabButton != selectedTab))
                tabButton.background.color = tabHoverColor;
        }

        public void OnTabExit(TabButton tabButton)
        {
            ResetTabs();
        }

        public void OnTabSelected(TabButton tabButton)
        {
            if (selectedTab != null)
            {
                selectedTab.Deselect();
            }

            selectedTab = tabButton;

            selectedTab.Select();

            ResetTabs();

            var index = tabButton.transform.GetSiblingIndex();

            if (selectorImage == null)
            {
                tabButton.background.color = tabSelectedColor;
            }
            else
            {
                _tabButTransforms.TryGetValue(tabButton, out var rectTransform);
                if (rectTransform == null)
                {
                    rectTransform = tabButton.GetComponent<RectTransform>();
                    _tabButTransforms.Add(tabButton, rectTransform);
                }

                index -= 1;

                StopAllCoroutines();
                StartCoroutine(MoveSelectedImage(rectTransform.anchoredPosition));
            }

            
            for (var i = 0; i < tabPages.Count; i++)
            {
                try
                {
                    tabPages[i].SetActive(i == index);

                }
                catch
                {
                    //ignored
                }
            }
        }

        private void ResetTabs()
        {
            foreach (var tabButton in tabButtons.Where(tabButton => (selectedTab == null) || (tabButton != selectedTab)))
            {
                tabButton.background.color = tabIdleColor;
            }
        }

        public void NextTab()
        {
            int currentIndex = selectedTab.transform.GetSiblingIndex();
            int nextIndex = currentIndex < tabButtons.Count - 1 ? currentIndex + 1 : tabButtons.Count - 1;
            OnTabSelected(tabButtons[nextIndex]);
        }

        public void PreviousTab()
        {
            int currentIndex = selectedTab.transform.GetSiblingIndex();
            int previousIndex = currentIndex > 0 ? currentIndex - 1 : 0;
            OnTabSelected(tabButtons[previousIndex]);
        }

        private IEnumerator MoveSelectedImage(Vector2 targetPosition)
        {
            while (!selectorImage.anchoredPosition.Approximately(targetPosition))
            {
                selectorImage.anchoredPosition = Vector2.Lerp(selectorImage.anchoredPosition, targetPosition,
                    Time.unscaledDeltaTime * smooth);
                yield return null;
            }
        }
    }
}
