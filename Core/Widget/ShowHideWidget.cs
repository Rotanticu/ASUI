using System.Threading.Tasks;
using UnityEngine;

namespace ASUI
{
    /// <summary>
    /// 支持 Show/Hide 状态的 Widget 基类
    /// 提供便捷的 Show() 和 Hide() 方法，自动处理状态转换
    /// </summary>
    public abstract class ShowHideWidget : WidgetsBase
    {
        /// <summary>
        /// 是否正在显示状态（通过检查当前状态不是 "Hide" 来判断）
        /// </summary>
        public bool IsShowing => TransitionName != "Hide";

        /// <summary>
        /// 显示 Widget
        /// </summary>
        public virtual async Task Show()
        {
            if (TransitionName == "Show") return;
            await Transition("Show");
        }

        /// <summary>
        /// 隐藏 Widget
        /// </summary>
        public virtual async Task Hide()
        {
            if (TransitionName == "Hide") return;
            await Transition("Hide");
        }

        /// <summary>
        /// 当 Widget 开始显示时调用（在状态转换开始时）
        /// </summary>
        protected virtual void OnShow()
        {
            // 子类可以重写此方法
        }

        /// <summary>
        /// 当 Widget 开始隐藏时调用（在状态转换开始时）
        /// </summary>
        protected virtual void OnHide()
        {
            // 子类可以重写此方法
        }

        /// <summary>
        /// 当 Widget 显示完成时调用（在状态转换结束时）
        /// </summary>
        protected virtual void OnShowCompleted()
        {
            // 子类可以重写此方法
        }

        /// <summary>
        /// 当 Widget 隐藏完成时调用（在状态转换结束时）
        /// </summary>
        protected virtual void OnHideCompleted()
        {
            // 子类可以重写此方法
        }

        public override void OnStartTransition(string transitionName)
        {
            base.OnStartTransition(transitionName);
            
            switch (transitionName)
            {
                case "Show":
                    OnShow();
                    break;
                case "Hide":
                    OnHide();
                    break;
            }
        }

        public override void OnEndTransition(string transitionName)
        {
            base.OnEndTransition(transitionName);
            
            switch (transitionName)
            {
                case "Show":
                    OnShowCompleted();
                    break;
                case "Hide":
                    OnHideCompleted();
                    break;
            }
        }
    }
}
