using R3;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace ASUI
{
    /// <summary>
    /// 动画状态枚举
    /// </summary>
    public enum AnimationState
    {
        uninitialized, // 未初始化
        Playing,     // 播放中
        Pause,      // 暂停
        Completed,   // 已完成
        Idle,        // 空闲
    }

    public abstract class AnimationProcess<T>
    {
        /// <summary>
        /// 当前状态
        /// </summary>
        public AnimationState CurrentState { get; private set; } = AnimationState.uninitialized;

        /// <summary>
        /// 一次性动画还是持续动画或需要多次播放的动画
        /// 一次性动画会在播放完成后自动销毁
        /// 持续动画或需要多次播放的动画需要手动销毁
        /// </summary>
        public bool IsOnce = false;

        private T m_animationDriver;
        public T AnimationDriver
        {
            get => m_animationDriver;
            protected set => m_animationDriver = value;
        }
        /// <summary>
        /// 开始播放动画
        /// </summary>
        public void PlayForward()
        {
            // if (m_animationDriver == null)
            // {
            //     InitializeAnimationDriver(IsOnce, AnimationUpdateType, animationSpeed, AnimationDriverCompletedCallback);
            // }
            // if (m_animationDriver == null)
            //     return;
            // CurrentState = AnimationState.Idle;
            // StartAnimationDriver();
            // CurrentState = AnimationState.Playing;

            // await Observable.EveryUpdate()
            //         .FirstAsync(_ => CurrentState == AnimationState.Completed || CurrentState == AnimationState.Idle);
        }

        /// <summary>
        /// 暂停动画
        /// </summary>
        public virtual void Pause()
        {
            // if (CurrentState == AnimationState.uninitialized)
            //     return;
            // if (CurrentState == AnimationState.Pause)
            //     return;
            // if (CurrentState == AnimationState.Playing)
            // {
            //     SetAnimationDriverSpeed(0);
            //     CurrentState = AnimationState.Pause;
            // }
        }

        /// <summary>
        /// 恢复动画
        /// </summary>
        public virtual void Resume()
        {
            // if (CurrentState == AnimationState.uninitialized)
            //     return;
            // if (CurrentState != AnimationState.Pause)
            //     return;
            // SetAnimationDriverSpeed(animationSpeed);
            // CurrentState = AnimationState.Playing;
        }

        /// <summary>
        /// 重置动画到初始状态
        /// </summary>
        public virtual void ReSet()
        {
            // if (CurrentState == AnimationState.uninitialized)
            //     return;
            // ReSetAnimationDriver();
            // CurrentState = AnimationState.Idle;
        }
        /// <summary>
        /// 立刻完成动画，会来到动画的结束状态
        /// withCompletedCallback: 是否调用CompletedCallback
        /// </summary>
        public virtual void CompletedImmediately(bool withCompletedCallback)
        {
            // if (CurrentState == AnimationState.uninitialized)
            //     return;
            // CompletedImmediatelyAnimationDriver(withCompletedCallback);
            // CurrentState = AnimationState.Completed;
        }
        /// <summary>
        /// 倒放动画，不能被打断的动画可以倒放
        ///damp动画不能倒放，不修改初始结束值的Tween可以倒放，Unity的Anim可以倒放，后两者不能被打断
        /// </summary>
        //public virtual async Task PlayBackwards()
        //{
        //    if (CurrentState == AnimationState.uninitialized)
        //        return;
        //    CompletedImmediately(false);
        //    SetAnimationSpeed(-AnimationSpeed);
        //    await PlayForward();
        //}
    }
}
