using System.Collections;
using MyBox;
using UnityEngine;

namespace Assets.Scripts._Core.Others
{
    public enum ObjectAnimationType
    {
        Move,
        Scale,
        Rotation
    }
    public class TweenObjectMovement : MonoBehaviour
    {
        public GameObject objectToAnimate;

        public ObjectAnimationType animationType;
        public LeanTweenType easeType;
        public float duration;
        public float delay;
        public bool loop;
        public bool pingpong;
        public bool customFromState;
        [ConditionalField(nameof(customFromState))]
        public Vector3 fromState;
        public Vector3 toState;

        public bool disableAfterSeconds;
        [ConditionalField(nameof(disableAfterSeconds))]
        public float disableDuration;

        private LTDescr _tweenObject;
        public bool showOnEnable;

        public void OnEnable()
        {
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
                case ObjectAnimationType.Move:
                    MoveAbsolute();
                    break;
                case ObjectAnimationType.Rotation:
                    Rotate();
                    break;
                case ObjectAnimationType.Scale:
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

        public void Rotate()
        {
            if (customFromState)
            {
                objectToAnimate.transform.rotation = Quaternion.Euler(fromState.x, fromState.y, fromState.z);
            }

            fromState = new Vector3(objectToAnimate.transform.rotation.x, objectToAnimate.transform.rotation.y,
                objectToAnimate.transform.rotation.z);

            _tweenObject = LeanTween.rotate(objectToAnimate, toState, duration);
        }

        public void MoveAbsolute()
        {
            if (customFromState)
            {
                objectToAnimate.transform.position = fromState;
            }

            fromState = objectToAnimate.transform.position;
            
            _tweenObject = LeanTween.move(objectToAnimate, toState, duration);
        }

        public void Scale()
        {
            if (customFromState)
            {
                objectToAnimate.transform.localScale = fromState;
            }

            fromState = objectToAnimate.transform.localScale;
            
            _tweenObject = LeanTween.scale(objectToAnimate, toState, duration);
        }

        void SwapDirection()
        {
            var temp = fromState;
            fromState = toState;
            toState = temp;
        }

        private void Disable()
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
}
