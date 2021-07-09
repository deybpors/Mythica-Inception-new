using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class ProgressBarUI : MonoBehaviour
    {
        [HideInInspector] public float maxValue;
        [HideInInspector] public float currentValue;
        
        public Image sliderColored;
        public Image sliderBack;
        public float smoothValue;
        private float _sliderSupposedValue;

        void LateUpdate()
        {
            _sliderSupposedValue = currentValue / maxValue;
            if (sliderColored.fillAmount >= _sliderSupposedValue)
            {
                sliderBack.fillAmount = Mathf.Lerp(sliderBack.fillAmount, _sliderSupposedValue, smoothValue * Time.deltaTime);
                sliderColored.fillAmount = _sliderSupposedValue;
            }
            else if (sliderColored.fillAmount < _sliderSupposedValue)
            {
                sliderColored.fillAmount = Mathf.Lerp(sliderColored.fillAmount, _sliderSupposedValue, smoothValue * Time.deltaTime);
                sliderBack.fillAmount = _sliderSupposedValue;
            }
        }
    }
}