using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace ASUI
{
    [ASUIStyle(typeof(CanvasGroup))]
    public struct CanvasGroupStyle : IASUIStyle
    {
        public float alpha;

        public readonly void ApplyStyle(Component component)
        {
            CanvasGroup canvasGroup = component as CanvasGroup;
            canvasGroup.alpha = alpha;
        }
        public void SaveStyle(Component component)
        {
            CanvasGroup canvasGroup = component as CanvasGroup;
            alpha = canvasGroup.alpha;
        }
#if UNITY_EDITOR
        public void DrawInEditorFoldout()
        {
            alpha = EditorGUILayout.FloatField("Alpha", alpha);
        }
#endif
    }
}
