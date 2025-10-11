using System.Security.Policy;
using TMPro;
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
        public void AddAnimationComponents(LitMotionAnimation animation, Component component, IASUIStyle toStyle)
        {
            if (!(toStyle is RawImageStyle toRawImageStyle))
                return;
                
            // 比较color，如果不同则添加color动画
            if (color != toRawImageStyle.color)
            {
                // 添加color动画组件
                var colorAnimation = new LitMotion.Animation.Components.GraphicColorAnimation();
                
                // 使用反射设置受保护的属性
                var targetField = typeof(LitMotion.Animation.PropertyAnimationComponent<Graphic, Color, LitMotion.NoOptions, LitMotion.Adapters.ColorMotionAdapter>)
                    .GetField("target", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                targetField?.SetValue(colorAnimation, component as RawImage);
                
                var settingsField = typeof(LitMotion.Animation.PropertyAnimationComponent<Graphic, Color, LitMotion.NoOptions, LitMotion.Adapters.ColorMotionAdapter>)
                    .GetField("settings", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                settingsField?.SetValue(colorAnimation, new LitMotion.SerializableMotionSettings<Color, LitMotion.NoOptions>
                {
                    StartValue = color,
                    EndValue = toRawImageStyle.color,
                    Duration = 1.0f,
                    Ease = LitMotion.Ease.OutQuart
                });
                
                ASUIStyleState.AddComponentToAnimation(animation, colorAnimation);
                Debug.Log($"添加RawImage Color动画: {color} → {toRawImageStyle.color}");
            }
            
            // 注意：texture和material通常不需要动画，因为它们是瞬间切换的
        }
        
#endif

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
