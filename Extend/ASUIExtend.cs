using UnityEngine;
using System.Collections.Generic;

namespace ASUI
{
    public static class ASUIExtend
    {
        public static T FindComponentInChildren<T>(this Component component, string childName)
        {
            Transform child = component.transform.Find(childName);
            if (child != null)
                return child.GetComponent<T>();
            return default;
        }

        public static void Set<K,V>(this Dictionary<K,V> Dictionary, K key, V Value)
        {
            if(Dictionary.ContainsKey(key))
            {
                Dictionary[key] = Value;
            }
            else
            {
                Dictionary.Add(key, Value);
            }
        }
    }
}

