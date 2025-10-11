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
    [ASUIStyle(typeof(RectTransform))]
    public struct RectTransformStyle : IASUIStyle
    {
        public Vector3 position;
        public Vector3 scale;
        public Quaternion rotation;

        public readonly async Task ApplyStyle(Component component)
        {
            RectTransform rectTransform = component as RectTransform;
            rectTransform.position = position;
            rectTransform.localScale = scale;
            rectTransform.rotation = rotation;
            await Task.CompletedTask;
        }
        public void SaveStyle(Component component)
        {
            RectTransform rectTransform = component as RectTransform;
            position = rectTransform.position;
            scale = rectTransform.localScale;
            rotation = rectTransform.rotation;
        }
        
#if UNITY_EDITOR
        public void AddAnimationComponents(LitMotionAnimation animation, Component component, IASUIStyle toStyle)
        {
            if (!(toStyle is RectTransformStyle toRectStyle))
                return;
                
            // 比较position，如果不同则添加position动画
            if (position != toRectStyle.position)
            {
                // 添加position动画组件
                var positionAnimation = new LitMotion.Animation.Components.TransformPositionAnimation();
                
                // 使用反射设置受保护的属性
                var targetField = typeof(LitMotion.Animation.PropertyAnimationComponent<Transform, Vector3, LitMotion.NoOptions, LitMotion.Adapters.Vector3MotionAdapter>)
                    .GetField("target", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                targetField?.SetValue(positionAnimation, component as Transform);
                
                var settingsField = typeof(LitMotion.Animation.PropertyAnimationComponent<Transform, Vector3, LitMotion.NoOptions, LitMotion.Adapters.Vector3MotionAdapter>)
                    .GetField("settings", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                settingsField?.SetValue(positionAnimation, new LitMotion.SerializableMotionSettings<Vector3, LitMotion.NoOptions>
                {
                    StartValue = position,
                    EndValue = toRectStyle.position,
                    Duration = 1.0f,
                    Ease = LitMotion.Ease.OutQuart
                });
                
                var useWorldSpaceField = typeof(LitMotion.Animation.Components.TransformPositionAnimationBase<LitMotion.NoOptions, LitMotion.Adapters.Vector3MotionAdapter>)
                    .GetField("useWorldSpace", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                useWorldSpaceField?.SetValue(positionAnimation, true);
                
                ASUIStyleState.AddComponentToAnimation(animation, positionAnimation);
                Debug.Log($"添加Position动画: {position} → {toRectStyle.position}");
            }
            
            // 比较scale，如果不同则添加scale动画
            if (scale != toRectStyle.scale)
            {
                // 添加scale动画组件
                var scaleAnimation = new LitMotion.Animation.Components.TransformScaleAnimation();
                
                // 使用反射设置受保护的属性
                var targetField = typeof(LitMotion.Animation.PropertyAnimationComponent<Transform, Vector3, LitMotion.NoOptions, LitMotion.Adapters.Vector3MotionAdapter>)
                    .GetField("target", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                targetField?.SetValue(scaleAnimation, component as Transform);
                
                var settingsField = typeof(LitMotion.Animation.PropertyAnimationComponent<Transform, Vector3, LitMotion.NoOptions, LitMotion.Adapters.Vector3MotionAdapter>)
                    .GetField("settings", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                settingsField?.SetValue(scaleAnimation, new LitMotion.SerializableMotionSettings<Vector3, LitMotion.NoOptions>
                {
                    StartValue = scale,
                    EndValue = toRectStyle.scale,
                    Duration = 1.0f,
                    Ease = LitMotion.Ease.OutQuart
                });
                
                ASUIStyleState.AddComponentToAnimation(animation, scaleAnimation);
                Debug.Log($"添加Scale动画: {scale} → {toRectStyle.scale}");
            }
            
            // 比较rotation，如果不同则添加rotation动画
            if (rotation != toRectStyle.rotation)
            {
                // 添加rotation动画组件
                var rotationAnimation = new LitMotion.Animation.Components.TransformRotationAnimation();
                
                // 使用反射设置受保护的属性
                var targetField = typeof(LitMotion.Animation.PropertyAnimationComponent<Transform, Vector3, LitMotion.NoOptions, LitMotion.Adapters.Vector3MotionAdapter>)
                    .GetField("target", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                targetField?.SetValue(rotationAnimation, component as Transform);
                
                var settingsField = typeof(LitMotion.Animation.PropertyAnimationComponent<Transform, Vector3, LitMotion.NoOptions, LitMotion.Adapters.Vector3MotionAdapter>)
                    .GetField("settings", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                settingsField?.SetValue(rotationAnimation, new LitMotion.SerializableMotionSettings<Vector3, LitMotion.NoOptions>
                {
                    StartValue = rotation.eulerAngles,
                    EndValue = toRectStyle.rotation.eulerAngles,
                    Duration = 1.0f,
                    Ease = LitMotion.Ease.OutQuart
                });
                
                var useWorldSpaceField = typeof(LitMotion.Animation.Components.TransformRotationAnimationBase<LitMotion.NoOptions, LitMotion.Adapters.Vector3MotionAdapter>)
                    .GetField("useWorldSpace", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                useWorldSpaceField?.SetValue(rotationAnimation, true);
                
                ASUIStyleState.AddComponentToAnimation(animation, rotationAnimation);
                Debug.Log($"添加Rotation动画: {rotation} → {toRectStyle.rotation}");
            }
        }
        
#endif
#if UNITY_EDITOR
        public void DrawInEditorFoldout(Component component = null)
        {
            var newPosition = EditorGUILayout.Vector3Field("Position", position);
            if (newPosition != position)
            {
                position = newPosition;
                if (component != null)
                {
                    _ = ApplyStyle(component);
                    EditorUtility.SetDirty(component);
                }
            }
            
            EditorGUILayout.Space();
            var newRotation = Quaternion.Euler(EditorGUILayout.Vector3Field("Rotation", rotation.eulerAngles));
            if (newRotation != rotation)
            {
                rotation = newRotation;
                if (component != null)
                {
                    _ = ApplyStyle(component);
                    EditorUtility.SetDirty(component);
                }
            }
            
            EditorGUILayout.Space();
            var newScale = EditorGUILayout.Vector3Field("Scale", scale);
            if (newScale != scale)
            {
                scale = newScale;
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
