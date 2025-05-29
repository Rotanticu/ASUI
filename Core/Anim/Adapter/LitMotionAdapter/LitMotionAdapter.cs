using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace ASUI
{
    public class LitMotionAdapter<TValue, TOptions, TAdapter>
        where TValue : unmanaged
        where TOptions : unmanaged, IMotionOptions
        where TAdapter : unmanaged, IMotionAdapter<TValue, TOptions>
    {
        public LitMotionAnimationBuilder<TValue, TOptions, TAdapter> litMotionAnimationBuilder;
        public LitMotionAnimationProcess litMotionAnimationProcess;
        
        public LitMotionAdapter(MotionBuilder<TValue, TOptions, TAdapter> motionBuilder)
        {
            this.litMotionAnimationBuilder = new LitMotionAnimationBuilder<TValue, TOptions, TAdapter>(motionBuilder);
        }

        public LitMotionAdapter(TValue from, TValue to, float duration, Ease ease = Ease.Linear, int loops = 0, LoopType loopType = LoopType.Restart, float delay = 0, DelayType delayType = DelayType.FirstLoop, AnimationUpdateType animationUpdateType = AnimationUpdateType.TimeScaleUpdate)
        {
            this.litMotionAnimationBuilder = new LitMotionAnimationBuilder<TValue, TOptions, TAdapter>(from, to, duration, ease, loops, loopType, delay, delayType, animationUpdateType);
        }
    }
}