using R3;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ASUI
{
    public sealed class ASTextMeshProUGUI : TextMeshProUGUI
    {
        public ReactiveProperty<string> RText = new ReactiveProperty<string>(string.Empty);

        public ReactiveProperty<Color> RColor = new ReactiveProperty<Color>(Color.white);

        override protected void Awake()
        {
            base.Awake();

            RText.Subscribe(newText =>
            {
                if (!Application.isPlaying) return;
                text = newText;
            }).AddTo(this);

            RColor.Subscribe(newColor =>
            {
                if (!Application.isPlaying) return;
                this.color = newColor;
            }).AddTo(this); ;
        }
        
        public void AnimateColorTo(Color target,float duration)
        {
            float startTime = Time.time;
            var from = color;
            Observable
            .TimerFrame(0, 1)
            .TakeWhile(_ => Time.time - startTime < duration)
            .Subscribe(_ =>
            {
                float t = (Time.time - startTime) / duration;
                RColor.Value = Color.Lerp(from, target, t);
            });
        }
    }
}
