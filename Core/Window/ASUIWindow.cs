using R3;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace ASUI
{
    /// <summary>
    /// ASUI窗口 - 数据驱动的窗口容器，负责路由、资源加载缓存、数据接收入口
    /// 组合Widget而不是继承Widget
    /// </summary>
    public class ASUIWindow
    {
        /// <summary>窗口唯一标识</summary>
        public string WindowId { get; private set; }
        
        /// <summary>窗口名称</summary>
        public string WindowName { get; private set; }
        
        /// <summary>窗口是否活跃</summary>
        public bool IsActive { get; private set; }
        
        /// <summary>窗口创建时调用</summary>
        public Action OnCreated { get; set; }
        
        /// <summary>窗口销毁时调用</summary>
        public Action OnDestroyed { get; set; }
        
        /// <summary>窗口显示时调用</summary>
        public Action OnShow { get; set; }
        
        /// <summary>窗口隐藏时调用</summary>
        public Action OnHide { get; set; }
        
        /// <summary>数据更新时调用</summary>
        public Action<object> OnDataUpdated { get; set; }
        
        /// <summary>数据加载错误时调用</summary>
        public Action<Exception> OnError { get; set; }
        
        /// <summary>数据流订阅</summary>
        private IDisposable _dataSubscription;
        
        /// <summary>资源加载器</summary>
        private IResourceLoader _resourceLoader;
        
        /// <summary>主Widget</summary>
        private WidgetsBase _mainWidget;
        
        public ASUIWindow(string windowName, WidgetsBase mainWidget, IResourceLoader resourceLoader = null)
        {
            WindowName = windowName;           // 开发者定义的类型名称
            WindowId = Guid.NewGuid().ToString(); // 系统自动分配的唯一ID
            IsActive = false;
            
#if UNITY_EDITOR
            // 设置资源加载器，没有传入就内部创建
            _resourceLoader = resourceLoader ?? new EditorResourceLoader();
#else
            //根据项目具体资源加载方案实现自定义加载器
            //_resourceLoader = resourceLoader ?? new MyResourceLoader();
#endif
            
            // 设置主Widget
            _mainWidget = mainWidget;
            
        }
        
        /// <summary>激活窗口</summary>
        public void Activate()
        {
            if (IsActive) return;
            IsActive = true;
            OnShow?.Invoke();
        }
        
        /// <summary>停用窗口</summary>
        public void Deactivate()
        {
            if (!IsActive) return;
            IsActive = false;
            OnHide?.Invoke();
        }
        
        /// <summary>订阅数据流</summary>
        public void SubscribeDataStream<T>(Observable<T> dataStream)
        where T : ASUIContext
        {
            _dataSubscription?.Dispose();
            
            _dataSubscription = dataStream.Subscribe(
                data =>
                {
                    AttachToContext(data);
                    OnDataUpdated?.Invoke(data);
                    //_mainWidget.OnDataUpdated(data);
                },
                error => OnError?.Invoke(error),
                result => { }
            );
        }
        
        /// <summary>取消数据流订阅</summary>
        public void UnsubscribeDataStream()
        {
            _dataSubscription?.Dispose();
            _dataSubscription = null;
        }
        
        /// <summary>获取资源加载器</summary>
        public IResourceLoader GetResourceLoader()
        {
            return _resourceLoader;
        }
        
        /// <summary>获取主Widget</summary>
        public WidgetsBase GetMainWidget()
        {
            return _mainWidget;
        }
        
        /// <summary>向Context添加Window信息</summary>
        public void AttachToContext(ASUIContext context)
        {
            if (context == null) return;
            
            if (context.Window == null)
            {
                context.Window = this;
            }
        }
        
        /// <summary>泛型创建Window</summary>
        public static ASUIWindow Create<T>(string windowName, IResourceLoader resourceLoader = null) 
            where T : WidgetsBase, new()
        {
            var widget = new T();
            return new ASUIWindow(windowName, widget, resourceLoader);
        }
        
        /// <summary>销毁窗口</summary>
        public void Destroy()
        {
            UnsubscribeDataStream();
            _mainWidget?.Destroy(false);
            _mainWidget = null;
            _resourceLoader?.Dispose();
            _resourceLoader = null;
            OnDestroyed?.Invoke();
        }
    }
}