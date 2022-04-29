using _Core.Managers;
using UnityEngine;

public class TerrainDetector
{
    private TerrainData terrainData;
    private int alphamapWidth;
    private int alphamapHeight;
    private float[,,] splatmapData;
    private int numTextures;
    private Vector3 _splatPosition = Vector3.zero;
    private Transform _terrainTransform;

    public TerrainDetector()
    {
        if (GameManager.instance == null) return;
        if(GameManager.instance.currentTerrain == null) return;

        Init();
    }

    private void Init()
    {
        terrainData = GameManager.instance.currentTerrain.terrainData;
        _terrainTransform = GameManager.instance.currentTerrain.transform;
        alphamapWidth = terrainData.alphamapWidth;
        alphamapHeight = terrainData.alphamapHeight;
        splatmapData = terrainData.GetAlphamaps(0, 0, alphamapWidth, alphamapHeight);
        numTextures = splatmapData.Length / (alphamapWidth * alphamapHeight);
    }

    private Vector3 ConvertToSplatMapCoordinate(Vector3 worldPosition)
    {
        var terrain = GameManager.instance.currentTerrain;


        if (terrain == null) return _splatPosition;
        
        if(terrainData == null) Init();

        var terPosition = _terrainTransform.position;
        _splatPosition.x = ((worldPosition.x - terPosition.x) / terrain.terrainData.size.x) * terrain.terrainData.alphamapWidth;
        _splatPosition.z = ((worldPosition.z - terPosition.z) / terrain.terrainData.size.z) * terrain.terrainData.alphamapHeight;

        return _splatPosition;
    }

    public int GetActiveTerrainTextureIdx(Vector3 position)
    {
        var terrainCord = ConvertToSplatMapCoordinate(position);
        var activeTerrainIndex = 50;
        var largestOpacity = 0f;

        for (var i = 0; i < numTextures; i++)
        {
            if (!(largestOpacity < splatmapData[(int) terrainCord.z, (int) terrainCord.x, i])) continue;
            
            activeTerrainIndex = i;
            largestOpacity = splatmapData[(int)terrainCord.z, (int)terrainCord.x, i];
        }

        return activeTerrainIndex;
    }

}