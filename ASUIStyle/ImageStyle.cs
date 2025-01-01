using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ASUI
{
    [ASUIStyle(typeof(Image))]
    public struct ImageStyle : IASUIStyle
    {
        public Color color;
        public Sprite sprite;
        public Material material;

        public readonly void ApplyStyle(Component component)
        {
            Image image = component as Image;
            image.color = color;
            image.sprite = sprite;
            image.material = material;
        }
        public void SaveStyle(Component component)
        {
            Image image = component as Image;
            color = image.color;
            sprite = image.sprite;
            material = image.material;
        }

#if UNITY_EDITOR
        public void DrawInEditorFoldout()
        {
            color = EditorGUILayout.ColorField("Color", color);
            EditorGUILayout.Space();
            sprite = (Sprite)EditorGUILayout.ObjectField("Sprite", sprite, typeof(Sprite), true);
            EditorGUILayout.Space();
            material = (Material)EditorGUILayout.ObjectField("Material", material, typeof(Material), true);
        }
#endif
    }
}
