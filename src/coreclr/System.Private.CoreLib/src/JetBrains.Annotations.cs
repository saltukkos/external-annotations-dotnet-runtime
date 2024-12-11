using System;
using System.Diagnostics.CodeAnalysis;

namespace JetBrains.Annotations
{
    [AttributeUsage(AttributeTargets.GenericParameter | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    public class DefaultEqualityUsageAttribute : Attribute;

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

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class CheckForPublicUnannotatedMembersInternalAttribute : Attribute
    {
        public CheckForPublicUnannotatedMembersInternalAttribute(
            [StringSyntax(StringSyntaxAttribute.Regex)] string memberNameRegex,
            string requiredAttributeFqn)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.GenericParameter | AttributeTargets.Parameter)]
    public sealed class MeansImplicitUseAttribute : Attribute
    {
        public MeansImplicitUseAttribute(ImplicitUseKindFlags useKindFlags, ImplicitUseTargetFlags targetFlags)
        {
          UseKindFlags = useKindFlags;
          TargetFlags = targetFlags;
        }

        public ImplicitUseKindFlags UseKindFlags { get; }

        public ImplicitUseTargetFlags TargetFlags { get; }
    }

    /// <summary>
    /// Specifies the details of an implicitly used symbol when it is marked
    /// with <see cref="MeansImplicitUseAttribute"/> or <see cref="UsedImplicitlyAttribute"/>.
    /// </summary>
    [Flags]
    public enum ImplicitUseKindFlags
    {
        Default = Access | Assign | InstantiatedWithFixedConstructorSignature,
        /// <summary>Only entity marked with attribute considered used.</summary>
        Access = 1,
        /// <summary>Indicates implicit assignment to a member.</summary>
        Assign = 2,
        /// <summary>
        /// Indicates implicit instantiation of a type with fixed constructor signature.
        /// That means any unused constructor parameters will not be reported as such.
        /// </summary>
        InstantiatedWithFixedConstructorSignature = 4,
        /// <summary>Indicates implicit instantiation of a type.</summary>
        InstantiatedNoFixedConstructorSignature = 8,
    }

    /// <summary>
    /// Specifies what is considered to be used implicitly when marked
    /// with <see cref="MeansImplicitUseAttribute"/> or <see cref="UsedImplicitlyAttribute"/>.
    /// </summary>
    [Flags]
    public enum ImplicitUseTargetFlags
    {
        Default = Itself,
        Itself = 1,
        /// <summary>Members of the type marked with the attribute are considered used.</summary>
        Members = 2,
        /// <summary> Inherited entities are considered used. </summary>
        WithInheritors = 4,
        /// <summary>Entity marked with the attribute and all its members considered used.</summary>
        WithMembers = Itself | Members
    }
}
