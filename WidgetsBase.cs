using NUnit.Framework;
using R3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace ASUI
{
    public abstract class WidgetsBase : IASUIInit, IASUIShow, IASUIHide, IASUIDestroy
    {
        private bool m_isInitialized;
        public bool IsInitialized { get => m_isInitialized;}

        public abstract bool IsVisible { get; }

        public abstract bool IsShow { get; }

        [SerializeField]
        private ASUIUnityEvent m_InitEvent;
        public ASUIUnityEvent InitEvent { get => m_InitEvent; set => m_InitEvent = value; }
        [SerializeField]
        private ASUIUnityEvent m_ShowEvent;
        public ASUIUnityEvent ShowEvent { get => m_ShowEvent; set => m_ShowEvent = value; }
        [SerializeField]
        private ASUIUnityEvent m_ShowAnimationCompletedEvent;
        public ASUIUnityEvent ShowAnimationCompletedEvent { get => m_ShowAnimationCompletedEvent; set => m_ShowAnimationCompletedEvent = value; }
        [SerializeField]
        private ASUIUnityEvent m_HideEvent;
        public ASUIUnityEvent HideEvent { get => m_HideEvent; set => m_HideEvent = value; }
        [SerializeField]
        private ASUIUnityEvent m_HideAnimationCompletedEvent;
        public ASUIUnityEvent HideAnimationCompletedEvent { get => m_HideAnimationCompletedEvent; set => m_HideAnimationCompletedEvent = value; }
        [SerializeField]
        private ASUIUnityEvent m_DestroyEvent;
        public ASUIUnityEvent DestroyEvent { get => m_DestroyEvent; set => m_DestroyEvent = value; }

        private GameObject m_GameObject;
        public GameObject GameObject { get => m_GameObject; set => m_GameObject = value;}
        public Transform Transform { get => m_GameObject.transform; }

        public ASUIStyleState styleState;
        public ASUIStyleState StyleState { get => styleState; set => styleState = value; }

        private List<WidgetsBase> m_widgetsCollection;
        public List<WidgetsBase> Children { get => m_widgetsCollection; set => m_widgetsCollection = value; }

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
            this.InitEvent ??= new ASUIUnityEvent();
            this.ShowEvent ??= new ASUIUnityEvent();
            this.ShowAnimationCompletedEvent ??= new ASUIUnityEvent();
            this.HideEvent ??= new ASUIUnityEvent();
            this.HideAnimationCompletedEvent ??= new ASUIUnityEvent();
            this.DestroyEvent ??= new ASUIUnityEvent();
        }
        public abstract void OnInit();

        public virtual async Task Show()
        {
            this.GameObject.SetActive(true);
            this.OnShow();
            this.ShowEvent?.Invoke();
            if (!IsShow)
            {
                await Observable.EveryUpdate()
                    .FirstAsync(_ => IsShow); // 等待直到IsShow为true
            }
        }
        public abstract void OnShow();

        public virtual async Task Hide()
        {
            this.OnHide();
            this.HideEvent?.Invoke();
            if (IsVisible)
            {
                await Observable.EveryUpdate()
                    .FirstAsync(_ => !IsVisible); // 等待直到IsVisible为false
            }
        }
        public abstract void OnHide();
        public virtual async Task Destroy(bool immediately)
        {
            if (!this.IsVisible || immediately)
            {
                this.DestroyImmediately();
            }
            else
            {
                await this.Hide();
                this.DestroyImmediately();
            }
        }

        public virtual void DestroyImmediately()
        {
            this.m_isInitialized = false;
            this.DestroyEvent?.Invoke();
            this.RemoveAllListeners();
            if(this.Children != null)
            {
                foreach (var widget in this.Children)
                {
                    widget.DestroyImmediately();
                }
            }
            this.Children = null;
            GameObject.Destroy(this.GameObject);
        }

        private void RemoveAllListeners()
        {
            this.InitEvent?.RemoveAllListeners();
            this.ShowEvent?.RemoveAllListeners();
            this.ShowAnimationCompletedEvent?.RemoveAllListeners();
            this.HideEvent?.RemoveAllListeners();
            this.HideAnimationCompletedEvent?.RemoveAllListeners();
            this.DestroyEvent?.RemoveAllListeners();
            this.InitEvent = null;
            this.ShowEvent = null;
            this.ShowAnimationCompletedEvent = null;
            this.HideEvent = null;
            this.HideAnimationCompletedEvent = null;
            this.DestroyEvent = null;
        }

        public abstract void OnDestroy();

        public abstract void ApplyStyle();

        public T CreateWidget<T>(GameObject gameObject) where T : WidgetsBase, new()
        {
            T widget = new();
            widget.Init(gameObject);
            this.Children ??= new List<WidgetsBase>();
            this.Children.Add(widget);
            return widget;
        }

        public async Task DestroyWidget(WidgetsBase widgetsBase, bool immediately = false)
        {
            if (this.Children != null)
            {
                if (this.Children.Contains(widgetsBase))
                    this.Children.Remove(widgetsBase);
            }
            await widgetsBase.Destroy(immediately);
        }

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
        public Task Show();
        public void OnShow();
        public ASUIUnityEvent ShowEvent { get; set; }
    }

    public interface IASUIHide
    {
        public Task Hide();
        public void OnHide();
        public ASUIUnityEvent HideEvent { get; set; }
    }

    public interface IASUIDestroy
    {
        public Task Destroy(bool immediately);
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