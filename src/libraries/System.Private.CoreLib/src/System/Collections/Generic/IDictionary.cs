// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;

namespace System.Collections.Generic
{
    // An IDictionary is a possibly unordered set of key-value pairs.
    // Keys can be any non-null object.  Values can be any object.
    // You can look up a value in an IDictionary via the default indexed
    // property, Items.
    public interface IDictionary<TKey, TValue> : ICollection<KeyValuePair<TKey, TValue>>
    {
        // Interfaces are not serializable
        // The Item property provides methods to read and edit entries
        // in the Dictionary.
        TValue this[TKey key]
        {
            [CollectionAccess(CollectionAccessType.Read)] get;
            [CollectionAccess(CollectionAccessType.UpdatedContent)] set;
        }

        // Returns a collections of the keys in this dictionary.
        [CollectionAccess(CollectionAccessType.Read)]
        ICollection<TKey> Keys
        {
            get;
        }

        // Returns a collections of the values in this dictionary.
        [CollectionAccess(CollectionAccessType.Read)]
        ICollection<TValue> Values
        {
            get;
        }

        // Returns whether this dictionary contains a particular key.
        //
        [CollectionAccess(CollectionAccessType.Read)]
        bool ContainsKey(TKey key);

        // Adds a key-value pair to the dictionary.
        //
        [CollectionAccess(CollectionAccessType.UpdatedContent)]
        void Add(TKey key, TValue value);

        // Removes a particular key from the dictionary.
        //
        [CollectionAccess(CollectionAccessType.ModifyExistingContent | CollectionAccessType.Read)]
        bool Remove(TKey key);

        [CollectionAccess(CollectionAccessType.Read)]
        bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value);
    }
}
