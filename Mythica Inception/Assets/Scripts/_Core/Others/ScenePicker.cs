using UnityEngine;
using UnityEditor;

namespace _Core.Others
{
    public class ScenePicker : MonoBehaviour
    {
        [SerializeField]
        public string path;
    }
    
    [CustomEditor(typeof(ScenePicker))]
    public class ScenePickerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var worldPicker = target as ScenePicker;
            var oldScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(worldPicker.path);
    
            serializedObject.Update();
    
            EditorGUI.BeginChangeCheck();
            var newScene = EditorGUILayout.ObjectField("Scene", oldScene, typeof(SceneAsset), false) as SceneAsset;
    
            if (EditorGUI.EndChangeCheck())
            {
                var newPath = AssetDatabase.GetAssetPath(newScene);
                var scenePathProperty = serializedObject.FindProperty("path");
                scenePathProperty.stringValue = newPath;
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}