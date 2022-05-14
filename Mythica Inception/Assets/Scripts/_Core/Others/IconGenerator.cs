using MyBox;
using UnityEditor;
using UnityEngine;
using Application = UnityEngine.Application;

public class IconGenerator : MonoBehaviour
{
    [SerializeField] private Camera _camera;

    [Foldout("File Settings", true)]
    [OverrideLabel("File Name")] public string fileName;
    [OverrideLabel("Path in Assets")] [SerializeField] private string _pathInAssets;

    [Foldout("Image Properties")]
    [OverrideLabel("Size")] [SerializeField] private Vector2Int _imageSize;

    public void TakeScreenshot()
    {
        if (fileName == string.Empty)
        {
            Debug.LogErrorFormat("Please input a file name for the screenshot.");
        }
        var fullPath = Application.dataPath + _pathInAssets + "/" + fileName + ".png";

        if (_camera == null) _camera = GetComponent<Camera>();
        var renderTexture = new RenderTexture(_imageSize.x, _imageSize.y, 24);
        _camera.targetTexture = renderTexture;
        var screenShot = new Texture2D(_imageSize.x, _imageSize.y, TextureFormat.RGBA32, false);
        _camera.Render();
        RenderTexture.active = renderTexture;
        screenShot.ReadPixels(new Rect(0,0, _imageSize.x, _imageSize.y),0,0);
        _camera.targetTexture = null;
        RenderTexture.active = null;

        if (Application.isEditor)
        {
            DestroyImmediate(renderTexture);
        }
        else
        {
            Destroy(renderTexture);
        }

        var bytes = screenShot.EncodeToPNG();
        System.IO.File.WriteAllBytes(fullPath, bytes);
    }
}

#if UNITY_EDITOR
    [CustomEditor(typeof(IconGenerator))]
    public class IconGeneratorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var iconGenerator = (IconGenerator)target;
            if (!GUILayout.Button("Screenshot")) return;
            
            iconGenerator.TakeScreenshot();
            iconGenerator.fileName = "";
            AssetDatabase.Refresh();
        }
    }
#endif
