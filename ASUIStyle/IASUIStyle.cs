
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace ASUI
{
    public interface IASUIStyle
    {
        public void SaveStyle(Component component);
        public async Task ApplyStyle(Component component)
        {
            await Task.CompletedTask;
        }
#if UNITY_EDITOR
        public void DrawInEditorFoldout(Component component = null);
#endif
    }
}
