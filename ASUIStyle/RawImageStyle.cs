using System.Security.Policy;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ASUI
{
    [ASUIStyle(typeof(RawImage))]
    public struct RawImageStyle : IASUIStyle
    {
        public Color color;
        public Texture texture;
        public Material material;

        public readonly void ApplyStyle(Component component)
        {
            RawImage rawImage = component as RawImage;
            rawImage.color = color;
            rawImage.texture = texture;
            rawImage.material = material;
        }
        public void SaveStyle(Component component)
        {
            RawImage rawImage = component as RawImage;
            color = rawImage.color;
            texture = rawImage.texture;
            material = rawImage.material;
        }

#if UNITY_EDITOR
        public void DrawInEditorFoldout()
        {
            color = EditorGUILayout.ColorField("Color", color);
            EditorGUILayout.Space();
            texture = (Texture)EditorGUILayout.ObjectField("Texture", texture, typeof(Texture), true);
            EditorGUILayout.Space();
            material = (Material)EditorGUILayout.ObjectField("Material", material, typeof(Material), true);
        }
#endif
    }
}
