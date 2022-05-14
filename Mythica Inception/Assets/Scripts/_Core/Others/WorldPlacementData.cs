using System;
using UnityEngine;

[Serializable]
public class WorldPlacementData
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 localScale;

    public WorldPlacementData(Vector3 position, Quaternion rotation, Vector3 localScale)
    {
        this.position = position;
        this.rotation = rotation;
        this.localScale = localScale;
    }
}