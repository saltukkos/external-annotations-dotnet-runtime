// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System
{
    public interface ICloneable
    {
        [CollectionAccess(CollectionAccessType.Read)]
        object Clone();
    }
}
