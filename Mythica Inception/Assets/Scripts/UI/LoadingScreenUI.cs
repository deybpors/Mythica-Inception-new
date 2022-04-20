using UI;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class LoadingScreenUI : MonoBehaviour
    {
        public UITweener tweener;
        public ProgressBarUI progressBar;
        public Camera loadingScreenCamera;

        [HideInInspector] public GameObject thisGameObject;

        void Start()
        {
            thisGameObject = gameObject;
        }
    }
}
