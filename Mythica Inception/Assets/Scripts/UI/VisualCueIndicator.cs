using System.Collections;
using _Core.Managers;
using TMPro;
using UnityEngine;

public class VisualCueIndicator : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    public LeanTweenType tweenType;
    public Vector3 startingScale = new Vector3(1.2f, .8f, 1);
    public float duration = .5f;
    public bool activated;
    private LTDescr _tweenObject;
    private GameObject _thisGameObject;
    private Transform _thisTransform;
    private Vector3 _one = Vector3.one;
    private Color32 _white = Color.white;
    private Color32 _colorTarget = new Color32(244, 137, 137, 255);
    private Vector3 _positionTarget = new Vector3(0, 1, 50);

    void OnEnable()
    {
        if(!activated) return;
        if (_thisGameObject == null)
        {
            _thisGameObject = gameObject;
            _thisTransform = transform;
        }
        _thisTransform.localScale = startingScale;
        _tweenObject = LeanTween.scale(_thisGameObject, _one, duration);
        _tweenObject.setEase(tweenType);
        StopAllCoroutines();
        StartCoroutine(Disable());
    }

    void OnDisable()
    {
        activated = false;
    }

    private IEnumerator Disable()
    {
        var timeElapsed = 0f;
        var seconds = duration * 1.2f;

        while (timeElapsed <= seconds)
        {
            var time = timeElapsed / seconds;
            textMeshPro.color = Color.Lerp(textMeshPro.color, _colorTarget, time);
            _thisTransform.position = Vector3.Lerp(_thisTransform.position, _positionTarget, time);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        try
        {
            GameManager.instance.pooler.BackToPool(_thisGameObject);
        }
        catch
        {
            _thisGameObject.SetActive(false);
        }
    }

    public void InitializeCue(int value, Color32 colorToChange)
    {
        if (_thisGameObject == null)
        {
            _thisGameObject = gameObject;
            _thisTransform = transform;
        }

        _thisGameObject.SetActive(false);
        _thisTransform.localScale = startingScale;
        _positionTarget = _thisTransform.position + Vector3.up;
        activated = true;
        textMeshPro.text = value.ToString();
        textMeshPro.color = _white;
        _colorTarget = colorToChange;
        _thisGameObject.SetActive(true);
    }
}
