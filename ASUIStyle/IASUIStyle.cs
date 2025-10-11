
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using LitMotion.Animation;

namespace ASUI
{
    public interface IASUIStyle
    {
        /// <summary>
        /// 保存当前组件的样式
        /// </summary>
        /// <param name="component">要保存样式的组件</param>
        public void SaveStyle(Component component);
        
        /// <summary>
        /// 应用样式到组件
        /// </summary>
        /// <param name="component">要应用样式的组件</param>
        public async Task ApplyStyle(Component component)
        {
            await Task.CompletedTask;
        }
        
        /// <summary>
        /// 为LitMotionAnimation添加动画组件
        /// 在实现类中根据具体的属性差异来决定添加哪些动画组件
        /// </summary>
        /// <param name="animation">目标LitMotionAnimation</param>
        /// <param name="component">目标组件</param>
        /// <param name="toStyle">目标样式</param>
        public void AddAnimationComponents(LitMotionAnimation animation, Component component, IASUIStyle toStyle);
        
#if UNITY_EDITOR
        /// <summary>
        /// 在编辑器中绘制样式配置界面
        /// </summary>
        /// <param name="component">目标组件</param>
        public void DrawInEditorFoldout(Component component = null);
#endif
    }
}
