using UnityEngine;
using UnityEngine.AI;


public class NavMeshBaker : MonoBehaviour
{
    public NavMeshSurface[] navMeshSurfaces;

    public void BuildNavMesh()
    {
        var navLength = navMeshSurfaces.Length;
        for (var i = 0; i < navLength; i++)
        {
            navMeshSurfaces[i].BuildNavMesh();
        }
    }
}
