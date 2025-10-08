using TMPro;
using UnityEngine;
using System.Threading.Tasks;
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

        public readonly async Task ApplyStyle(Component component)
        {
            TextMeshProUGUI textMeshProUGUI = component as TextMeshProUGUI;
            textMeshProUGUI.text = text;
            textMeshProUGUI.color = color;
            await Task.CompletedTask;
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
                    _ = ApplyStyle(component);
                    EditorUtility.SetDirty(component);
                }
            }
            
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
        }
#endif
    }
}
