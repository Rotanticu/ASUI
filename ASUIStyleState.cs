using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Xml.Linq;
using UnityEngine.UI;
using System.Security.AccessControl;
using UnityEngine.Rendering;

namespace ASUI
{
    //Hack: Unity 不序列化泛型类型，这么写才能在不使用Odin的情况下让Unity序列化字典
    //https://odininspector.com/tutorials/serialize-anything/serializing-dictionaries#odin-serializer
    [Serializable]
    public class ComponentToIASUIStyleSerializedDictionary : UnitySerializedDictionary<Component, IASUIStyle> { }
    [Serializable]
    public class StringToDictionaryIASUIStyleSerializedDictionary : UnitySerializedDictionary<string, ComponentToIASUIStyleSerializedDictionary> { }

    public class ASUIStyleState : MonoBehaviour
    {
        [SerializeField]
        private string m_state;
        public string State
        {
            get
            {
                return m_state;
            }
            set
            {
                m_state = value;
            }

        }
        [SerializeField]
        public List<ASUIInfo> ASUIInfoList = new List<ASUIInfo>();
        [SerializeField]
        public StringToDictionaryIASUIStyleSerializedDictionary StateStyleDictionary = new StringToDictionaryIASUIStyleSerializedDictionary();

        public T GetComponentByUIName<T>(string UIName) where T : Component
        {
            for (int i = 0; i < ASUIInfoList.Count; i++)
            {
                if (ASUIInfoList[i].UIName == UIName)
                {
                    return ASUIInfoList[i].Component as T;
                }
            }
            T findResult = this.FindComponentInChildren<T>(UIName);
            if (findResult != null)
            {
                this.AddComponentWithName(UIName, findResult);
            }
            return findResult;
        }

        public void AddComponentWithName(string UIName,Component component)
        {
            this.ASUIInfoList.Add(new ASUIInfo(UIName, component));
        }
    }

    [Serializable]
    public struct ASUIInfo
    {
        public string UIName;
        public Component Component;

        public ASUIInfo(string UIName,Component component)
        {
            this .UIName = UIName;
            this .Component = component;
        }
    }
}

