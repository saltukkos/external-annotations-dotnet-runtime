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

    [Flags]
    public enum CollectionAccessType
    {
        /// <summary>Method does not use or modify content of the collection.</summary>
        None = 0,
        /// <summary>Method only reads content of the collection but does not modify it.</summary>
        Read = 1,
        /// <summary>Method can change content of the collection but does not add new elements.</summary>
        ModifyExistingContent = 2,
        /// <summary>Method can add new elements to the collection.</summary>
        UpdatedContent = 6,
    }

    [AttributeUsage(AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.ReturnValue)]
    public sealed class CollectionAccessAttribute(CollectionAccessType collectionAccessType) : Attribute
    {
        public CollectionAccessType CollectionAccessType { get; } = collectionAccessType;
    }

}
