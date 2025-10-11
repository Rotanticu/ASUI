using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using LitMotion.Animation;

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
        public void AddAnimationComponents(LitMotionAnimation animation, Component component, IASUIStyle toStyle)
        {
            if (!(toStyle is CanvasGroupStyle toCanvasGroupStyle))
                return;
                
            // 比较alpha，如果不同则添加alpha动画
            if (alpha != toCanvasGroupStyle.alpha)
            {
                // 添加alpha动画组件
                var alphaAnimation = new LitMotion.Animation.Components.CanvasGroupAlphaAnimation();
                
                // 使用反射设置受保护的属性
                var targetField = typeof(LitMotion.Animation.PropertyAnimationComponent<CanvasGroup, float, LitMotion.NoOptions, LitMotion.Adapters.FloatMotionAdapter>)
                    .GetField("target", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                targetField?.SetValue(alphaAnimation, component as CanvasGroup);
                
                var settingsField = typeof(LitMotion.Animation.PropertyAnimationComponent<CanvasGroup, float, LitMotion.NoOptions, LitMotion.Adapters.FloatMotionAdapter>)
                    .GetField("settings", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                settingsField?.SetValue(alphaAnimation, new LitMotion.SerializableMotionSettings<float, LitMotion.NoOptions>
                {
                    StartValue = alpha,
                    EndValue = toCanvasGroupStyle.alpha,
                    Duration = 1.0f,
                    Ease = LitMotion.Ease.OutQuart
                });
                
                ASUIStyleState.AddComponentToAnimation(animation, alphaAnimation);
                Debug.Log($"添加CanvasGroup Alpha动画: {alpha} → {toCanvasGroupStyle.alpha}");
            }
        }
        
#endif
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
