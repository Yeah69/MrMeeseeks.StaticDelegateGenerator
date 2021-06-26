using System;

namespace MrMeeseeks.StaticDelegateGenerator
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class StaticDelegateAttribute : Attribute
    {
        // ReSharper disable once UnusedParameter.Local *** Is used in the generator
        public StaticDelegateAttribute(Type type)
        {
        }
    }
}
