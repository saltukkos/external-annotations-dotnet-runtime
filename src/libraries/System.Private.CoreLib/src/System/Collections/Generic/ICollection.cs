// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if MONO
using System.Diagnostics.CodeAnalysis;
#endif

namespace System.Collections.Generic
{
    // Base interface for all collections, defining enumerators, size, and
    // synchronization methods.
    public interface ICollection<T> : IEnumerable<T>
    {
        [CollectionAccess(CollectionAccessType.Read)]
        int Count
        {
#if MONO
            [DynamicDependency(nameof(Array.InternalArray__ICollection_get_Count), typeof(Array))]
#endif
            get;
        }

        [CollectionAccess(CollectionAccessType.None)]
        bool IsReadOnly
        {
#if MONO
            [DynamicDependency(nameof(Array.InternalArray__ICollection_get_IsReadOnly), typeof(Array))]
#endif
            get;
        }

#if MONO
        [DynamicDependency(nameof(Array.InternalArray__ICollection_Add) + "``1", typeof(Array))]
#endif
        [CollectionAccess(CollectionAccessType.UpdatedContent)]
        void Add(T item);

#if MONO
        [DynamicDependency(nameof(Array.InternalArray__ICollection_Clear), typeof(Array))]
#endif
        [CollectionAccess(CollectionAccessType.ModifyExistingContent)]
        void Clear();

#if MONO
        [DynamicDependency(nameof(Array.InternalArray__ICollection_Contains) + "``1", typeof(Array))]
#endif
        [CollectionAccess(CollectionAccessType.Read)]
        bool Contains(T item);

        // CopyTo copies a collection into an Array, starting at a particular
        // index into the array.
#if MONO
        [DynamicDependency(nameof(Array.InternalArray__ICollection_CopyTo) + "``1", typeof(Array))]
#endif
        [CollectionAccess(CollectionAccessType.Read)]
        void CopyTo(T[] array, int arrayIndex);

#if MONO
        [DynamicDependency(nameof(Array.InternalArray__ICollection_Remove) + "``1", typeof(Array))]
#endif
        [CollectionAccess(CollectionAccessType.Read | CollectionAccessType.ModifyExistingContent)]
        bool Remove(T item);
    }
}
