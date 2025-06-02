using LitMotion;
using UnityEngine;
using System;
using R3.Triggers;
using R3;
using System.Threading.Tasks;

namespace ASUI
{
    public class LitMotionAdapter<TValue, TOptions, TAdapter> : AnimationBuilder<TValue, Ease, LoopType, LitMotion.DelayType, AnimationUpdateType>, AnimationProcess<MotionHandle>
        where TValue : unmanaged
        where TOptions : unmanaged, IMotionOptions
        where TAdapter : unmanaged, IMotionAdapter<TValue, TOptions>
    {
        public LitMotionAdapter(MotionBuilder<TValue, TOptions, TAdapter> motionBuilder)
        {
            this.MotionBuilder = motionBuilder;
            this.motionSettings = motionBuilder.ToMotionSettings();
            this.IsOnce = false;
        }

        public LitMotionAdapter(TValue from, TValue to, float duration, Ease ease = Ease.Linear, int loops = 0, LoopType loopType = LoopType.Restart, float delay = 0, LitMotion.DelayType delayType = LitMotion.DelayType.FirstLoop, AnimationUpdateType animationUpdateType = AnimationUpdateType.TimeScaleUpdate)
        {
            this.MotionBuilder = LMotion.Create<TValue, TOptions, TAdapter>(from, to, duration)
                .WithEase(ease)
                .WithLoops(loops, loopType)
                .WithDelay(delay, delayType)
                .WithUpdateType(animationUpdateType);
            this.motionSettings = MotionBuilder.ToMotionSettings();
            this.IsOnce = false;
        }

        /// <summary>
        /// LitMotion 动画构建器实例
        /// </summary>
        public MotionBuilder<TValue, TOptions, TAdapter> MotionBuilder { get; private set; }
        private MotionHandle motionHandle;
        /// <summary>
        /// 动画驱动器实例
        /// </summary>
        public MotionHandle AnimationDriver { get => motionHandle; }

        /// <summary>
        /// 一次性动画还是持续动画或需要多次播放的动画
        /// 一次性动画会在播放完成后自动销毁
        /// 持续动画或需要多次播放的动画需要手动销毁
        /// </summary>
        public bool IsOnce { get; set; }
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
        /// <summary>
        /// 当前动画状态
        /// </summary>
        public AnimationState CurrentState
        {
            get
            {
                if (isPaused)
                    return AnimationState.Pause;
                if (motionHandle.IsPlaying())
                    return AnimationState.Playing;
                else
                {
                    if (motionHandle.Time == 1)
                        return AnimationState.Completed;
                    if (motionHandle.PlaybackSpeed < 0 && motionHandle.Time == 0)
                    return AnimationState.Completed;
                }
                return AnimationState.Idle;
            }
        }
#region 动画构建器相关
        private AnimationUpdateType animationUpdateType;

        public override TValue From { get => MotionSettings.StartValue; }
        public override TValue To { get => MotionSettings.EndValue; }
        public override float Duration { get => motionHandle == MotionHandle.None ? MotionSettings.Duration : motionHandle.Duration; }
        public override Ease Ease { get => MotionSettings.Ease; set => SetMotionSettingsDirty().MotionBuilder.WithEase(value); }
        public override AnimationCurve AnimationCurve { get => MotionSettings.CustomEaseCurve; set => SetMotionSettingsDirty().MotionBuilder.WithEase(value); }
        public override int Loops { get => motionHandle == MotionHandle.None ? MotionSettings.Loops : motionHandle.Loops; set => SetMotionSettingsDirty().MotionBuilder.WithLoops(value, LoopType); }
        public override LoopType LoopType { get => MotionSettings.LoopType; set => SetMotionSettingsDirty().MotionBuilder.WithLoops(Loops, value); }
        public LitMotionAdapter<TValue, TOptions, TAdapter> WithLoop(int loops, LoopType loopType)
        {
            SetMotionSettingsDirty()
            .MotionBuilder.WithLoops(loops, loopType);
            return this;
        }
        public override float Delay { get => motionHandle == MotionHandle.None ? MotionSettings.Delay : motionHandle.Delay; set => SetMotionSettingsDirty().MotionBuilder.WithDelay(value, DelayType); }
        public override LitMotion.DelayType DelayType { get => MotionSettings.DelayType; set => SetMotionSettingsDirty().MotionBuilder.WithDelay(Delay, value); }

        public LitMotionAdapter<TValue, TOptions, TAdapter> WithDelay(float delay, LitMotion.DelayType delayType)
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
        public LitMotionAdapter<TValue, TOptions, TAdapter> SetMotionSettingsDirty()
        {
            needRefreshMotionSettings = true;
            return this;
        }
        #endregion

        #region 动画过程控制相关
        public async Task PlayForward()
        {
            if (motionHandle != null)
            {
                if (this.IsOnce)
                    motionHandle.Cancel();
                else
                    motionHandle.Time = 0;
            }
            else
            {
                //todo: 这里需要根据不同的动画类型来创建不同的 MotionHandle
                motionHandle = MotionBuilder.RunWithoutBinding();
                if (!this.IsOnce)
                    await motionHandle.Preserve();
            }
            await motionHandle;
        }

        private float originalSpeed = 1f;
        private bool isPaused = false;
        public void Pause()
        {
            if (motionHandle == MotionHandle.None || isPaused)
                return;
            isPaused = true;
            originalSpeed = motionHandle.PlaybackSpeed;
            motionHandle.PlaybackSpeed = 0f;
        }

        public void Resume()
        {
            if (motionHandle == MotionHandle.None || !isPaused)
                return;
            isPaused = false;
            motionHandle.PlaybackSpeed = originalSpeed;
        }

        public void ReSet()
        {
            if (motionHandle == MotionHandle.None)
                return;
            motionHandle.Time = 0;
            motionHandle.Cancel();
        }

        public void CompletedImmediately(bool withCompletedCallback)
        {
            if (motionHandle == MotionHandle.None)
                return;
            motionHandle.Complete();
        }

        public void SetAnimationSpeed(float speed)
        {
            if (motionHandle == MotionHandle.None)
                return;
            originalSpeed = speed;
            if (!isPaused)
                motionHandle.PlaybackSpeed = speed;
        }
#endregion
        public override void Dispose()
        {
            targetComponent = null;
            motionSettings = null;
            motionHandle.Cancel();
            var temp = MotionBuilder;
            temp.Dispose();
        }
    }
}