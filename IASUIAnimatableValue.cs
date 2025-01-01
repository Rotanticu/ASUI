using LitDamper;
using System;
using UnityEngine;

namespace ASUI
{
    public interface IASUIAnimatableValue<T>: ASUIAnimatable where T : struct
    {
        public T StartValue { get; set; } 
        public T CurrentValue { get; set; }
        public T EndValue { get; set; }
        public float AnimationSpeed { get; set; }
        public Ease Ease { get; set; }
        public float InterpolationTime { get; set; }



        //下面的放实现里
        public bool IsPause { get; set; }

        public new bool IsAnimating
        {
            get
            {
                return !IsPause && !CurrentValue.Equals(EndValue);
            }
        }
        public void PlayAnimation(T startValue,T endValue,float AnimationSpeed, Ease ease = Ease.Linear);

        public void PlaybackAnimation();

        public void PlaybackAnimation(float AnimationSpeed);

        public void SetAnimationInterpolationDirectly(float t, Ease ease);


        public void GetInterpolation(T startValue, T endValue, float currentTime, float duration, Ease ease);
    }

    public struct testASUIAnimatableValue : IASUIAnimatableValue<float>
    {
        public float StartValue { get; set; }
        public float CurrentValue { get; set; }
        public float EndValue { get; set; }
        public float AnimationSpeed { get; set; }
        public Ease Ease { get; set; }
        public float InterpolationTime { get; set; }
        public bool IsPause { get; set; }

        public bool IsAnimating => !IsPause && AnimationSpeed !=0 && AnimationSpeed > 0 ? !CurrentValue.Equals(EndValue) : !CurrentValue.Equals(StartValue);

        public void AnimationCompleteImmediately()
        {
            CurrentValue = EndValue;
        }

        public void GetInterpolation(float startValue, float endValue, float currentTime, float duration, Ease ease)
        {
            throw new NotImplementedException();
        }

        public void PlayAnimation(float startValue, float endValue, float AnimationSpeed, Ease ease = Ease.Linear)
        {
            throw new NotImplementedException();
        }

        public void PlaybackAnimation()
        {
            throw new NotImplementedException();
        }

        public void PlaybackAnimation(float AnimationSpeed)
        {
            throw new NotImplementedException();
        }

        public void SetAnimationInterpolationDirectly(float t, Ease ease)
        {
            throw new NotImplementedException();
        }
    }



}
