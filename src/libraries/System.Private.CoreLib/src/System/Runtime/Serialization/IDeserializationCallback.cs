// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Runtime.Serialization
{
    public interface IDeserializationCallback
    {
        [CollectionAccess(CollectionAccessType.UpdatedContent)]
        void OnDeserialization(object? sender);
    }
}
