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
    [ASUIStyle(typeof(Image), 100)]
    public class ImageStyle : IASUIStyle
    {
        public Color color;
        public Sprite sprite;
        public Material material;

        public async Task ApplyStyle(Component component)
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
        public UnityEngine.UIElements.VisualElement CreateUIElementsEditor(Component component = null)
        {
            var container = new UnityEngine.UIElements.VisualElement();
            container.name = "image-style-editor";

            var image = component as Image;
            if (image == null)
            {
                var errorLabel = new UnityEngine.UIElements.Label("错误: 组件类型不匹配，需要Image组件");
                errorLabel.style.color = Color.red;
                container.Add(errorLabel);
                return container;
            }

            // 从组件获取当前值
            color = image.color;
            sprite = image.sprite;
            material = image.material;

            // Color字段
            var colorField = new UnityEditor.UIElements.ColorField("Color");
            colorField.value = color;
            colorField.RegisterCallback<UnityEngine.UIElements.ChangeEvent<Color>>(evt =>
            {
                color = evt.newValue;
                image.color = color;
                EditorUtility.SetDirty(component);
            });
            container.Add(colorField);

            // Sprite字段
            var spriteField = new UnityEditor.UIElements.ObjectField("Sprite");
            spriteField.objectType = typeof(Sprite);
            spriteField.value = sprite;
            spriteField.RegisterCallback<UnityEngine.UIElements.ChangeEvent<UnityEngine.Object>>(evt =>
            {
                sprite = evt.newValue as Sprite;
                image.sprite = sprite;
                EditorUtility.SetDirty(component);
            });
            container.Add(spriteField);

            // Material字段
            var materialField = new UnityEditor.UIElements.ObjectField("Material");
            materialField.objectType = typeof(Material);
            materialField.value = material;
            materialField.RegisterCallback<UnityEngine.UIElements.ChangeEvent<UnityEngine.Object>>(evt =>
            {
                material = evt.newValue as Material;
                image.material = material;
                EditorUtility.SetDirty(component);
            });
            container.Add(materialField);

            return container;
        }
#endif
    }
}
