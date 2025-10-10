using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;

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
