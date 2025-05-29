using UnityEngine;
using LitMotion;
using System;
using R3;
using R3.Triggers;

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
    public class LitMotionAnimationBuilder<TValue, TOptions, TAdapter> : AnimationBuilder<TValue, Ease, LoopType, DelayType, AnimationUpdateType>
        where TValue : unmanaged
        where TOptions : unmanaged, IMotionOptions
        where TAdapter : unmanaged, IMotionAdapter<TValue, TOptions>
    {
        public MotionBuilder<TValue, TOptions, TAdapter> MotionBuilder { get; private set; }

        private bool needRefreshMotionSettings = false;
        private MotionSettings<TValue, TOptions> motionSettings;

        public MotionSettings<TValue, TOptions> MotionSettings
        {
            get
            {
                if (motionSettings == null || needRefreshMotionSettings)
                {
                    motionSettings = MotionBuilder.ToMotionSettings();
                    needRefreshMotionSettings = false;
                }
                return motionSettings;
            }
        }

        public LitMotionAnimationBuilder<TValue, TOptions, TAdapter> SetMotionSettingsDirty()
        {
            needRefreshMotionSettings = true;
            return this;
        }

        private AnimationUpdateType animationUpdateType;

        public LitMotionAnimationBuilder(TValue from, TValue to, float duration, Ease ease = Ease.Linear, int loops = 0, LoopType loopType = LoopType.Restart, float delay = 0, DelayType delayType = DelayType.FirstLoop, AnimationUpdateType animationUpdateType = AnimationUpdateType.TimeScaleUpdate)
        {
            MotionBuilder = LMotion.Create<TValue, TOptions, TAdapter>(from, to, duration).WithEase(ease).WithLoops(loops, loopType).WithDelay(delay, delayType).WithUpdateType(animationUpdateType);
            motionSettings = MotionBuilder.ToMotionSettings();
        }

        public LitMotionAnimationBuilder(MotionBuilder<TValue, TOptions, TAdapter> motionBuilder)
        {
            MotionBuilder = motionBuilder;
            motionSettings = motionBuilder.ToMotionSettings();
        }

        public override TValue From { get => MotionSettings.StartValue; }
        public override TValue To { get => MotionSettings.EndValue; }
        public override float Duration { get => MotionSettings.Duration; }
        public override Ease Ease { get => MotionSettings.Ease; set => SetMotionSettingsDirty().MotionBuilder.WithEase(value); }
        public override AnimationCurve AnimationCurve { get => MotionSettings.CustomEaseCurve; set => SetMotionSettingsDirty().MotionBuilder.WithEase(value); }
        public override int Loops { get => MotionSettings.Loops; set => SetMotionSettingsDirty().MotionBuilder.WithLoops(value, LoopType); }
        public override LoopType LoopType { get => MotionSettings.LoopType; set => SetMotionSettingsDirty().MotionBuilder.WithLoops(Loops, value); }
        public LitMotionAnimationBuilder<TValue, TOptions, TAdapter> WithLoop(int loops, LoopType loopType)
        {
            SetMotionSettingsDirty()
            .MotionBuilder.WithLoops(loops, loopType);
            return this;
        }
        public override float Delay { get => MotionSettings.Delay; set => SetMotionSettingsDirty().MotionBuilder.WithDelay(value, DelayType); }
        public override DelayType DelayType { get => MotionSettings.DelayType; set => SetMotionSettingsDirty().MotionBuilder.WithDelay(Delay,value); }

        public LitMotionAnimationBuilder<TValue, TOptions, TAdapter> WithDelay(float delay, DelayType delayType)
        {
            SetMotionSettingsDirty()
            .MotionBuilder.WithDelay(delay, delayType);
            return this;
        }
        public override AnimationUpdateType AnimationUpdateType { get => animationUpdateType; set => SetMotionSettingsDirty().MotionBuilder.WithUpdateType(value); }

        private Component targetComponent;
        public override Component TargetComponent
        {
            get
            {
                return targetComponent;
            }
            set
            {
                MotionBuilder.AddTo(value);
                //如果添加成功了，应该有这个组件
                if (value.gameObject.GetComponent<ObservableDestroyTrigger>() != null)
                    targetComponent = value;
            }
        }

        public override TBuilder AddOnLoopCompleteAction<TBuilder>(Action<int> action)
        {
            MotionBuilder.WithOnLoopComplete(action);
            return this as TBuilder;
        }
        public override TBuilder AddOnCompleteAction<TBuilder>(Action action)
        {
            MotionBuilder.WithOnComplete(action);
            return this as TBuilder;
        }
        public override TBuilder AddOnCancelAction<TBuilder>(Action action)
        {
            MotionBuilder.WithOnCancel(action);
            return this as TBuilder;
        }

        public override void Dispose()
        {
            targetComponent = null;
            motionSettings = null;
            var temp = MotionBuilder;
            temp.Dispose();
        }

    }
}
