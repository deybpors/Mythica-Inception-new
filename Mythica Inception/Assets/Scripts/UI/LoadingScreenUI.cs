using UI;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class LoadingScreenUI : MonoBehaviour
    {
        public UITweener tweener;
        public ProgressBarUI progressBar;
        public Camera loadScreenCamera;

        [HideInInspector] public GameObject thisGameObject;
        [HideInInspector] public GameObject loadingScreeenCameraObj;

        void Start()
        {
            Initialize();
        }

        public void Initialize()
        {
            thisGameObject = gameObject;
            loadingScreeenCameraObj = loadScreenCamera.gameObject;
        }
    }
}
