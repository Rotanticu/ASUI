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
    [ASUIStyle(typeof(RawImage), 80)]
    public class RawImageStyle : IASUIStyle
    {
        public Color color;
        public Texture texture;
        public Material material;

        public async Task ApplyStyle(Component component)
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
        public UnityEngine.UIElements.VisualElement CreateUIElementsEditor(Component component = null)
        {
            var container = new UnityEngine.UIElements.VisualElement();
            container.name = "raw-image-style-editor";

            var rawImage = component as RawImage;
            if (rawImage == null)
            {
                var errorLabel = new UnityEngine.UIElements.Label("错误: 组件类型不匹配，需要RawImage组件");
                errorLabel.style.color = Color.red;
                container.Add(errorLabel);
                return container;
            }

            // 从组件获取当前值
            color = rawImage.color;
            texture = rawImage.texture;
            material = rawImage.material;

            // Color字段
            var colorField = new UnityEditor.UIElements.ColorField("Color");
            colorField.value = color;
            colorField.RegisterCallback<UnityEngine.UIElements.ChangeEvent<Color>>(evt =>
            {
                color = evt.newValue;
                rawImage.color = color;
                EditorUtility.SetDirty(component);
            });
            container.Add(colorField);

            // Texture字段
            var textureField = new UnityEditor.UIElements.ObjectField("Texture");
            textureField.objectType = typeof(Texture);
            textureField.value = texture;
            textureField.RegisterCallback<UnityEngine.UIElements.ChangeEvent<UnityEngine.Object>>(evt =>
            {
                texture = evt.newValue as Texture;
                rawImage.texture = texture;
                EditorUtility.SetDirty(component);
            });
            container.Add(textureField);

            // Material字段
            var materialField = new UnityEditor.UIElements.ObjectField("Material");
            materialField.objectType = typeof(Material);
            materialField.value = material;
            materialField.RegisterCallback<UnityEngine.UIElements.ChangeEvent<UnityEngine.Object>>(evt =>
            {
                material = evt.newValue as Material;
                rawImage.material = material;
                EditorUtility.SetDirty(component);
            });
            container.Add(materialField);

            return container;
        }
#endif
    }
}
