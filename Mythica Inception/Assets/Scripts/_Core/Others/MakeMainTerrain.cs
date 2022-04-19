using System.Collections;
using System.Collections.Generic;
using _Core.Managers;
using UnityEngine;

public class MakeMainTerrain : MonoBehaviour
{
    public Terrain terrain;

    void Start()
    {
        GameManager.instance.currentTerrain = terrain;
    }
}
