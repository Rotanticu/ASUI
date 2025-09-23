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
        Uninitialized = 0,
        Entering = 1,
        Show = 2,
        Exiting = 3,
        Hide = 4,
        Destroyed = 5,
    }
    public abstract class WidgetsBase : IASUIInit, IASUIShow, IASUIHide, IASUIDestroy
    {
        private WidgetState m_WidgetState = WidgetState.Uninitialized;
        public WidgetState WidgetState{ get => m_WidgetState; set => m_WidgetState = value;}
        public bool IsInitialized { get => WidgetState != WidgetState.Uninitialized; }

        /// <summary>
        /// �Ƿ�ɱ��������ʶ��
        /// ���ڶ�����ԭ�򣬿�����ЩUIԪ��Enteringʱ����Ļ�����͸��״̬����Ҫһ��ʱ��ſɼ�
        /// ����Ƿ�ɼ���Ҫ�ھ�����������жϲ�ʵ��
        /// </summary>
        public abstract bool IsVisible { get; }

        /// <summary>
        /// �Ƿ����
        /// ֻҪ����Hide״̬�������,����Entering��Show״̬
        /// </summary>
        public bool IsShow 
        {
            get =>
                (WidgetState == WidgetState.Entering ||
                WidgetState == WidgetState.Show) &&
                this.GameObject.activeInHierarchy;
        }

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
        public GameObject GameObject { get => m_GameObject; private set => m_GameObject = value;}
        public Transform Transform { get => m_GameObject.transform; }

        private ASUIStyleState m_styleState;
        public ASUIStyleState StyleState { 
            get
            {
                if(m_styleState == null)
                {
                    m_styleState = this.GameObject.GetComponent<ASUIStyleState>();
                    if (m_styleState == null)
                    {
                        Debug.Log($"{this.GameObject?.name}û��ASUIStyleState���");
                        return null;
                    }
                }
                return m_styleState;
            } 
            set => m_styleState = value; }

        private List<WidgetsBase> m_children;
        public List<WidgetsBase> Children { get => m_children; set => m_children = value; }

        public virtual void Init(GameObject gameObject)
        {
            this.m_GameObject = gameObject;
            this.m_styleState = this.Transform.GetComponent<ASUIStyleState>();
            this.InstantiateUnityEvent();
            this.WidgetState = WidgetState.Hide;
            this.OnInit();
        }
        private void InstantiateUnityEvent()
        {
            if (this.IsInitialized)
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
            if (WidgetState == WidgetState.Entering)
            {
                await Observable.EveryUpdate()
                    .FirstAsync(_ => WidgetState != WidgetState.Entering); // �ȴ�ֱ���˳�Entering״̬
            }
        }
        public virtual void OnShow()
        {

        }

        public virtual async Task Hide()
        {
            this.OnHide();
            this.HideEvent?.Invoke();
            if (WidgetState == WidgetState.Exiting)
            {
                await Observable.EveryUpdate()
                    .FirstAsync(_ => WidgetState != WidgetState.Exiting); // �ȴ�ֱ���˳�Exiting״̬
            }
            this.GameObject.SetActive(false);
        }
        public virtual void OnHide()
        {

        }
        public virtual async Task Destroy(bool immediately)
        {
            if (this.WidgetState == WidgetState.Uninitialized ||
                this.WidgetState == WidgetState.Hide ||
                immediately)
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
            this.WidgetState = WidgetState.Destroyed;
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

        public virtual void OnDestroy()
        {

        }

        public virtual void ApplyStyle()
        {

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