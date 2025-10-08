using System.Collections.Generic;
using UnityEngine;

namespace ASUI
{
    /// <summary>ASUI上下文类</summary>
    public class ASUIContext
    {
        /// <summary>所属窗口</summary>
        public ASUIWindow Window { get; set; }
        
        /// <summary>重置上下文</summary>
        public virtual void Reset()
        {
            Window = null;
        }
    }
}
