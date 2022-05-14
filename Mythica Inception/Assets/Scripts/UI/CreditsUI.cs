using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreditsUI : MonoBehaviour
{
    public TextAsset creditTextAsset;
    public RectTransform content;
    public TextMeshProUGUI creditText;
    public ScrollRect scrollRect;

    [Range(.001f, .05f)]
    public float speed = .025f;
    private Coroutine _scrollCoroutine;


    void OnEnable()
    {
        if(creditText.text == string.Empty)
            PopulateCreditsList();

        content.anchoredPosition = new Vector2(content.anchoredPosition.x, 0);
    }

    private void PopulateCreditsList()
    {
        if(creditTextAsset == null) return;
        creditText.text = string.Empty;
        var creditData = creditTextAsset.text.Split('\n');
        var creditCount = creditData.Length;

        for (var i = 0; i < creditCount; i++)
        {
            var credit = creditData[i].Split(',');
            if (credit[0] == string.Empty) break;

            var creditString = "<b>" + credit[0] + "</b> - " +
                               credit[1] + " " +
                               credit[2] + "\n" +
                               credit[4] + "\n" +
                               "<link=\"" + credit[5] + "\"><i><color=#97e4ff>" + credit[5] + "</color></i></link>\n\n";
            creditText.text += creditString;
        }

        StartCoroutine(ChangeSize());
    }

    void Update()
    {
        if (scrollRect.verticalNormalizedPosition > 0)
        {
            _scrollCoroutine ??= StartCoroutine(Scroll());
        }
    }

    void OnDisable()
    {
        _scrollCoroutine = null;
    }

    private IEnumerator ChangeSize()
    {
        while (true)
        {
            content.sizeDelta = new Vector2(content.sizeDelta.x, 42.35f * creditText.textInfo.lineCount);
            yield return null;
            if (content.sizeDelta.y > 0) break;
        }
    }

    private IEnumerator Scroll()
    {
        while (scrollRect.verticalNormalizedPosition > 0)
        {
            scrollRect.verticalNormalizedPosition -= Time.deltaTime * speed;
            if(scrollRect.verticalNormalizedPosition <= 0) break;
            yield return null;
        }

        _scrollCoroutine = null;
    }
}
