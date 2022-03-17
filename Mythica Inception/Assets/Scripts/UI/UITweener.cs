using System.Collections;
using System.Collections.Generic;
using MyBox;
using UnityEditor;
using UnityEngine;

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

        public List<GameObject> objectsToDisableOnDisable;
        public bool disableAfterSeconds;

        [ConditionalField(nameof(disableAfterSeconds))]
        public float disableDuration;

        private LTDescr _tweenObject;
        public bool showOnEnable;
        private CanvasGroup _canvasGroup;

        public void OnEnable()
        {
            _canvasGroup = gameObject.GetComponent<CanvasGroup>();
            
            origFrom = from;
            origFromState = fromState;
            origTo = to;
            origToState = toState;
            
            if (showOnEnable)
            {
                Show();
            }
        }
        private void Show()
        {
            HandleTween();
        }

        public LTDescr GetTweenObject()
        {
            return _tweenObject;
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
            objectToAnimate.GetComponent<RectTransform>().anchoredPosition = fromState;
            _tweenObject = LeanTween.move(objectToAnimate.GetComponent<RectTransform>(), toState, duration);
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
                    (fromState, toState) = (toState, fromState);
                    break;
                case UIAnimationType.Scale:
                    var temp = fromState;
                    fromState = transform.localScale;
                    toState = temp;
                    break;
                case UIAnimationType.Fade:
                    var tempNum = from;
                    from = _canvasGroup.alpha;
                    to = tempNum;
                    break;
            }
        }

        public void Disable()
        {
            SwapDirection();
            HandleTween();
            _tweenObject.setOnComplete(() =>
            {
                DisableObjectsToDisable();
                from = origFrom;
                to = origTo;
                fromState = origFromState;
                toState = origToState;
                gameObject.SetActive(false);
            });
        }

        private void DisableObjectsToDisable()
        {
            int objectCount;
            if((objectCount = objectsToDisableOnDisable.Count)<= 0) return;
            
            for (var i = 0; i < objectCount; i++)
            {
                var obj = objectsToDisableOnDisable[i];
                obj.SetActive(false);
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
