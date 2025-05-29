using UnityEngine;
using System;

namespace ASUI
{
    public abstract class AnimationBuilder<TValue,TEase,TLoopType,TDelayType,TUpdateType> : IDisposable
    where TEase : Enum
    where TLoopType : Enum
    where TDelayType : Enum
    {
        public abstract Component TargetComponent { get; set; }
        public abstract TValue From { get; }
        public abstract TValue To { get;}
        public abstract float Duration { get;}
        public abstract TEase Ease { get; set; }
        public abstract AnimationCurve AnimationCurve { get; set; }

        public abstract int Loops { get; set; }
        public abstract TLoopType LoopType { get; set; }

        public abstract float Delay { get; set; }
        public abstract TDelayType DelayType { get; set; }

        public abstract TUpdateType AnimationUpdateType { get; set; }

        public virtual TBuilder WithTargetComponent<TBuilder>(Component targetComponent) where TBuilder : AnimationBuilder<TValue, TEase, TLoopType, TDelayType, TUpdateType>
        {
            TargetComponent = targetComponent;
            return this as TBuilder;
        }
        public virtual TBuilder WithEase<TBuilder>(TEase ease) where TBuilder : AnimationBuilder<TValue, TEase, TLoopType, TDelayType, TUpdateType>
        {
            Ease = ease;
            return this as TBuilder;
        }
        public virtual TBuilder WithDelay<TBuilder>(float delay) where TBuilder : AnimationBuilder<TValue, TEase, TLoopType, TDelayType, TUpdateType>
        {
            Delay = delay;
            return this as TBuilder;
        }
        public virtual TBuilder WithDelayType<TBuilder>(TDelayType delayType) where TBuilder : AnimationBuilder<TValue, TEase, TLoopType, TDelayType, TUpdateType>
        {
            DelayType = delayType;
            return this as TBuilder;
        }
        public virtual TBuilder WithLoops<TBuilder>(int loops) where TBuilder : AnimationBuilder<TValue, TEase, TLoopType, TDelayType, TUpdateType>
        {
            Loops = loops;
            return this as TBuilder;
        }
        public virtual TBuilder WithLoopType<TBuilder>(TLoopType loopType) where TBuilder : AnimationBuilder<TValue, TEase, TLoopType, TDelayType, TUpdateType>
        {
            LoopType = loopType;
            return this as TBuilder;
        }
        public virtual TBuilder WithUpdateType<TBuilder>(TUpdateType updateType) where TBuilder : AnimationBuilder<TValue, TEase, TLoopType, TDelayType, TUpdateType>
        {
            AnimationUpdateType = updateType;
            return this as TBuilder;
        }

        public abstract TBuilder AddOnLoopCompleteAction<TBuilder>(Action<int> action) where TBuilder : AnimationBuilder<TValue, TEase, TLoopType, TDelayType, TUpdateType>;
        public abstract TBuilder AddOnCompleteAction<TBuilder>(Action action) where TBuilder : AnimationBuilder<TValue, TEase, TLoopType, TDelayType, TUpdateType>;
        public abstract TBuilder AddOnCancelAction<TBuilder>(Action action) where TBuilder : AnimationBuilder<TValue, TEase, TLoopType, TDelayType, TUpdateType>;

        // /// <summary>
        // /// 动画速度
        // /// </summary>
        // public float AnimationSpeed { get; set; }

        // /// <summary>
        // /// 初始化动画驱动器
        // /// </summary>
        // protected abstract void InitializeAnimationDriver(bool isOnce, TUpdateType animationUpdateType, float animationSpeed);

        // /// <summary>
        // /// 开始播放动画
        // /// </summary>
        // protected abstract void StartAnimationDriver();

        // /// <summary>
        // /// 取消动画，立刻回到动画的起始状态
        // /// </summary>
        // protected abstract void ReSetAnimationDriver();

        // /// <summary>
        // /// 立刻完成动画，会来到动画的结束状态
        // /// </summary>
        // protected abstract void CompletedImmediatelyAnimationDriver(bool withCompletedCallback);

        // /// <summary>
        // /// 设置动画播放速度
        // /// </summary>
        // /// <param name="speed"></param>
        // protected abstract void SetAnimationDriverSpeed(float speed);

        /// <summary>
        /// 销毁动画驱动器
        /// </summary>
        public abstract void Dispose();
    }
}
