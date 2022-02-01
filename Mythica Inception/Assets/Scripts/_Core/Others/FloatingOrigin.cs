// Based on the Unity Wiki FloatingOrigin script by Peter Stirling
// URL: http://wiki.unity3d.com/index.php/Floating_Origin

using _Core.Managers;
using Cinemachine;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class FloatingOrigin : MonoBehaviour
{
    [Tooltip("Point of reference from which to check the distance to origin.")]
    public Transform ReferenceObject = null;

    [Tooltip("Distance from the origin the reference object must be in order to trigger an origin shift.")]
    public float Threshold = 5000f;

    [Header("Options")]
    [Tooltip("When true, origin shifts are considered only from the horizontal distance to orign.")]
    public bool Use2DDistance = false;

    [Tooltip("When true, updates ALL open scenes. When false, updates only the active scene.")]
    public bool UpdateAllScenes = true;

    [Tooltip("Should ParticleSystems be moved with an origin shift.")]
    public bool UpdateParticles = true;

    [Tooltip("Should TrailRenderers be moved with an origin shift.")]
    public bool UpdateTrailRenderers = true;

    [Tooltip("Should LineRenderers be moved with an origin shift.")]
    public bool UpdateLineRenderers = true;

    private Vector3 referencePosition;

    private ParticleSystem.Particle[] parts = null;

    void LateUpdate()
    {
        if (ReferenceObject == null)
            return;

        referencePosition = ReferenceObject.position;

        if (Use2DDistance)
            referencePosition.y = 0f;

        if (!(referencePosition.magnitude > Threshold) || GameManager.instance.enemiesSeePlayer.Count > 0) return;

        //Debug.Log("repositioning");
        
        MoveRootTransforms(referencePosition);

        MoveCinemachineCameras(referencePosition);

        MoveNavMeshAgent(referencePosition);

        if (UpdateParticles)
            MoveParticles(referencePosition);

        if (UpdateTrailRenderers)
            MoveTrailRenderers(referencePosition);

        if (UpdateLineRenderers)
            MoveLineRenderers(referencePosition);
    }

    private void MoveNavMeshData(NavMeshBaker baker)
    {
        baker.BuildNavMesh();
    }

    private void MoveRootTransforms(Vector3 offset)
    {
        var navMeshTransferred = false;

        if (UpdateAllScenes)
        {
            for (var i = 0; i < SceneManager.sceneCount; i++)
            {
                var scenes = SceneManager.GetSceneAt(i).GetRootGameObjects();
                var scenesLength = scenes.Length;
                for (var j = 0; j < scenesLength; j++)
                {
                    var objectInScene = scenes[j];
                    objectInScene.transform.position -= offset;
                    
                    var navMeshBaker = objectInScene.GetComponent<NavMeshBaker>();
                    if (navMeshBaker == null || navMeshTransferred) continue;
                    MoveNavMeshData(navMeshBaker);
                    navMeshTransferred = true;
                }
            }
        }
        else
        {
            var scenes = SceneManager.GetActiveScene().GetRootGameObjects();
            var scenesLength = scenes.Length;
            for (var i = 0; i < scenesLength; i++)
            {
                var objectInScene = scenes[i];
                objectInScene.transform.position -= offset;

                var navMeshBaker = objectInScene.GetComponent<NavMeshBaker>();
                if (navMeshBaker == null || navMeshTransferred) continue;
                MoveNavMeshData(navMeshBaker);
                navMeshTransferred = true;
            }
        }
    }

    private void MoveCinemachineCameras(Vector3 posDelta)
    {
        var vcamCount = CinemachineCore.Instance.VirtualCameraCount;
        for (var i = 0; i < vcamCount; ++i)
        {
            CinemachineCore.Instance.GetVirtualCamera(i).OnTargetObjectWarped(ReferenceObject, -posDelta);
        }
    }

    private void MoveNavMeshAgent(Vector3 offset)
    {
        var agents = FindObjectsOfType<NavMeshAgent>() as NavMeshAgent[];
        foreach (var agent in agents)
        {
            agent.Warp(agent.transform.position - offset);
        }
    }

    private void MoveTrailRenderers(Vector3 offset)
    {
        var trails = FindObjectsOfType<TrailRenderer>() as TrailRenderer[];
        foreach (var trail in trails)
        {
            var positions = new Vector3[trail.positionCount];

            var positionCount = trail.GetPositions(positions);
            for (var i = 0; i < positionCount; ++i)
                positions[i] -= offset;

            trail.SetPositions(positions);
        }
    }

    private void MoveLineRenderers(Vector3 offset)
    {
        var lines = FindObjectsOfType<LineRenderer>() as LineRenderer[];
        foreach (var line in lines)
        {
            var positions = new Vector3[line.positionCount];

            var positionCount = line.GetPositions(positions);
            for (var i = 0; i < positionCount; ++i)
                positions[i] -= offset;

            line.SetPositions(positions);
        }
    }

    private void MoveParticles(Vector3 offset)
    {
        var particles = FindObjectsOfType<ParticleSystem>() as ParticleSystem[];
        foreach (var system in particles)
        {
            if (system.main.simulationSpace != ParticleSystemSimulationSpace.World)
                continue;

            var particlesNeeded = system.main.maxParticles;

            if (particlesNeeded <= 0)
                continue;

            var wasPaused = system.isPaused;
            var wasPlaying = system.isPlaying;

            if (!wasPaused)
                system.Pause();

            // ensure a sufficiently large array in which to store the particles
            if (parts == null || parts.Length < particlesNeeded)
            {
                parts = new ParticleSystem.Particle[particlesNeeded];
            }

            // now get the particles
            var num = system.GetParticles(parts);

            for (var i = 0; i < num; i++)
            {
                parts[i].position -= offset;
            }

            system.SetParticles(parts, num);

            if (wasPlaying)
                system.Play();
        }
    }
}