
using System;

namespace ASUI
{
    [AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]

    public class ASUIStyleAttribute : Attribute
    {
        public Type ComponentType { get; }

        public ASUIStyleAttribute(Type ComponentType)
        {
            this.ComponentType = ComponentType;
        }
    }
}
