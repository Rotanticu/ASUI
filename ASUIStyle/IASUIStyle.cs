
using System;
using TMPro;
using UnityEngine;

namespace ASUI
{
    public interface IASUIStyle
    {
        public void SaveStyle(Component component);
        public void ApplyStyle(Component component);
#if UNITY_EDITOR
        public void DrawInEditorFoldout(Component component = null);
#endif
    }
}
