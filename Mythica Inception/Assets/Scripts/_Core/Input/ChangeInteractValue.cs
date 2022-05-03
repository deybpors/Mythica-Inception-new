using _Core.Managers;
using UnityEngine;

[CreateAssetMenu(menuName = "Others/Change Interact")]
public class ChangeInteractValue : ScriptableObject
{
    public void ChangeValue(bool value)
    {
        if(GameManager.instance == null) return;
        GameManager.instance.inputHandler.interact = value;
    }
}
