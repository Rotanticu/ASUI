using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using UnityEngine.UI;

namespace ASUI
{
    /// <summary>
    /// ASUI 样式状态管理组件
    /// 使用 List 序列化，提供稳定的样式状态管理
    /// </summary>
    public class ASUIStyleState : MonoBehaviour
    {
        [Header("状态管理")]
        [SerializeField] private string currentState;
        [SerializeField] private string previousState;
        
        [Header("组件引用")]
        [SerializeField] private List<ASUIComponentInfo> componentInfos = new List<ASUIComponentInfo>();
        
        [Header("样式状态")]
        [SerializeField] private List<ASUIStyleStateData> styleStates = new List<ASUIStyleStateData>();
        
        // 编辑器访问属性
        public List<ASUIComponentInfo> ComponentInfos => componentInfos;
        public List<ASUIStyleStateData> StyleStates => styleStates;
        
        #region 状态管理
        
        public string CurrentState => currentState;
        public string PreviousState => previousState;
        
        private void SetCurrentState(string stateName)
        {
            previousState = currentState;
            currentState = stateName;
        }
        
        #endregion
        
        #region 组件引用管理
        
        public T GetComponentByUIName<T>(string uiName) where T : Component
        {
            // 查列表
            var info = componentInfos.FirstOrDefault(c => c.componentName == uiName);
            if (info != null && info.component is T)
            {
                return info.component as T;
            }
            return null;
        }
        
        public void AddComponentWithName(string uiName, Component component)
        {
            var existing = componentInfos.FirstOrDefault(c => c.componentName == uiName);
            if (existing != null)
            {
                existing.component = component;
                existing.componentTypeName = component.GetType().FullName;
            }
            else
            {
                componentInfos.Add(new ASUIComponentInfo
                {
                    componentName = uiName,
                    component = component,
                    componentTypeName = component.GetType().FullName
                });
            }
        }
        
        #endregion
        
        #region 状态数据管理
        
        public void AddOrUpdateState(string stateName, List<ASUIComponentStyleData> componentStyles)
        {
            var existingState = styleStates.FirstOrDefault(s => s.stateName == stateName);
            if (existingState != null)
            {
                existingState.componentStyles = componentStyles;
            }
            else
            {
                styleStates.Add(new ASUIStyleStateData
                {
                    stateName = stateName,
                    componentStyles = componentStyles
                });
            }
        }
        
        public List<ASUIComponentStyleData> GetStateData(string stateName)
        {
            var state = styleStates.FirstOrDefault(s => s.stateName == stateName);
            return state?.componentStyles;
        }
        
        public ASUIStyleStateData GetStyleState(string stateName)
        {
            return styleStates.FirstOrDefault(s => s.stateName == stateName);
        }
        
        #endregion
        
        #region 状态切换和应用
        
        public async Task ApplyState(string stateName)
        {
            if (currentState == stateName) return;
            
            var stateData = GetStateData(stateName);
            if (stateData == null)
            {
                Debug.LogWarning($"State '{stateName}' not found.");
                return;
            }

            SetCurrentState(stateName);
            
            // 应用样式到所有组件
            var tasks = new List<Task>();
            foreach (var componentStyle in stateData)
            {
                var component = GetComponentByUIName<Component>(componentStyle.componentName);
                if (component != null)
                {
                    tasks.Add(ApplyComponentStyle(component, componentStyle));
                }
            }
            
            await Task.WhenAll(tasks);
        }
        
        private async Task ApplyComponentStyle(Component component, ASUIComponentStyleData styleData)
        {
            if (styleData.style != null)
            {
                await styleData.style.ApplyStyle(component);
            }
            else
            {
                Debug.LogWarning($"Style data for component '{component.name}' is null.");
            }
        }
        
        #endregion
    }
    
    #region 数据结构定义
    
    [System.Serializable]
    public class ASUIComponentInfo
    {
        [SerializeField] public string componentName;
        [SerializeField] public Component component;
        [SerializeField] public string componentTypeName; // 存储类型名称字符串
    }
    
    [System.Serializable]
    public class ASUIStyleStateData
    {
        [SerializeField] public string stateName;
        [SerializeField] public List<ASUIComponentStyleData> componentStyles = new List<ASUIComponentStyleData>();
    }
    
    [System.Serializable]
    public class ASUIComponentStyleData
    {
        [SerializeField] public string componentName;
        [SerializeField] public string componentTypeName; // 存储类型名称字符串
        [SerializeField] public IASUIStyle style; // 直接存储样式对象
    }
    
    #endregion
}