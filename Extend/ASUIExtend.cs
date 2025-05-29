using UnityEngine;
using System.Collections.Generic;
using Codice.CM.Common;

namespace ASUI
{
    public static class ASUIExtend
    {
        public static T FindComponentInChildren<T>(this Component component, string childName)
        {
            Transform child = component.transform.Find(childName);
            if (child != null)
                return child.GetComponent<T>();
            return default;
        }

        public static void Set<K, V>(this Dictionary<K, V> Dictionary, K key, V Value)
        {
            if (Dictionary.ContainsKey(key))
            {
                Dictionary[key] = Value;
            }
            else
            {
                Dictionary.Add(key, Value);
            }
        }
        
        public static LitMotion.MotionBuilder<TValue,TOptions,TAdapter> WithUpdateType<TValue,TOptions,TAdapter>(this LitMotion.MotionBuilder<TValue,TOptions,TAdapter> motionBuilder, AnimationUpdateType animationUpdateType)
        where TValue : unmanaged
        where TOptions : unmanaged, LitMotion.IMotionOptions
        where TAdapter : unmanaged, LitMotion.IMotionAdapter<TValue, TOptions>
        {
            switch (animationUpdateType)
            {
                case AnimationUpdateType.TimeScaleUpdate:
                motionBuilder.WithScheduler(LitMotion.MotionScheduler.TimeUpdate);
                break;
                case AnimationUpdateType.UnscaledTimeUpdate:
                motionBuilder.WithScheduler(LitMotion.MotionScheduler.TimeUpdateIgnoreTimeScale);
                break;
                case AnimationUpdateType.FixedUpdate:
                motionBuilder.WithScheduler(LitMotion.MotionScheduler.FixedUpdate);
                break;
            }
            return motionBuilder;
        }
    }
}

