// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Collections
{
    // An IDictionary is a possibly unordered set of key-value pairs.
    // Keys can be any non-null object.  Values can be any object.
    // You can look up a value in an IDictionary via the default indexed
    // property, Items.
    public interface IDictionary : ICollection
    {
        // Interfaces are not serializable
        // The Item property provides methods to read and edit entries
        // in the Dictionary.
        object? this[object key]
        {
            [CollectionAccess(CollectionAccessType.Read)] get;
            [CollectionAccess(CollectionAccessType.UpdatedContent)] set;
        }

        // Returns a collections of the keys in this dictionary.
        [CollectionAccess(CollectionAccessType.Read)]
        ICollection Keys { get; }

        // Returns a collections of the values in this dictionary.
        [CollectionAccess(CollectionAccessType.Read)]
        ICollection Values { get; }

        // Returns whether this dictionary contains a particular key.
        [CollectionAccess(CollectionAccessType.Read)]
        bool Contains(object key);

        // Adds a key-value pair to the dictionary.
        [CollectionAccess(CollectionAccessType.UpdatedContent)]
        void Add(object key, object? value);

        // Removes all pairs from the dictionary.
        [CollectionAccess(CollectionAccessType.ModifyExistingContent)]
        void Clear();

        [CollectionAccess(CollectionAccessType.None)]
        bool IsReadOnly { get; }

        [CollectionAccess(CollectionAccessType.None)]
        bool IsFixedSize { get; }

        // Returns an IDictionaryEnumerator for this dictionary.
        [CollectionAccess(CollectionAccessType.Read)]
        new IDictionaryEnumerator GetEnumerator();

        // Removes a particular key from the dictionary.
        [CollectionAccess(CollectionAccessType.ModifyExistingContent)]
        void Remove(object key);
    }
}
