using System;

namespace JetBrains.Annotations
{
    [AttributeUsage(AttributeTargets.GenericParameter | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    public class DefaultEqualityUsageAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.All)]
    public class DefaultEqualityUsageInternalAttribute : Attribute
    {
        public DefaultEqualityUsageInternalAttribute(params string[] names)
        {
        }
    }
}
