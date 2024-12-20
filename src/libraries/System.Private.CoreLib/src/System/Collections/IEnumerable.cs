// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

namespace System.Collections
{
    [Guid("496B0ABE-CDEE-11d3-88E8-00902754C43A")]
    [ComVisible(true)]
    public interface IEnumerable
    {
        // Returns an IEnumerator for this enumerable Object.  The enumerator provides
        // a simple way to access all the contents of a collection.
        [CollectionAccess(CollectionAccessType.Read)]
        IEnumerator GetEnumerator();
    }
}
