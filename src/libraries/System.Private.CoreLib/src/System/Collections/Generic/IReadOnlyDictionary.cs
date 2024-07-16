// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;

namespace System.Collections.Generic
{
    // Provides a read-only view of a generic dictionary.
    public interface IReadOnlyDictionary<TKey, TValue> : IReadOnlyCollection<KeyValuePair<TKey, TValue>>
    {
        [CollectionAccess(CollectionAccessType.Read)]
        bool ContainsKey(TKey key);
        [CollectionAccess(CollectionAccessType.Read)]
        bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value);

        TValue this[TKey key] { [CollectionAccess(CollectionAccessType.Read)] get; }
        [CollectionAccess(CollectionAccessType.Read)] IEnumerable<TKey> Keys { get; }
        [CollectionAccess(CollectionAccessType.Read)] IEnumerable<TValue> Values { get; }
    }
}
