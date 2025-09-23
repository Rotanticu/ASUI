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

    public interface AnimationProcess<T>
    {
        // /// <summary>
        // /// 当前状态
        // /// </summary>
        // public AnimationState CurrentState { get; }

        // // /// <summary>
        // /// 一次性动画还是持续动画或需要多次播放的动画
        // /// 一次性动画会在播放完成后自动销毁
        // /// 持续动画或需要多次播放的动画需要手动销毁
        // /// </summary>
        // public bool IsOnce { get; }
        // /// <summary>
        // /// 获取动画驱动实例
        // /// </summary>
        // public T AnimationDriver { get; }
        // /// <summary>
        // /// 开始播放动画
        // /// </summary>
        // public Task PlayForward();

        // /// <summary>
        // /// 暂停动画
        // /// </summary>
        // public void Pause();

        // /// <summary>
        // /// 恢复动画
        // /// </summary>
        // public void Resume();

        // /// <summary>
        // /// 重置动画到初始状态
        // /// </summary>
        // public void ReSet();
        // /// <summary>
        // /// 立刻完成动画，会来到动画的结束状态
        // /// withCompletedCallback: 是否调用CompletedCallback
        // /// </summary>
        // public void CompletedImmediately(bool withCompletedCallback);

        // /// <summary>
        // /// 倒放动画，不能被打断的动画可以倒放
        // ///damp动画不能倒放，不修改初始结束值的Tween可以倒放，Unity的Anim可以倒放，后两者不能被打断
        // /// </summary>
        // //public virtual async Task PlayBackwards()
        // //{
        // //    if (CurrentState == AnimationState.uninitialized)
        // //        return;
        // //    CompletedImmediately(false);
        // //    SetAnimationSpeed(-AnimationSpeed);
        // //    await PlayForward();
        // //}

        // /// <summary>
        // /// 设置动画速度
        // /// </summary>
        // public void SetAnimationSpeed(float speed);
    }
}
