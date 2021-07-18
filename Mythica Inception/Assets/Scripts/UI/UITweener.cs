using System;
using System.Collections;
using MyBox;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.UI
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

        public UIAnimationType animationType;
        public LeanTweenType easeType;
        public float duration;
        public float delay;
        public bool loop;
        public bool pingpong;
        public bool startPositionOffset;
        [ConditionalField(nameof(animationType), false, UIAnimationType.Scale, UIAnimationType.Move)] 
        public Vector3 fromState;
        [ConditionalField(nameof(animationType), false, UIAnimationType.Scale, UIAnimationType.Move)]
        public Vector3 toState;
        
        [ConditionalField(nameof(animationType), false, UIAnimationType.Fade)]
        public float from;
        [ConditionalField(nameof(animationType), false, UIAnimationType.Fade)]
        public float to;

        public bool disableAfterSeconds;
        [ConditionalField(nameof(disableAfterSeconds))]
        public float disableDuration;

        private LTDescr _tweenObject;
        public bool showOnEnable;
        private CanvasGroup _canvasGroup;

        public void OnEnable()
        {
            _canvasGroup = gameObject.GetComponent<CanvasGroup>();
            if (showOnEnable)
            {
                Show();
            }
        }
        private void Show()
        {
            HandleTween();
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
            if (animationType == UIAnimationType.Scale || animationType == UIAnimationType.Move)
            {
                var temp = fromState;
                fromState = animationType == UIAnimationType.Scale ? transform.localScale : transform.position;
                toState = temp; 
            }
            else
            {
                var temp = from;
                from = objectToAnimate.GetComponent<CanvasGroup>().alpha;
                to = temp; 
            }
        }

        public void Disable()
        {
            SwapDirection();
            HandleTween();
            _tweenObject.setOnComplete(() =>
            {
                SwapDirection();
                gameObject.SetActive(false);
            });
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
