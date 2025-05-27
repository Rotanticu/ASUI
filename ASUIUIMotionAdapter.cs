using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace ASUI
{
    public class ASUIUIMotionAdapter<TValue> : ASUIAnimationProcess<MotionHandle>
    {
        private Component targetComponent;
        private TValue startTValue;
        private TValue endTValue;
        private float motionDuration;
        private Ease motionEaseType = Ease.Linear;
        private Action onMotionComplete;
        private Action<TValue> onMotionUpdate;
        private MotionType motionPropertyType;

        public enum MotionType
        {
            Position,
            LocalPosition,
            Rotation,
            LocalRotation,
            Scale,
            Color,
            Alpha,
            SizeDelta,
            Custom
        }

        // Constructor for component-based animations (Transform, UI)
        public ASUIUIMotionAdapter(Component target, TValue start, TValue end, float duration, MotionType motionType, Action onComplete = null)
        {
            this.targetComponent = target;
            this.startTValue = start;
            this.endTValue = end;
            this.motionDuration = duration;
            this.motionPropertyType = motionType;
            this.onMotionComplete = onComplete;
            if (motionType == MotionType.Alpha && !(startTValue is float))
            {
                Debug.LogError("ASUIUIMotionAdapter: For Alpha animations, TValue must be float.");
            }
        }

        // Constructor for custom value animations (not tied to a component)
        public ASUIUIMotionAdapter(TValue start, TValue end, float duration, Action<TValue> onUpdate, Action onComplete = null)
        {
            this.startTValue = start;
            this.endTValue = end;
            this.motionDuration = duration;
            this.motionPropertyType = MotionType.Custom;
            this.onMotionUpdate = onUpdate;
            this.onMotionComplete = onComplete;
        }

        protected override void InitializeAnimationDriver(bool isOnce, AnimationUpdateType animationUpdateType, float currentAnimationSpeed, Action completedCallback)
        {
            var scheduler = animationUpdateType switch
            {
                AnimationUpdateType.TimeScaleUpdate => MotionScheduler.Update,
                AnimationUpdateType.UnscaledTimeUpdate => MotionScheduler.Update, // Defaulting to Update. LitMotion builder has WithUpdateMode for specific unscaled needs.
                AnimationUpdateType.FixedUpdate => MotionScheduler.FixedUpdate,
                _ => MotionScheduler.Update
            };

            MotionHandle motionHandle = default;

            Action internalOnComplete = () => { completedCallback?.Invoke(); onMotionComplete?.Invoke(); };

            // Build the common part of the motion
            // We build and bind individually because LMotion.Create<TVal1, TVal2> is specific about TVal1 and TVal2 types.
            // Casting (e.g. (Vector3)(object)startTValue) is needed if TValue isn't the exact type LMotion.Create expects.

            switch (motionPropertyType)
            {
                case MotionType.Position:
                    {
                        if (targetComponent is Transform t && startTValue is Vector3 s && endTValue is Vector3 e)
                            motionHandle = LMotion.Create(s, e, motionDuration).WithEase(motionEaseType).WithScheduler(scheduler).WithOnComplete(internalOnComplete).BindToPosition(t);
                    }
                    break;
                case MotionType.LocalPosition:
                    {
                        if (targetComponent is Transform t && startTValue is Vector3 s && endTValue is Vector3 e)
                            motionHandle = LMotion.Create(s, e, motionDuration).WithEase(motionEaseType).WithScheduler(scheduler).WithOnComplete(internalOnComplete).BindToLocalPosition(t);
                    }
                    break;
                case MotionType.Rotation:
                    {
                        if (targetComponent is Transform t && startTValue is Vector3 s && endTValue is Vector3 e)
                            motionHandle = LMotion.Create(Quaternion.Euler(s), Quaternion.Euler(e), motionDuration).WithEase(motionEaseType).WithScheduler(scheduler).WithOnComplete(internalOnComplete).BindToRotation(t);
                    }
                    break;
                case MotionType.LocalRotation:
                    {
                        if (targetComponent is Transform t && startTValue is Vector3 s && endTValue is Vector3 e)
                            motionHandle = LMotion.Create(Quaternion.Euler(s), Quaternion.Euler(e), motionDuration).WithEase(motionEaseType).WithScheduler(scheduler).WithOnComplete(internalOnComplete).BindToLocalRotation(t);
                    }
                    break;
                case MotionType.Scale:
                    {
                        if (targetComponent is Transform t && startTValue is Vector3 s && endTValue is Vector3 e)
                            motionHandle = LMotion.Create(s, e, motionDuration).WithEase(motionEaseType).WithScheduler(scheduler).WithOnComplete(internalOnComplete).BindToScale(t);
                    }
                    break;
                case MotionType.Color:
                    {
                        if (targetComponent is Image img && startTValue is Color sc && endTValue is Color ec)
                            motionHandle = LMotion.Create(sc, ec, motionDuration).WithEase(motionEaseType).WithScheduler(scheduler).WithOnComplete(internalOnComplete).BindToColor(img);
                        else if (targetComponent is TextMeshProUGUI tmp && startTValue is Color sct && endTValue is Color ect)
                            motionHandle = LMotion.Create(sct, ect, motionDuration).WithEase(motionEaseType).WithScheduler(scheduler).WithOnComplete(internalOnComplete).BindToColor(tmp);
                    }
                    break;
                case MotionType.Alpha:
                    {
                        if (startTValue is float sa && endTValue is float ea)
                        {
                            if (targetComponent is CanvasGroup cg)
                                motionHandle = LMotion.Create(sa, ea, motionDuration).WithEase(motionEaseType).WithScheduler(scheduler).WithOnComplete(internalOnComplete).BindToAlpha(cg);
                            else if (targetComponent is Image img)
                                motionHandle = LMotion.Create(sa, ea, motionDuration).WithEase(motionEaseType).WithScheduler(scheduler).WithOnComplete(internalOnComplete).BindToAlpha(img);
                            else if (targetComponent is TextMeshProUGUI tmp)
                                motionHandle = LMotion.Create(sa, ea, motionDuration).WithEase(motionEaseType).WithScheduler(scheduler).WithOnComplete(internalOnComplete).BindToAlpha(tmp);
                        }
                        else
                        {
                            Debug.LogError("ASUIUIMotionAdapter: Alpha animation requires TValue to be float.");
                        }
                    }
                    break;
                case MotionType.SizeDelta:
                    {
                        if (targetComponent is RectTransform rt && startTValue is Vector2 s && endTValue is Vector2 e)
                            motionHandle = LMotion.Create(s, e, motionDuration).WithEase(motionEaseType).WithScheduler(scheduler).WithOnComplete(internalOnComplete).BindToSizeDelta(rt);
                    }
                    break;
                case MotionType.Custom:
                    {
                        motionHandle = LMotion.Create(startTValue, endTValue, motionDuration)
                            .WithEase(motionEaseType).WithScheduler(scheduler).WithOnComplete(internalOnComplete);
                        if (onMotionUpdate != null) motionHandle = motionHandle.WithOnUpdate(onMotionUpdate);
                        motionHandle = motionHandle.RunWithoutBinding();
                    }
                    break;
                default:
                    Debug.LogError($"ASUIUIMotionAdapter: Unsupported motion type: {motionPropertyType}");
                    break;
            }

            if (motionHandle.IsActive() && targetComponent != null) motionHandle.AddTo(targetComponent.gameObject); // Auto-add to target's lifecycle if bound to component

            AnimationDriver = motionHandle;
            SetAnimationDriverSpeed(currentAnimationSpeed); // Apply initial speed defined by base class or SetAnimationSpeed
            if (CurrentState == AnimationState.Playing || (IsOnce && CurrentState == AnimationState.Idle)) // Auto-play if it was playing OR if it's a once-off animation starting from idle
            {
                StartAnimationDriver(); // This will set the playback speed, effectively starting/resuming
            }
            else if (CurrentState == AnimationState.Pause)
            {
                AnimationDriver.SetPlaybackSpeed(0); // Ensure it respects paused state
            }
        }

        protected override void StartAnimationDriver() // Called by ASUIAnimationProcess or when resuming
        {
            if (AnimationDriver.IsActive()) AnimationDriver.SetPlaybackSpeed(this.animationSpeed);
        }

        public override void Pause()
        {
            if (CurrentState == AnimationState.Playing && AnimationDriver.IsActive())
            {
                originalSpeed = this.animationSpeed; // Store current speed from base class field
                AnimationDriver.SetPlaybackSpeed(0); // LitMotion pauses by setting speed to 0
                CurrentState = AnimationState.Pause;
            }
        }

        public override void Resume()
        {
            if (CurrentState == AnimationState.Pause && AnimationDriver.IsActive())
            {
                AnimationDriver.SetPlaybackSpeed(originalSpeed);
                CurrentState = AnimationState.Playing;
            }
        }

        protected override void ReSetAnimationDriver()
        {
            bool wasPlaying = CurrentState == AnimationState.Playing;
            if (AnimationDriver.IsActive()) AnimationDriver.Cancel();
            InitializeAnimationDriver(IsOnce, AnimationUpdateType, this.animationSpeed, AnimationDriverCompletedCallback);
            if (wasPlaying) StartAnimationDriver();
        }

        protected override void CompletedImmediatelyAnimationDriver(bool withCompletedCallback)
        {
            if (AnimationDriver.IsActive())
            {
                if (withCompletedCallback) AnimationDriver.Complete();
                else AnimationDriver.Cancel();
            }
        }

        protected override void SetAnimationDriverSpeed(float speed)
        {   // This method is called by the base class and by this class's Pause/Resume
            // Ensure we store the speed in the base class field as it's used by StartAnimationDriver on Resume
            this.animationSpeed = speed;
            if (AnimationDriver.IsActive())
            {
                AnimationDriver.SetPlaybackSpeed(speed);
            }
        }

        public override void Kill()
        {
            if (AnimationDriver.IsActive()) AnimationDriver.Cancel();
        }

        public ASUIUIMotionAdapter<TValue> WithEase(Ease ease)
        {
            this.motionEaseType = ease;
            if (CurrentState != AnimationState.uninitialized && AnimationDriver.IsActive())
            {
                bool wasPlaying = CurrentState == AnimationState.Playing;
                AnimationDriver.Cancel();
                InitializeAnimationDriver(IsOnce, AnimationUpdateType, this.animationSpeed, AnimationDriverCompletedCallback);
                if (wasPlaying) StartAnimationDriver();
            }
            return this;
        }
    }
}