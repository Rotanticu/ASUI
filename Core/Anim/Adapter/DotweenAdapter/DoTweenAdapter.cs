//using DG.Tweening;
using UnityEngine;
using System;
using R3.Triggers;
using System.Threading.Tasks;

namespace ASUI
{
    public enum DelayType : byte
    {
        /// <summary>
        /// Delay when starting playback
        /// </summary>
        FirstLoop = 0,
        /// <summary>
        /// Delay every loop
        /// </summary>
        EveryLoop = 1,
    }
    //public class DoTweenAdapter<TValue> : AnimationBuilder<TValue, Ease, LoopType, DelayType, AnimationUpdateType>, AnimationProcess<Tween>
    //    where TValue : struct
    //  {
    //     public DoTweenAdapter(Tween tween)
    //     {
    //         this.tween = tween;
    //         this.IsOnce = false;
    //     }
    //     public DoTweenAdapter(TValue from, TValue to, float duration, Ease ease = Ease.Linear, int loops = 0, LoopType loopType = LoopType.Restart, float delay = 0, DelayType delayType = DelayType.FirstLoop, AnimationUpdateType animationUpdateType = AnimationUpdateType.TimeScaleUpdate)
    //     {

    //     }

    //     private Tween tween;
    //     /// <summary>
    //     /// 动画驱动器实例
    //     /// </summary>
    //     public Tween AnimationDriver { get => tween; }

    //     /// <summary>
    //     /// 一次性动画还是持续动画或需要多次播放的动画
    //     /// 一次性动画会在播放完成后自动销毁
    //     /// 持续动画或需要多次播放的动画需要手动销毁
    //     /// </summary>
    //     public bool IsOnce { get; set; }

    //     /// <summary>
    //     /// 当前动画状态
    //     /// </summary>
    //     public AnimationState CurrentState
    //     {
    //         get
    //         {
    //             if (tween == null)
    //                 return AnimationState.uninitialized;
    //             if (tween.IsPlaying())
    //                 return AnimationState.Playing;
    //             if (tween.IsComplete())
    //                 return AnimationState.Completed;
    //             if (tween.IsActive() && !tween.IsPlaying() && !tween.IsComplete())
    //                 return AnimationState.Pause;
    //             return AnimationState.Idle;
    //         }
    //     }

    //     #region 动画构建器相关
    //     private Component targetComponent;

    //     public override TValue From { get;}
    //     public override TValue To { get;}
    //     public override float Duration
    //     {
    //         get => tween?.Duration(false) ?? 0f;
    //     }

    //     private Ease easeType = Ease.OutQuad;
    //     public override Ease Ease
    //     {
    //         get => easeType;
    //         set
    //         {
    //             if (tween != null)
    //             {
    //                 easeType = value;
    //                 tween.SetEase(value);
    //             }
    //         }
    //     }
    //     private AnimationCurve easeCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
    //     public override AnimationCurve AnimationCurve
    //     {
    //         get => easeCurve;
    //         set
    //         {
    //             if (tween != null)
    //             {
    //                 easeCurve = value;
    //                 tween.SetEase(value);
    //             }
                    
    //         }
    //     }
    //     private int loops = 1;
    //     public override int Loops
    //     {
    //         get => loops;
    //         set
    //         {
    //             if (tween != null)
    //             {
    //                 loops = value;
    //                 tween.SetLoops(value, LoopType);
    //             }
    //         }
    //     }
    //     private LoopType loopType = LoopType.Restart;
    //     public override LoopType LoopType
    //     {
    //         get => loopType;
    //         set
    //         {
    //             if (tween != null)
    //             {
    //                 loopType = value;
    //                 tween.SetLoops(Loops, value);
    //             }
    //         }
    //     }
    //     private float delay;
    //     public override float Delay
    //     {
    //         get => delay;
    //         set
    //         {
    //             if (tween != null)
    //             {
    //                 delay = value;
    //                 tween.SetDelay(value,delayType == DelayType.EveryLoop ? true : false);
    //             }
    //         }
    //     }
    //     private DelayType delayType = DelayType.FirstLoop;
    //     public override DelayType DelayType
    //     {
    //         get => delayType;
    //         set
    //         {
    //             if (tween != null)
    //             {
    //                 delayType = value;
    //                 tween.SetDelay(Delay,value == DelayType.EveryLoop ? true : false);
    //             }
    //         }
    //     }
    //     private UpdateType updateType = UpdateType.Normal;

    //     private AnimationUpdateType animationUpdateType = AnimationUpdateType.TimeScaleUpdate;
    //     public override AnimationUpdateType AnimationUpdateType
    //     {
    //         get => animationUpdateType;
    //         set
    //         {
    //             if (tween == null)
    //                 return;
    //             switch (value)
    //             {
    //                 case AnimationUpdateType.TimeScaleUpdate:
    //                 {
    //                     updateType = UpdateType.Normal;
    //                     animationUpdateType = value;
    //                     tween.SetUpdate(updateType, false);
    //                 }
    //                 break;
    //                 case AnimationUpdateType.UnscaledTimeUpdate:
    //                 {
    //                     updateType = UpdateType.Normal;
    //                     animationUpdateType = value;
    //                     tween.SetUpdate(updateType, true);
    //                 }
    //                 break;
    //                 case AnimationUpdateType.FixedUpdate:
    //                 {
    //                     updateType = UpdateType.Fixed;
    //                     animationUpdateType = value;
    //                     tween.SetUpdate(updateType, false);
    //                 }
    //                 break;
    //                 default:
    //                 break;
    //             }
    //         }
    //     }

