
using System;
using TMPro;
using UnityEngine;

namespace ASUI
{
    public interface IASUIStyle
    {
        public void SaveStyle(Component component);
        public void ApplyStyle(Component component);
        public void DrawInEditorFoldout();
    }
}
