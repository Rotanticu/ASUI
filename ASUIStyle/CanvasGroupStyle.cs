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
        public void DrawInEditorFoldout(Component component = null)
        {
            var newAlpha = EditorGUILayout.FloatField("Alpha", alpha);
            if (newAlpha != alpha)
            {
                alpha = newAlpha;
                // 实时应用到UI组件
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
