using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ASUI
{
    [ASUIStyle(typeof(RectTransform))]
    public struct RectTransformStyle : IASUIStyle
    {
        public Vector3 position;
        public Vector3 scale;
        public Quaternion rotation;

        public readonly void ApplyStyle(Component component)
        {
            RectTransform rectTransform = component as RectTransform;
            rectTransform.position = position;
            rectTransform.localScale = scale;
            rectTransform.rotation = rotation;
        }
        public void SaveStyle(Component component)
        {
            RectTransform rectTransform = component as RectTransform;
            position = rectTransform.position;
            scale = rectTransform.localScale;
            rotation = rectTransform.rotation;
        }
#if UNITY_EDITOR
        public void DrawInEditorFoldout()
        {
            position = EditorGUILayout.Vector3Field("Position", position);
            EditorGUILayout.Space();
            rotation = Quaternion.Euler(EditorGUILayout.Vector3Field("Rotation", rotation.eulerAngles));
            EditorGUILayout.Space();
            scale = EditorGUILayout.Vector3Field("Scale", scale);
        }
#endif
    }
}
