using System;
using System.Threading.Tasks;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ASUI
{
    /// <summary>编辑器资源加载器实现 - 测试用</summary>
    public class EditorResourceLoader : IResourceLoader
    {
        private bool _disposed = false;

        /// <summary>异步加载资源</summary>
        public async Task<T> LoadAsync<T>(string path) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            if (_disposed) return null;
            
            // 模拟异步加载（编辑器环境下AssetDatabase是同步的）
            await Task.Yield();
            
            var resource = AssetDatabase.LoadAssetAtPath<T>(path);
            if (resource == null)
            {
                Debug.LogError($"无法加载资源: {path}");
            }
            
            return resource;
#else
            Debug.LogError("EditorResourceLoader can only be used in the Unity Editor.");
            return null;
#endif
        }

        /// <summary>卸载资源</summary>
        public void Unload(string path)
        {
#if UNITY_EDITOR
            // 测试用，暂时不实现
            Debug.Log($"EditorResourceLoader.Unload: {path}");
#else
            Debug.LogError("EditorResourceLoader can only be used in the Unity Editor.");
#endif
        }

        /// <summary>释放资源</summary>
        public void Release(bool force = false)
        {
#if UNITY_EDITOR
            // 测试用，暂时不实现
            Debug.Log("EditorResourceLoader.Release");
#else
            Debug.LogError("EditorResourceLoader can only be used in the Unity Editor.");
#endif
        }

        /// <summary>释放资源</summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
            }
        }
    }
}
