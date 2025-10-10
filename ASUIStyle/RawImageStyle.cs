using System.Security.Policy;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ASUI
{
    [System.Serializable]
    [ASUIStyle(typeof(RawImage))]
    public struct RawImageStyle : IASUIStyle
    {
        public Color color;
        public Texture texture;
        public Material material;

        public readonly async Task ApplyStyle(Component component)
        {
            RawImage rawImage = component as RawImage;
            rawImage.color = color;
            rawImage.texture = texture;
            rawImage.material = material;
            await Task.CompletedTask;
        }
        public void SaveStyle(Component component)
        {
            RawImage rawImage = component as RawImage;
            color = rawImage.color;
            texture = rawImage.texture;
            material = rawImage.material;
        }

#if UNITY_EDITOR
        public void DrawInEditorFoldout(Component component = null)
        {
            var newColor = EditorGUILayout.ColorField("Color", color);
            if (newColor != color)
            {
                color = newColor;
                if (component != null)
                {
                    _ = ApplyStyle(component);
                    EditorUtility.SetDirty(component);
                }
            }
            
            EditorGUILayout.Space();
            var newTexture = (Texture)EditorGUILayout.ObjectField("Texture", texture, typeof(Texture), true);
            if (newTexture != texture)
            {
                texture = newTexture;
                if (component != null)
                {
                    _ = ApplyStyle(component);
                    EditorUtility.SetDirty(component);
                }
            }
            
            EditorGUILayout.Space();
            var newMaterial = (Material)EditorGUILayout.ObjectField("Material", material, typeof(Material), true);
            if (newMaterial != material)
            {
                material = newMaterial;
                if (component != null)
                {
                    _ = ApplyStyle(component);
                    EditorUtility.SetDirty(component);
                }
            }
        }
#endif
    }
}