    //     public override Component TargetComponent
    //     {
    //         get => targetComponent;
    //         set
    //         {
    //             targetComponent = value;
    //             if (value != null && value.gameObject.GetComponent<ObservableDestroyTrigger>() == null)
    //             {
    //                 value.gameObject.AddComponent<ObservableDestroyTrigger>();
    //             }
    //         }
    //     }

    //     public override TBuilder AddOnLoopCompleteAction<TBuilder>(Action<int> action)
    //     {
    //         if (tween != null)
    //         {
    //             tween.OnStepComplete(() => action?.Invoke(tween.CompletedLoops()));
    //         }
    //         return this as TBuilder;
    //     }

    //     public override TBuilder AddOnCompleteAction<TBuilder>(Action action)
    //     {
    //         if (tween != null)
    //         {
    //             tween.OnComplete(() => action?.Invoke());
    //         }
    //         return this as TBuilder;
    //     }

    //     public override TBuilder AddOnCancelAction<TBuilder>(Action action)
    //     {
    //         if (tween != null)
    //         {
    //             tween.OnKill(() => action?.Invoke());
    //         }
    //         return this as TBuilder;
    //     }
    //     #endregion

    //     #region 动画过程控制相关
    //     public async Task PlayForward()
    //     {
    //         if (tween != null)
    //         {
    //             if (this.IsOnce)
    //                 tween.Kill();
    //             else
    //                 tween.Restart();
    //         }

    //         // 创建新的 Tween
    //         tween = CreateTween(From, To, Duration);

    //         if (!this.IsOnce)
    //         {
    //             // 保持 Tween 实例
    //             tween.SetAutoKill(false);
    //         }

    //         await tween.AsyncWaitForCompletion();
    //     }

    //     private float originalSpeed = 1f;
    //     private bool isPaused = false;

    //     public void Pause()
    //     {
    //         if (tween == null || isPaused)
    //             return;
    //         isPaused = true;
    //         originalSpeed = tween.timeScale;
    //         tween.Pause();
    //     }

    //     public void Resume()
    //     {
    //         if (tween == null || !isPaused)
    //             return;
    //         isPaused = false;
    //         tween.timeScale = originalSpeed;
    //         tween.Play();
    //     }

    //     public void ReSet()
    //     {
    //         if (tween == null)
    //             return;
    //         tween.Restart();
    //         tween.Pause();
    //     }

    //     public void CompletedImmediately(bool withCompletedCallback)
    //     {
    //         if (tween == null)
    //             return;
    //         tween.Complete(withCompletedCallback);
    //     }

    //     public void SetAnimationSpeed(float speed)
    //     {
    //         if (tween == null)
    //             return;
    //         originalSpeed = speed;
    //         if (!isPaused)
    //             tween.timeScale = speed;
    //     }
    //     #endregion

    //     private Tween CreateTween(TValue from, TValue to, float duration)
    //     {
    //         // 根据不同的 TValue 类型创建对应的 Tween
    //         Tween newTween = null;
            
    //         if (typeof(TValue) == typeof(float))
    //         {
    //             newTween = DOTween.To(() => (float)(object)from, x => { }, (float)(object)to, duration);
    //         }
    //         else if (typeof(TValue) == typeof(Vector2))
    //         {
    //             newTween = DOTween.To(() => (Vector2)(object)from, x => { }, (Vector2)(object)to, duration);
    //         }
    //         else if (typeof(TValue) == typeof(Vector3))
    //         {
    //             newTween = DOTween.To(() => (Vector3)(object)from, x => { }, (Vector3)(object)to, duration);
    //         }
    //         else if (typeof(TValue) == typeof(Color))
    //         {
    //             newTween = DOTween.To(() => (Color)(object)from, x => { }, (Color)(object)to, duration);
    //         }
    //         else if (typeof(TValue) == typeof(Quaternion))
    //         {
    //             DOTween.To(() => (Quaternion)(object)from, x => { }, (Vector3)(object)to, duration);
    //         }

    //         if (newTween != null)
    //         {
    //             newTween.SetEase(Ease)
    //                 .SetLoops(Loops, LoopType)
    //                 .SetDelay(Delay)
    //                 .SetUpdate(AnimationUpdateType == AnimationUpdateType.FixedUpdate ? UpdateType.Fixed : UpdateType.Normal, AnimationUpdateType == AnimationUpdateType.UnscaledTimeUpdate);
    //         }

    //         return newTween;
    //     }

    //     public override void Dispose()
    //     {
    //         if (tween != null)
    //         {
    //             tween.Kill();
    //             tween = null;
    //         }
    //         targetComponent = null;
    //     }
    // }
} 