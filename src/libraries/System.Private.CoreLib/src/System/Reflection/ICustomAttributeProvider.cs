// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Reflection
{
    public interface ICustomAttributeProvider
    {
        [return: CollectionAccess(CollectionAccessType.UpdatedContent)]
        object[] GetCustomAttributes(bool inherit);

        [return: CollectionAccess(CollectionAccessType.UpdatedContent)]
        object[] GetCustomAttributes(Type attributeType, bool inherit);
        bool IsDefined(Type attributeType, bool inherit);
    }
}
