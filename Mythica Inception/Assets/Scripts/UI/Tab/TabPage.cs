using UnityEngine;

namespace Assets.Scripts.UI
{
    public abstract class TabPage : MonoBehaviour
    {
        void OnEnable()
        {
            OnActive();
        }

        protected abstract void OnActive();
    }
}
