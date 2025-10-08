using System;
using System.Threading.Tasks;
using UnityEngine;

namespace ASUI
{
    /// <summary>资源加载器接口</summary>
    public interface IResourceLoader : IDisposable
    {
        /// <summary>异步加载资源</summary>
        Task<T> LoadAsync<T>(string path) where T : UnityEngine.Object;
        
        /// <summary>卸载资源</summary>
        void Unload(string path);
        
        /// <summary>释放资源</summary>
        void Release(bool force = false);
    }
}
