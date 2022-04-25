using System;
using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    public enum UIAnimationType
    {
        Move,
        Scale,
        Fade
    }
    public class UITweener : MonoBehaviour
    {
        public GameObject objectToAnimate;
        public bool ignoreTimeScale;
        public UIAnimationType animationType;
        public LeanTweenType easeType;
        public float duration;
        public float delay;
        public bool loop;
        public bool pingpong;
        public bool startPositionOffset;

        [ConditionalField(nameof(animationType), false, UIAnimationType.Scale, UIAnimationType.Move)] 
        public Vector3 fromState;
        private Vector3 origFromState;
        
        [ConditionalField(nameof(animationType), false, UIAnimationType.Scale, UIAnimationType.Move)]
        public Vector3 toState;
        private Vector3 origToState;

        [ConditionalField(nameof(animationType), false, UIAnimationType.Fade)]
        public float from;
        private float origFrom;
        
        [ConditionalField(nameof(animationType), false, UIAnimationType.Fade)]
        public float to;
        private float origTo;

        public List<UITweener> objectsToDisableOnDisable;
        public bool disableAfterSeconds;

        [ConditionalField(nameof(disableAfterSeconds))]
        public float disableDuration;

        public bool inactiveAfterDone;

        private LTDescr _tweenObject;
        public bool showOnEnable;
        [SerializeField, HideInInspector] private CanvasGroup _canvasGroup;

        [HideInInspector] public bool disabled;

        void Awake()
        {
            origFromState = fromState;
            origToState = toState;
            origFrom = from;
            origTo = to;
        }

        public void OnEnable()
        {
            if (_canvasGroup == null)
            {
                _canvasGroup = gameObject.GetComponent<CanvasGroup>();
            }

            if (showOnEnable)
            {
                Show();
            }

            disabled = false;
        }
        private void Show()
        {
            HandleTween();
        }

        public LTDescr GetTweenObject()
        {
            return _tweenObject;
        }

        public CanvasGroup GetCanvasGroup()
        {
            if (_canvasGroup != null) return _canvasGroup;
            
            _canvasGroup = GetComponent<CanvasGroup>();
            return _canvasGroup;
        }

        public void HandleTween()
        {
            if (objectToAnimate == null)
            {
                objectToAnimate = gameObject;
            }

            switch (animationType)
            {
                case UIAnimationType.Move:
                    MoveAbsolute();
                    break;
                case UIAnimationType.Fade:
                    Fade();
                    break;
                case UIAnimationType.Scale:
                    Scale();
                    break;
            }
            
            _tweenObject.setDelay(delay);
            _tweenObject.setEase(easeType);

            if (loop)
            {
                _tweenObject.loopCount = int.MaxValue;
            }

            if (pingpong)
            {
                _tweenObject.setLoopPingPong();
            }

            if (disableAfterSeconds)
            {
                StartCoroutine("DisableAfter", disableDuration);
            }

            if (ignoreTimeScale)
            {
                _tweenObject.setIgnoreTimeScale(true);
            }
        }

        public void Fade()
        {
            if (_canvasGroup == null)
            {
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            if (startPositionOffset)
            {
                _canvasGroup.alpha = from;
            }

            _tweenObject = LeanTween.alphaCanvas(_canvasGroup, to, duration);
        }

        public void MoveAbsolute()
        {
            var rectTransform = objectToAnimate.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = fromState;
            _tweenObject = LeanTween.move(rectTransform, toState, duration);
        }

        public void Scale()
        {
            if (startPositionOffset)
            {
                objectToAnimate.GetComponent<RectTransform>().localScale = fromState;
            }

            _tweenObject = LeanTween.scale(objectToAnimate, toState, duration);
        }

        void SwapDirection()
        {
            
            switch (animationType)
            {
                case UIAnimationType.Move:
                    (fromState, toState) = (origToState, origFromState);
                    break;
                case UIAnimationType.Scale:
                    var temp = origFromState;
                    fromState = transform.localScale;
                    toState = temp;
                    break;
                case UIAnimationType.Fade:
                    try
                    {
                        var tempNum = origFrom;
                        from = _canvasGroup.alpha;
                        to = tempNum;
                    }
                    catch
                    {
                        //ignored
                    }
                    break;
            }
        }

        public void Disable()
        {
            disabled = true;
            SwapDirection();
            HandleTween();
            _tweenObject.setOnComplete(() =>
            {
                DisableObjectsToDisable();
                fromState = origFromState;
                toState = origToState;
                from = origFrom;
                to = origTo;
                if (inactiveAfterDone)
                {
                    gameObject.SetActive(false);
                }
            });
        }

        private void DisableObjectsToDisable()
        {
            int objectCount;
            if((objectCount = objectsToDisableOnDisable.Count)<= 0) return;
            
            for (var i = 0; i < objectCount; i++)
            {
                var obj = objectsToDisableOnDisable[i];
                obj.Disable();
            }
        }

        IEnumerator DisableAfter(float time)
        {
            yield return new WaitForSeconds(time);
            Disable();
        }
    }
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(UITweener))]
    public class UITweenerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            UITweener myScript = (UITweener)target;
            if (GUILayout.Button("Disable"))
            {
                myScript.Disable();
            }
        }
    }
    #endif
}
