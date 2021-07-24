using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
public class Object2Terrain : EditorWindow {
 
	[MenuItem("Terrain/Object to Terrain", false, 2000)] static void OpenWindow () {
 
		EditorWindow.GetWindow<Object2Terrain>(true);
	}
 
	private int resolution = 512;
	private Vector3 addTerrain;
	int bottomTopRadioSelected = 0;
	static string[] bottomTopRadio = new string[] { "Bottom Up", "Top Down"};
	private float shiftHeight = 0f;
 
	void OnGUI () {
 
		resolution = EditorGUILayout.IntField("Resolution", resolution);
		addTerrain = EditorGUILayout.Vector3Field("Add terrain", addTerrain);
		shiftHeight = EditorGUILayout.Slider("Shift height", shiftHeight, -1f, 1f);
		bottomTopRadioSelected = GUILayout.SelectionGrid(bottomTopRadioSelected, bottomTopRadio, bottomTopRadio.Length, EditorStyles.radioButton);

		if (!GUILayout.Button("Create Terrain")) return;
		
		if(Selection.activeGameObject == null){
 
			EditorUtility.DisplayDialog("No object selected", "Please select an object.", "Ok");
		}
 
		else{
 
			CreateTerrain();
		}
	}
 
	delegate void CleanUp();
 
	void CreateTerrain(){	
 
		//fire up the progress bar
		ShowProgressBar(1, 100);

		var terrain = new TerrainData {heightmapResolution = resolution};
		var terrainObject = Terrain.CreateTerrainGameObject(terrain);
		Undo.RegisterCreatedObjectUndo(terrainObject, "Object to Terrain");
 
		var collider = Selection.activeGameObject.GetComponent<MeshCollider>();
		CleanUp cleanUp = null;
 
		//Add a collider to our source object if it does not exist.
		//Otherwise raycasting doesn't work.
		if(!collider){
 
			collider = Selection.activeGameObject.AddComponent<MeshCollider>();
			cleanUp = () => DestroyImmediate(collider);
		}
 
		var bounds = collider.bounds;	
		var sizeFactor = bounds.size.y / (bounds.size.y + addTerrain.y);
		terrain.size = bounds.size + addTerrain;
		bounds.size = new Vector3(terrain.size.x, bounds.size.y, terrain.size.z);
 
		// Do raycasting samples over the object to see what terrain heights should be
		var heights = new float[terrain.heightmapResolution, terrain.heightmapResolution];	
		var ray = new Ray(new Vector3(bounds.min.x, bounds.max.y + bounds.size.y, bounds.min.z), -Vector3.up);
		RaycastHit hit;
		var meshHeightInverse = 1 / bounds.size.y;
		var rayOrigin = ray.origin;
 
		var maxHeight = heights.GetLength(0);
		var maxLength = heights.GetLength(1);
 
		var stepXZ = new Vector2(bounds.size.x / maxLength, bounds.size.z / maxHeight);
 
		for(int zCount = 0; zCount < maxHeight; zCount++){
 
			ShowProgressBar(zCount, maxHeight);
 
			for(int xCount = 0; xCount < maxLength; xCount++){
 
				float height = 0.0f;
 
				if(collider.Raycast(ray, out hit, bounds.size.y * 3)){
 
					height = (hit.point.y - bounds.min.y) * meshHeightInverse;
					height += shiftHeight;
 
					//bottom up
					if(bottomTopRadioSelected == 0){
 
						height *= sizeFactor;
					}
 
					//clamp
					if(height < 0){
 
						height = 0;
					}
				}
 
				heights[zCount, xCount] = height;
           		rayOrigin.x += stepXZ[0];
           		ray.origin = rayOrigin;
			}
 
			rayOrigin.z += stepXZ[1];
      		rayOrigin.x = bounds.min.x;
      		ray.origin = rayOrigin;
		}
 
		terrain.SetHeights(0, 0, heights);
		AssetDatabase.CreateAsset(terrain, "Assets/New Terrain.asset");
		
		EditorUtility.ClearProgressBar();
 
		if(cleanUp != null){
 
			cleanUp();    
		}
	}
 
    void ShowProgressBar(float progress, float maxProgress){
 
		var p = progress / maxProgress;
		EditorUtility.DisplayProgressBar("Creating Terrain...", Mathf.RoundToInt(p * 100f)+ " %", p);
	}
}
#endif
