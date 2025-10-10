using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

#if UNITY_EDITOR
using UnityEditor;
#endif
namespace ASUI
{
    [System.Serializable]
    [ASUIStyle(typeof(CanvasGroup))]
    public struct CanvasGroupStyle : IASUIStyle
    {
        public float alpha;

        public readonly async Task ApplyStyle(Component component)
        {
            CanvasGroup canvasGroup = component as CanvasGroup;
            canvasGroup.alpha = alpha;
            await Task.CompletedTask;
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
                    _ = ApplyStyle(component);
                    EditorUtility.SetDirty(component);
                }
            }
        }
#endif
    }
}
