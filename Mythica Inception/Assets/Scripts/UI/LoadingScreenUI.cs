using UI;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class LoadingScreenUI : MonoBehaviour
    {
        public UITweener tweener;
        public ProgressBarUI progressBar;

        [HideInInspector] public GameObject thisGameObject;

        void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            thisGameObject = gameObject;
        }
    }
}
