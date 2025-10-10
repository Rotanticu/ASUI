using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ASUI
{
    [System.Serializable]
    [ASUIStyle(typeof(Image))]
    public struct ImageStyle : IASUIStyle
    {
        public Color color;
        public Sprite sprite;
        public Material material;

        public readonly async Task ApplyStyle(Component component)
        {
            Image image = component as Image;
            image.color = color;
            image.sprite = sprite;
            image.material = material;
            await Task.CompletedTask;
        }
        public void SaveStyle(Component component)
        {
            Image image = component as Image;
            color = image.color;
            sprite = image.sprite;
            material = image.material;
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
            var newSprite = (Sprite)EditorGUILayout.ObjectField("Sprite", sprite, typeof(Sprite), true);
            if (newSprite != sprite)
            {
                sprite = newSprite;
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
