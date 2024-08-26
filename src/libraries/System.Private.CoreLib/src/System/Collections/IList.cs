// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Collections
{
    // An IList is an ordered collection of objects.  The exact ordering
    // is up to the implementation of the list, ranging from a sorted
    // order to insertion order.
    public interface IList : ICollection
    {
        // The Item property provides methods to read and edit entries in the List.
        object? this[int index]
        {
            [CollectionAccess(CollectionAccessType.Read)]
            get;
            [CollectionAccess(CollectionAccessType.ModifyExistingContent)]
            set;
        }

        // Adds an item to the list.  The exact position in the list is
        // implementation-dependent, so while ArrayList may always insert
        // in the last available location, a SortedList most likely would not.
        // The return value is the position the new element was inserted in.
        [CollectionAccess(CollectionAccessType.UpdatedContent | CollectionAccessType.Read)]
        int Add(object? value);

        // Returns whether the list contains a particular item.
        [CollectionAccess(CollectionAccessType.Read)]
        bool Contains([DefaultEqualityUsage] object? value);

        // Removes all items from the list.
        [CollectionAccess(CollectionAccessType.ModifyExistingContent)]
        void Clear();

        [CollectionAccess(CollectionAccessType.None)]
        bool IsReadOnly
        { get; }


        [CollectionAccess(CollectionAccessType.None)]
        bool IsFixedSize
        {
            get;
        }


        // Returns the index of a particular item, if it is in the list.
        // Returns -1 if the item isn't in the list.
        [CollectionAccess(CollectionAccessType.Read)]
        int IndexOf([DefaultEqualityUsage] object? value);

        // Inserts value into the list at position index.
        // index must be non-negative and less than or equal to the
        // number of elements in the list.  If index equals the number
        // of items in the list, then value is appended to the end.
        [CollectionAccess(CollectionAccessType.UpdatedContent)]
        void Insert(int index, object? value);

        // Removes an item from the list.
        [CollectionAccess(CollectionAccessType.ModifyExistingContent)]
        void Remove([DefaultEqualityUsage] object? value);

        // Removes the item at position index.
        [CollectionAccess(CollectionAccessType.ModifyExistingContent)]
        void RemoveAt(int index);
    }
}
