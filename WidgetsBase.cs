using NUnit.Framework;
using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace ASUI
{
    public abstract class WidgetsBase : IASUIInit, IASUIShow, IASUIHide, IASUIDestroy
    {
        private bool m_isInitialized;
        public bool IsInitialized { get { return m_isInitialized; } }

        public abstract bool IsVisible { get; }

        public abstract bool IsShow { get; }

        protected bool m_DestroyWhenHideCompleted;

        [SerializeField]
        private ASUIUnityEvent m_InitEvent;
        public ASUIUnityEvent InitEvent { get { return m_InitEvent; } set { m_InitEvent = value; } }
        [SerializeField]
        private ASUIUnityEvent m_ShowEvent;
        public ASUIUnityEvent ShowEvent { get { return m_ShowEvent; } set { m_ShowEvent = value; } }
        [SerializeField]
        private ASUIUnityEvent m_ShowAnimationCompletedEvent;
        public ASUIUnityEvent ShowAnimationCompletedEvent { get { return m_ShowAnimationCompletedEvent; } set { m_ShowAnimationCompletedEvent = value; } }
        [SerializeField]
        private ASUIUnityEvent m_HideEvent;
        public ASUIUnityEvent HideEvent { get { return m_HideEvent; } set { m_HideEvent = value; } }
        [SerializeField]
        private ASUIUnityEvent m_HideAnimationCompletedEvent;
        public ASUIUnityEvent HideAnimationCompletedEvent { get { return m_HideAnimationCompletedEvent; } set { m_HideAnimationCompletedEvent = value; } }
        [SerializeField]
        private ASUIUnityEvent m_DestroyEvent;
        public ASUIUnityEvent DestroyEvent { get { return m_DestroyEvent; } set { m_DestroyEvent = value; } }

        private GameObject m_GameObject;
        public GameObject GameObject
        {
            get
            {
                return m_GameObject;
            }
            set
            {
                m_GameObject = value;
            }
        }
        public Transform Transform { get { return m_GameObject.transform; } }
        public virtual void Init(GameObject gameObject)
        {
            this.m_GameObject = gameObject;
            if (!gameObject.activeSelf)
                this.m_GameObject.SetActive(true);
            this.InstantiateUnityEvent();
            this.m_isInitialized = true;
            this.OnInit();
        }
        private void InstantiateUnityEvent()
        {
            if (this.m_isInitialized)
                return;
            if(this.InitEvent == null) 
                this.InitEvent = new ASUIUnityEvent();
            if (this.ShowEvent == null) 
                this.ShowEvent = new ASUIUnityEvent();
            if (this.ShowAnimationCompletedEvent == null) 
                this.ShowAnimationCompletedEvent = new ASUIUnityEvent();
            if (this.HideEvent == null) 
                this.HideEvent = new ASUIUnityEvent();
            if (this.HideAnimationCompletedEvent == null) 
                this.HideAnimationCompletedEvent = new ASUIUnityEvent();
            if (this.DestroyEvent == null) 
                this.DestroyEvent = new ASUIUnityEvent();
        }
        public abstract void OnInit();

        public virtual void Show()
        {
            this.GameObject.SetActive(true);
            this.OnShow();
            this.ShowEvent?.Invoke();
        }
        public abstract void OnShow();

        public virtual void Hide()
        {
            this.OnHide();
            this.HideEvent?.Invoke();
        }
        public abstract void OnHide();
        public virtual void Destroy(bool immediately)
        {
            if (!this.IsVisible || (this.IsVisible && immediately))
            {
                this.DestroyImmediately();
            }
            else
            {
                this.m_DestroyWhenHideCompleted = true;
                this.Hide();
            }
        }

        public virtual void DestroyImmediately()
        {
            this.m_isInitialized = false;
            this.DestroyEvent?.Invoke();
            this.RemoveAllListeners();
            GameObject.Destroy(this.GameObject);
        }

        private void RemoveAllListeners()
        {
            this.InitEvent.RemoveAllListeners();
            this.ShowEvent.RemoveAllListeners();
            this.ShowAnimationCompletedEvent.RemoveAllListeners();
            this.HideEvent.RemoveAllListeners();
            this.HideAnimationCompletedEvent.RemoveAllListeners();
            this.DestroyEvent.RemoveAllListeners();
            this.InitEvent = null;
            this.ShowEvent = null;
            this.ShowAnimationCompletedEvent = null;
            this.HideEvent = null;
            this.HideAnimationCompletedEvent = null;
            this.DestroyEvent = null;
        }

        public abstract void OnDestroy();

        public abstract void ApplyStyle();

    }
    [Serializable]
    public class ASUIUnityEvent : UnityEvent
    {
    }
    public interface IASUIInit
    {
        public void Init(GameObject gameObject);
        public void OnInit();
        public ASUIUnityEvent InitEvent { get; set; }
    }

    public interface IASUIShow
    {
        public void Show();
        public void OnShow();
        public ASUIUnityEvent ShowEvent { get; set; }
    }

    public interface IASUIHide
    {
        public void Hide();
        public void OnHide();
        public ASUIUnityEvent HideEvent { get; set; }
    }

    public interface IASUIDestroy
    {
        public void Destroy(bool immediately);
        public void OnDestroy();
        public ASUIUnityEvent DestroyEvent { get; set; }
    }

    public interface IASUIInteraction
    {
        public bool IsInteraction { get; set; }
    }
    public interface IASUIStateSwitch
    {
        public ASUIStyleState StyleState { get; set; }

        public string CurrentState { get; set; }

        public async void SwitchToState(string stateName)
        {
            var isExistState = StyleState.StateStyleDictionary.TryGetValue(stateName, out ComponentToIASUIStyleSerializedDictionary styleDic);
            if (!isExistState)
                return;
            var timer = Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1), UnityTimeProvider.Update).Take(styleDic.Count);
            IEnumerator enumerator = styleDic.GetEnumerator();
            timer.Subscribe((_) =>
            {
                if (enumerator.MoveNext())
                {
                    var kvp = (KeyValuePair<Component, IASUIStyle>)enumerator.Current;
                    kvp.Value.ApplyStyle(kvp.Key);
                }
            }
            );
            await timer.LastAsync();
        }
    }
}