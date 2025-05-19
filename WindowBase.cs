using R3;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace ASUI
{
    public abstract class ASUIWindowBase : WidgetsBase, IASUIStateSwitch
    {
        public override bool IsVisible => this.WidgetState != WidgetState.Hide;

        public bool IsAnimating
        {
            get => this.WidgetState is WidgetState.Entering or WidgetState.Exiting;
        }
        public string CurrentState { get => StyleState.State; set => StyleState.State = value; }

        public override void Init(GameObject gameObject)
        {
            base.Init(gameObject);
        }

        public override async Task Show()
        {
            this.WidgetState = WidgetState.Entering;
            this.PlayShowAnimation();
            await base.Show();
        }
        public abstract void PlayShowAnimation();

        public virtual void ShowAnimationCompleted()
        {
            this.WidgetState = WidgetState.Show;
            ShowAnimationCompletedEvent?.Invoke();
        }
        public override async Task Hide()
        {
            this.WidgetState = WidgetState.Exiting;
            this.PlayHideAnimation();
            await base.Hide();
        }
        public abstract void PlayHideAnimation();

        public virtual void HideAnimationCompleted()
        {
            this.WidgetState = WidgetState.Hide;
            HideAnimationCompletedEvent?.Invoke();
        }

        public override void DestroyImmediately()
        {
            this.WidgetState = WidgetState.Destroyed;
            base.DestroyImmediately();
        }

        public void AnimationCompleteImmediately()
        {
            
        }
    }
}