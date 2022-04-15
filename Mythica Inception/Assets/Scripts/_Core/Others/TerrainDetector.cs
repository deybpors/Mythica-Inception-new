using UnityEngine;

public class TerrainDetector
{
    private TerrainData terrainData;
    private int alphamapWidth;
    private int alphamapHeight;
    private float[,,] splatmapData;
    private int numTextures;
    private Terrain activeTerrain;
    private Transform activeTerrainTransform;
    private readonly Vector3 _zero = Vector3.zero;

    public TerrainDetector()
    {
        activeTerrain = Terrain.activeTerrain;
        
        if(activeTerrain == null) return;
        activeTerrainTransform = activeTerrain.transform;
        terrainData = activeTerrain.terrainData;
        alphamapWidth = terrainData.alphamapWidth;
        alphamapHeight = terrainData.alphamapHeight;

        splatmapData = terrainData.GetAlphamaps(0, 0, alphamapWidth, alphamapHeight);
        numTextures = splatmapData.Length / (alphamapWidth * alphamapHeight);
    }

    private Vector3 ConvertToSplatMapCoordinate(Vector3 worldPosition)
    {
        if (activeTerrain == null) return _zero;

        Vector3 splatPosition = new Vector3();
        Vector3 terPosition = activeTerrainTransform.position;
        splatPosition.x = ((worldPosition.x - terPosition.x) / activeTerrain.terrainData.size.x) * activeTerrain.terrainData.alphamapWidth;
        splatPosition.z = ((worldPosition.z - terPosition.z) / activeTerrain.terrainData.size.z) * activeTerrain.terrainData.alphamapHeight;
        return splatPosition;
    }

    public int GetActiveTerrainTextureIdx(Vector3 position)
    {
        Vector3 terrainCord = ConvertToSplatMapCoordinate(position);
        int activeTerrainIndex = 0;
        float largestOpacity = 0f;

        for (int i = 0; i < numTextures; i++)
        {
            if (!(largestOpacity < splatmapData[(int) terrainCord.z, (int) terrainCord.x, i])) continue;
            
            activeTerrainIndex = i;
            largestOpacity = splatmapData[(int)terrainCord.z, (int)terrainCord.x, i];
        }

        return activeTerrainIndex;
    }

}