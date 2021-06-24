using System;

namespace MrMeeseeks.StaticDelegateGenerator
{

    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class StaticTypeToDelegateAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public StaticTypeToDelegateAttribute(Type type)
        {
            Type = type;
        }
    }
}
