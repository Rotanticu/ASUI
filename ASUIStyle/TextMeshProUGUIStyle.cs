using TMPro;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ASUI
{
    [ASUIStyle(typeof(TextMeshProUGUI))]
    public struct TextMeshProUGUIStyle : IASUIStyle
    {
        public string text;
        public Color color;

        public readonly void ApplyStyle(Component component)
        {
            TextMeshProUGUI textMeshProUGUI = component as TextMeshProUGUI;
            textMeshProUGUI.text = text;
            textMeshProUGUI.color = color;
        }
        public void SaveStyle(Component component)
        {
            TextMeshProUGUI textMeshProUGUI = component as TextMeshProUGUI;
            text = textMeshProUGUI.text;
            color = textMeshProUGUI.color;
        }

#if UNITY_EDITOR
        public void DrawInEditorFoldout(Component component = null)
        {
            var newText = EditorGUILayout.TextField("Text", text, GUILayout.ExpandWidth(true));
            if (newText != text)
            {
                text = newText;
                if (component != null)
                {
                    ApplyStyle(component);
                    EditorUtility.SetDirty(component);
                }
            }
            
            var newColor = EditorGUILayout.ColorField("Color", color);
            if (newColor != color)
            {
                color = newColor;
                if (component != null)
                {
                    ApplyStyle(component);
                    EditorUtility.SetDirty(component);
                }
            }
        }
#endif
    }
}
