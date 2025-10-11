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
        public void AddAnimationComponents(LitMotionAnimation animation, Component component, IASUIStyle toStyle)
        {
            if (!(toStyle is ImageStyle toImageStyle))
                return;
                
            // 比较color，如果不同则添加color动画
            if (color != toImageStyle.color)
            {
                // 添加color动画组件
                var colorAnimation = new LitMotion.Animation.Components.ImageColorAnimation();
                
                // 使用反射设置受保护的属性
                var targetField = typeof(LitMotion.Animation.PropertyAnimationComponent<Image, Color, LitMotion.NoOptions, LitMotion.Adapters.ColorMotionAdapter>)
                    .GetField("target", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                targetField?.SetValue(colorAnimation, component as Image);
                
                var settingsField = typeof(LitMotion.Animation.PropertyAnimationComponent<Image, Color, LitMotion.NoOptions, LitMotion.Adapters.ColorMotionAdapter>)
                    .GetField("settings", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                settingsField?.SetValue(colorAnimation, new LitMotion.SerializableMotionSettings<Color, LitMotion.NoOptions>
                {
                    StartValue = color,
                    EndValue = toImageStyle.color,
                    Duration = 1.0f,
                    Ease = LitMotion.Ease.OutQuart
                });
                
                ASUIStyleState.AddComponentToAnimation(animation, colorAnimation);
                Debug.Log($"添加Image Color动画: {color} → {toImageStyle.color}");
            }
            
            // 注意：sprite和material通常不需要动画，因为它们是瞬间切换的
            // 如果需要sprite动画，可以添加sprite切换的延迟效果
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
