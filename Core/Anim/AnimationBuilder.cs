using UnityEngine;
using System;

namespace ASUI
{
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
    //public abstract class AnimationBuilder<TValue, TEase, TLoopType, TDelayType, TUpdateType> : IDisposable
    public abstract class AnimationBuilder<TValue, TEase, TLoopType, TDelayType, TUpdateType>
    where TEase : Enum
    where TLoopType : Enum
    where TDelayType : Enum
    {
        // public abstract Component TargetComponent { get; set; }
        // public abstract TValue From { get; }
        // public abstract TValue To { get; }
        // //public abstract float Duration { get; }
        // public abstract TEase Ease { get; set; }
        // public abstract AnimationCurve AnimationCurve { get; set; }

        // //public abstract int Loops { get; set; }
        // //public abstract TLoopType LoopType { get; set; }

        // //public abstract float Delay { get; set; }
        // //public abstract TDelayType DelayType { get; set; }

        // public abstract TUpdateType AnimationUpdateType { get; set; }

        // /// <summary>
        // /// 设置目标组件
        // /// </summary>
        // /// <typeparam name="TBuilder"></typeparam>
        // /// <param name="targetComponent"></param>
        // /// <returns></returns>
        // public virtual TBuilder WithTargetComponent<TBuilder>(Component targetComponent) where TBuilder : AnimationBuilder<TValue, TEase, TLoopType, TDelayType, TUpdateType>
        // {
        //     TargetComponent = targetComponent;
        //     return this as TBuilder;
        // }
        // /// <summary>
        // /// 设置动画曲线
        // /// </summary>
        // /// <typeparam name="TBuilder"></typeparam>
        // /// <param name="ease"></param>
        // /// <returns></returns>
        // public virtual TBuilder WithEase<TBuilder>(TEase ease) where TBuilder : AnimationBuilder<TValue, TEase, TLoopType, TDelayType, TUpdateType>
        // {
        //     Ease = ease;
        //     return this as TBuilder;
        // }
        // // /// <summary>
        // // /// 设置动画延迟
        // // /// </summary>
        // // /// <typeparam name="TBuilder"></typeparam>
        // // /// <param name="delay"></param>
        // // /// <returns></returns>
        // // public virtual TBuilder WithDelay<TBuilder>(float delay) where TBuilder : AnimationBuilder<TValue, TEase, TLoopType, TDelayType, TUpdateType>
        // // {
        // //     Delay = delay;
        // //     return this as TBuilder;
        // // }
        // /// <summary>
        // /// 设置动画延迟类型
        // /// </summary>
        // /// <typeparam name="TBuilder"></typeparam>
        // /// <param name="delayType"></param>
        // /// <returns></returns>
        // // public virtual TBuilder WithDelayType<TBuilder>(TDelayType delayType) where TBuilder : AnimationBuilder<TValue, TEase, TLoopType, TDelayType, TUpdateType>
        // // {
        // //     DelayType = delayType;
        // //     return this as TBuilder;
        // // }
        // /// <summary>
        // /// 设置动画循环次数
        // /// </summary>
        // /// <typeparam name="TBuilder"></typeparam>
        // /// <param name="loops"></param>
        // /// <returns></returns>
        // // public virtual TBuilder WithLoops<TBuilder>(int loops) where TBuilder : AnimationBuilder<TValue, TEase, TLoopType, TDelayType, TUpdateType>
        // // {
        // //     Loops = loops;
        // //     return this as TBuilder;
        // // }
        // /// <summary>
        // /// 设置动画循环类型
        // /// </summary>
        // /// <typeparam name="TBuilder"></typeparam>
        // /// <param name="loopType"></param>
        // /// <returns></returns>
        // // public virtual TBuilder WithLoopType<TBuilder>(TLoopType loopType) where TBuilder : AnimationBuilder<TValue, TEase, TLoopType, TDelayType, TUpdateType>
        // // {
        // //     LoopType = loopType;
        // //     return this as TBuilder;
        // // }
        // /// <summary>
        // /// 设置动画帧驱动类型
        // /// </summary>
        // /// <typeparam name="TBuilder"></typeparam>
        // /// <param name="updateType"></param>
        // /// <returns></returns>
        // public virtual TBuilder WithUpdateType<TBuilder>(TUpdateType updateType) where TBuilder : AnimationBuilder<TValue, TEase, TLoopType, TDelayType, TUpdateType>
        // {
        //     AnimationUpdateType = updateType;
        //     return this as TBuilder;
        // }

        // /// <summary>
        // /// 添加循环完成回调
        // /// </summary>
        // /// <typeparam name="TBuilder"></typeparam>
        // /// <param name="action"></param>
        // /// <returns></returns>
        // public abstract TBuilder AddOnLoopCompleteAction<TBuilder>(Action<int> action) where TBuilder : AnimationBuilder<TValue, TEase, TLoopType, TDelayType, TUpdateType>;
        // /// <summary>
        // /// 添加动画完成回调
        // /// </summary>
        // /// <typeparam name="TBuilder"></typeparam>
        // /// <param name="action"></param>
        // /// <returns></returns>
        // public abstract TBuilder AddOnCompleteAction<TBuilder>(Action action) where TBuilder : AnimationBuilder<TValue, TEase, TLoopType, TDelayType, TUpdateType>;
        // /// <summary>
        // /// 添加动画取消回调
        // /// </summary>
        // /// <typeparam name="TBuilder"></typeparam>
        // /// <param name="action"></param>
        // /// <returns></returns>
        // public abstract TBuilder AddOnCancelAction<TBuilder>(Action action) where TBuilder : AnimationBuilder<TValue, TEase, TLoopType, TDelayType, TUpdateType>;

        // /// <summary>
        // /// 销毁动画驱动器
        // /// </summary>
        // public abstract void Dispose();
    }
}
