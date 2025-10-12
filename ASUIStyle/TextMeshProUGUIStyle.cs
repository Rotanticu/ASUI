using TMPro;
using UnityEngine;
using System.Threading.Tasks;
using LitMotion.Animation;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ASUI
{
    [System.Serializable]
    [ASUIStyle(typeof(TextMeshProUGUI), 90)]
    public class TextMeshProUGUIStyle : IASUIStyle
    {
        public string text;
        public Color color;

        public async Task ApplyStyle(Component component)
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
        public void AddAnimationComponents(LitMotionAnimation animation, Component component, IASUIStyle toStyle)
        {
            if (!(toStyle is TextMeshProUGUIStyle toTextStyle))
                return;
                
            // 比较color，如果不同则添加color动画
            if (color != toTextStyle.color)
            {
                // 添加color动画组件
                var colorAnimation = new LitMotion.Animation.Components.GraphicColorAnimation();
                
                // 使用反射设置受保护的属性
                var targetField = typeof(LitMotion.Animation.PropertyAnimationComponent<Graphic, Color, LitMotion.NoOptions, LitMotion.Adapters.ColorMotionAdapter>)
                    .GetField("target", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                targetField?.SetValue(colorAnimation, component as TextMeshProUGUI);
                
                var settingsField = typeof(LitMotion.Animation.PropertyAnimationComponent<Graphic, Color, LitMotion.NoOptions, LitMotion.Adapters.ColorMotionAdapter>)
                    .GetField("settings", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                settingsField?.SetValue(colorAnimation, new LitMotion.SerializableMotionSettings<Color, LitMotion.NoOptions>
                {
                    StartValue = color,
                    EndValue = toTextStyle.color,
                    Duration = 1.0f,
                    Ease = LitMotion.Ease.OutQuart
                });
                
                ASUIStyleState.AddComponentToAnimation(animation, colorAnimation);
                Debug.Log($"添加TextMeshPro Color动画: {color} → {toTextStyle.color}");
            }
            
            // 注意：text通常不需要动画，因为文本是瞬间切换的
            // 如果需要文本动画，可以添加打字机效果
        }
        
#endif

#if UNITY_EDITOR
        public UnityEngine.UIElements.VisualElement CreateUIElementsEditor(Component component = null)
        {
            var container = new UnityEngine.UIElements.VisualElement();
            container.name = "textmeshpro-style-editor";

            var textMeshPro = component as TextMeshProUGUI;
            if (textMeshPro == null)
            {
                var errorLabel = new UnityEngine.UIElements.Label("错误: 组件类型不匹配，需要TextMeshProUGUI组件");
                errorLabel.style.color = Color.red;
                container.Add(errorLabel);
                return container;
            }

            // 从组件获取当前值
            text = textMeshPro.text;
            color = textMeshPro.color;

            // Text字段
            var textField = new UnityEngine.UIElements.TextField("Text");
            textField.value = text;
            textField.RegisterCallback<UnityEngine.UIElements.ChangeEvent<string>>(evt =>
            {
                text = evt.newValue;
                textMeshPro.text = text;
                EditorUtility.SetDirty(component);
            });
            container.Add(textField);

            // Color字段
            var colorField = new UnityEditor.UIElements.ColorField("Color");
            colorField.value = color;
            colorField.RegisterCallback<UnityEngine.UIElements.ChangeEvent<Color>>(evt =>
            {
                color = evt.newValue;
                textMeshPro.color = color;
                EditorUtility.SetDirty(component);
            });
            container.Add(colorField);

            return container;
        }
#endif
    }
}
