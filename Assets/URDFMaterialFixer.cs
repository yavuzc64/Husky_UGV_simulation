using UnityEngine;
using UnityEditor;

public class URDFMaterialFixer : EditorWindow
{
    [MenuItem("Tools/Fix URDF Materials")]
    static void FixMaterials()
    {
        string[] guids = AssetDatabase.FindAssets("t:Material");
        foreach (string guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat != null && mat.shader.name.Contains("Legacy Shaders"))
            {
                Color c = mat.color;
                mat.shader = Shader.Find("Universal Render Pipeline/Lit");
                mat.SetColor("_BaseColor", c);
                Debug.Log($"Converted: {mat.name}");
            }
        }
    }
}
