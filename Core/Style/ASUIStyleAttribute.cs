
using System;

namespace ASUI
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class ASUIStyleAttribute : Attribute
    {
        public Type ComponentType { get; }
        public int Priority { get; }

        public ASUIStyleAttribute(Type ComponentType, int priority = 0)
        {
            this.ComponentType = ComponentType;
            this.Priority = priority;
        }
    }
}
