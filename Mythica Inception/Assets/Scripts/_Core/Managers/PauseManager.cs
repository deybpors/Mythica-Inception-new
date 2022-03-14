using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public bool paused;
    public void PauseGameplay(float timeScale)
    {
        Time.timeScale = timeScale;
    }
}
