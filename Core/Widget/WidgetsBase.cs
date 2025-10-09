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
    public enum WidgetState
    {

        Showing = 1,
        Showed = 2,
        Hiding = 3,
        Hideen = 4,


        Uninitialized = 0,
        Transitioning = 6,
        Idle = 5,
        Destroyed = 5,
    }
    public abstract class WidgetsBase
    {
        private WidgetState m_WidgetState = WidgetState.Uninitialized;
        public WidgetState WidgetState { get => m_WidgetState; set => m_WidgetState = value; }


        private string PrevTransitionName;
        public string TransitionName { get; set; }
        public bool IsInitialized { get => WidgetState != WidgetState.Uninitialized; }

        [SerializeField]
        private ASUIUnityEvent _initEvent;
        public ASUIUnityEvent InitEvent { get => _initEvent; set => _initEvent = value; }
        [SerializeField]
        private ASUIUnityEvent _destroyEvent;
        public ASUIUnityEvent DestroyEvent { get => _destroyEvent; set => _destroyEvent = value; }

        [SerializeField]
        private ASUITransUnityEvent _startTransitionEvent;
        public ASUITransUnityEvent StartTransitionEvent { get => _startTransitionEvent; set => _startTransitionEvent = value; }
        [SerializeField]
        private ASUITransUnityEvent _endTransitionEvent;
        public ASUITransUnityEvent EndTransitionEvent { get => _endTransitionEvent; set => _endTransitionEvent = value; }

        private GameObject _gameObject;
        public GameObject GameObject { get => _gameObject; private set => _gameObject = value; }
        public Transform Transform { get => _gameObject.transform; }

        private ASUIStyleState _styleState;
        public ASUIStyleState StyleState
        {
            get
            {
                if (_styleState == null)
                {
                    _styleState = this.GameObject.GetComponent<ASUIStyleState>();
                    if (_styleState == null)
                    {
                        Debug.Log($"{this.GameObject?.name}没有ASUIStyleState组件");
                        return null;
                    }
                }
                return _styleState;
            }
            set => _styleState = value;
        }

        private List<WidgetsBase> _children;
        public List<WidgetsBase> Children { get => _children; set => _children = value; }

        public virtual void Init(GameObject gameObject)
        {
            this._gameObject = gameObject;
            this._styleState = this.Transform.GetComponent<ASUIStyleState>();
            this.InstantiateUnityEvent();
            this.WidgetState = WidgetState.Idle;
            this.OnInit();
        }
        private void InstantiateUnityEvent()
        {
            if (this.IsInitialized)
                return;
            this.InitEvent ??= new ASUIUnityEvent();
            this.StartTransitionEvent ??= new ASUITransUnityEvent();
            this.EndTransitionEvent ??= new ASUITransUnityEvent();
            this.DestroyEvent ??= new ASUIUnityEvent();
        }
        public virtual void OnInit()
        {

        }

        public virtual async Task Transition(string transitionName)
        {
            if (!this.IsInitialized)
                return;
            if (this.TransitionName == transitionName)
                return;
            if (this.StyleState)
                this.PrevTransitionName = this.TransitionName;
            this.TransitionName = transitionName;
            this.WidgetState = WidgetState.Transitioning;
            this.StartTransitionEvent?.Invoke(transitionName);
            this.OnStartTransition(transitionName);
            await Task.WhenAll(this.Children.Select(child => child.Transition(transitionName))
            .Append(ApplyStyle(this.PrevTransitionName == transitionName)));
            this.WidgetState = WidgetState.Idle;
            this.OnEndTransition(transitionName);
            this.EndTransitionEvent?.Invoke(transitionName);
        }
        public virtual async Task Destroy(bool immediately)
        {
            this.WidgetState = WidgetState.Destroyed;
            this.DestroyEvent?.Invoke();
            this.RemoveAllListeners();
            if (!immediately)
                await Task.WhenAll(this.Children.Select(child => child.Transition("Hide")).Append(this.Transition("Hide")));
            await Task.WhenAll(this.Children.Select(child => child.Destroy(immediately)).ToArray());
            GameObject.Destroy(this.GameObject);
        }

        public virtual void OnStartTransition(string transitionName)
        {
            
        }

        public virtual void OnEndTransition(string transitionName)
        {
            
        }

        private void RemoveAllListeners()
        {
            this.InitEvent?.RemoveAllListeners();
            this.StartTransitionEvent?.RemoveAllListeners();
            this.EndTransitionEvent?.RemoveAllListeners();
            this.DestroyEvent?.RemoveAllListeners();
            this.InitEvent = null;
            this.StartTransitionEvent = null;
            this.EndTransitionEvent = null;
            this.DestroyEvent = null;
        }

        public virtual void OnDestroy()
        {
            
        }

        public async virtual Task ApplyStyle(bool isReverse)
        {
            await this.StyleState.ApplyState(this.TransitionName);
        }

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
    public class ASUITransUnityEvent : UnityEvent<string>
    {
    }
    
    [Serializable]
    public class ASUIUnityEvent : UnityEvent
    {
    }
}