using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.UI
{
    [System.Serializable]
    public class DebugCommand
    {

        public string commandId;
        [TextArea(3,5)]
        public string commandDescription;
        public UnityEvent action;

        public void InvokeCommand()
        {
            action?.Invoke();
        }
    }
}

