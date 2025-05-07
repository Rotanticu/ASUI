using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace ASUI
{
    public enum ASUIWindowState
    {
        IsShowAnimating = 1,
        IsShow = 2,
        IsHideAnimating = 3,
        IsHide = 4,
    }
    public abstract class ASUIWindowBase : WidgetsBase, IASUIWindowAnimation, IASUIStateSwitch
    {
        private ASUIWindowState m_WindowState = ASUIWindowState.IsShow;
        public override bool IsVisible => this.WindowState != ASUIWindowState.IsHide;

        public override bool IsShow => this.WindowState is ASUIWindowState.IsShow or ASUIWindowState.IsShowAnimating;
        public ASUIWindowState WindowState
        {
            get
            {
                return m_WindowState;
            }
            set
            {
                m_WindowState = value;
            }
        }

        public bool IsAnimating
        {
            get => this.WindowState is ASUIWindowState.IsShowAnimating or ASUIWindowState.IsHideAnimating;
        }
        public string CurrentState { get => styleState.State; set => styleState.State = value; }

        public override void Init(GameObject gameObject)
        {
            this.styleState = this.Transform.GetComponent<ASUIStyleState>();
            base.Init(gameObject);
        }

        public override async Task Show()
        {
            this.WindowState = ASUIWindowState.IsShowAnimating;
            this.PlayShowAnimation();
            await base.Show();
        }
        public abstract void PlayShowAnimation();

        public virtual void ShowAnimationCompleted()
        {
            this.WindowState = ASUIWindowState.IsShow;
            ShowAnimationCompletedEvent?.Invoke();
        }
        public override async Task Hide()
        {
            this.WindowState = ASUIWindowState.IsHideAnimating;
            this.PlayHideAnimation();
            await base.Hide();
            this.HideAnimationCompleted();
        }
        public abstract void PlayHideAnimation();

        public virtual void HideAnimationCompleted()
        {
            this.WindowState = ASUIWindowState.IsHide;
            HideAnimationCompletedEvent?.Invoke();
            this.GameObject.SetActive(false);
        }

        public override void DestroyImmediately()
        {
            this.m_WindowState = ASUIWindowState.IsHide;
            base.DestroyImmediately();
        }

        public void AnimationCompleteImmediately()
        {

        }
    }

    public interface ASUIAnimatable
    {
        public bool IsAnimating { get;}

        public void AnimationCompleteImmediately();
    }

    public interface IASUIShowAnimatable : ASUIAnimatable
    {
        public void PlayShowAnimation();
        public void ShowAnimationCompleted();

        public ASUIUnityEvent ShowAnimationCompletedEvent { get; set; }
    }

    public interface IASUIHideAnimatable : ASUIAnimatable
    {

        public void PlayHideAnimation();
        public void HideAnimationCompleted();

        public ASUIUnityEvent HideAnimationCompletedEvent { get; set; }
    }


    public interface IASUIWindowAnimation : IASUIShowAnimatable, IASUIHideAnimatable
    {

    }
}