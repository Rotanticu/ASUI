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

    public enum AnimationUpdateType
    {
        /// <summary>
        /// 动画器与 Update 调用同步更新，其速度与当前时间刻度一致。如果时间刻度变慢，动画也会随之减慢以匹配
        /// </summary>
        TimeScaleUpdate,
        /// <summary>
        /// 动画器与 Update 调用同步更新，但是速度与当前时间刻度无关。如果时间刻度变慢，动画仍然会以相同的速度播放
        /// </summary>
        UnscaledTimeUpdate,
        /// <summary>
        /// 动画器与 FixedUpdate 调用同步更新（即与物理系统保持步调一致）
        /// </summary>
        FixedUpdate,
    }

    public abstract class AnimationProcess<T> : IAnimationBehavior
    {
        /// <summary>
        /// �����ĵ�ǰ״̬
        /// </summary>
        public AnimationState CurrentState { get; private set; } = AnimationState.uninitialized;

        /// <summary>
        /// 一次性动画还是持续动画或需要多次播放的动画
        /// 一次性动画会在播放完成后自动销毁
        /// 持续动画或需要多次播放的动画需要手动销毁
        /// </summary>
        public bool IsOnce = false;

        public AnimationUpdateType AnimationUpdateType = AnimationUpdateType.TimeScaleUpdate;

        /// <summary>
        /// 动画速度
        /// </summary>
        private float animationSpeed = 1f;

        protected float originalSpeed;

        private T m_animationDriver;
        public T AnimationDriver
        {
            get => m_animationDriver;
            protected set => m_animationDriver = value;
        }
        /// <summary>
        /// 开始播放动画
        /// </summary>
        public async Task PlayForward()
        {
            if (m_animationDriver == null)
            {
                InitializeAnimationDriver(IsOnce, AnimationUpdateType, animationSpeed, AnimationDriverCompletedCallback);
            }
            if (m_animationDriver == null)
                return;
            CurrentState = AnimationState.Idle;
            StartAnimationDriver();
            CurrentState = AnimationState.Playing;

            await Observable.EveryUpdate()
                    .FirstAsync(_ => CurrentState == AnimationState.Completed || CurrentState == AnimationState.Idle);
        }

        /// <summary>
        /// 暂停动画
        /// </summary>
        public virtual void Pause()
        {
            if (CurrentState == AnimationState.uninitialized)
                return;
            if (CurrentState == AnimationState.Pause)
                return;
            if (CurrentState == AnimationState.Playing)
            {
                SetAnimationDriverSpeed(0);
                CurrentState = AnimationState.Pause;
            }
        }

        /// <summary>
        /// 恢复动画
        /// </summary>
        public virtual void Resume()
        {
            if (CurrentState == AnimationState.uninitialized)
                return;
            if (CurrentState != AnimationState.Pause)
                return;
            SetAnimationDriverSpeed(animationSpeed);
            CurrentState = AnimationState.Playing;
        }

        /// <summary>
        /// 重置动画到初始状态
        /// </summary>
        public virtual void ReSet()
        {
            if (CurrentState == AnimationState.uninitialized)
                return;
            ReSetAnimationDriver();
            CurrentState = AnimationState.Idle;
        }
        /// <summary>
        /// 立刻完成动画，会来到动画的结束状态
        /// withCompletedCallback: 是否调用CompletedCallback
        /// </summary>
        public virtual void CompletedImmediately(bool withCompletedCallback)
        {
            if (CurrentState == AnimationState.uninitialized)
                return;
            CompletedImmediatelyAnimationDriver(withCompletedCallback);
            CurrentState = AnimationState.Completed;
        }

        public virtual void SetAnimationSpeed(float speed)
        {
            if (CurrentState == AnimationState.uninitialized)
                return;
            animationSpeed = speed;
            if (CurrentState != AnimationState.Pause)
                SetAnimationDriverSpeed(animationSpeed);

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

        private void AnimationDriverCompletedCallback()
        {
            CurrentState = AnimationState.Completed;
        }

        /// <summary>
        /// 初始化动画驱动器
        /// </summary>
        protected abstract void InitializeAnimationDriver(bool isOnce, AnimationUpdateType animationUpdateType, float animationSpeed, Action completedCallback);

        /// <summary>
        /// 开始播放动画
        /// </summary>
        protected abstract void StartAnimationDriver();

        /// <summary>
        /// 取消动画，立刻回到动画的起始状态
        /// </summary>
        protected abstract void ReSetAnimationDriver();

        /// <summary>
        /// 立刻完成动画，会来到动画的结束状态
        /// </summary>
        protected abstract void CompletedImmediatelyAnimationDriver(bool withCompletedCallback);

        /// <summary>
        /// 设置动画播放速度
        /// </summary>
        /// <param name="speed"></param>
        protected abstract void SetAnimationDriverSpeed(float speed);

        /// <summary>
        /// 销毁动画驱动器
        /// </summary>
        public abstract void Kill();
    }

}
