// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace System.Collections.Specialized
{
    /// <devdoc>
    /// <para>
    /// OrderedDictionary offers IDictionary syntax with ordering.  Objects
    /// added or inserted in an IOrderedDictionary must have both a key and an index, and
    /// can be retrieved by either.
    /// OrderedDictionary is used by the ParameterCollection because MSAccess relies on ordering of
    /// parameters, while almost all other DBs do not.  DataKeyArray also uses it so
    /// DataKeys can be retrieved by either their name or their index.
    ///
    /// OrderedDictionary implements IDeserializationCallback because it needs to have the
    /// contained ArrayList and Hashtable deserialized before it tries to get its count and objects.
    /// </para>
    /// </devdoc>
    [Serializable]
    [System.Runtime.CompilerServices.TypeForwardedFrom("System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    public class OrderedDictionary : IOrderedDictionary, ISerializable, IDeserializationCallback
    {
        private ArrayList? _objectsArray;
        private Hashtable? _objectsTable;
        private int _initialCapacity;
        private IEqualityComparer? _comparer;
        private bool _readOnly;
        private readonly SerializationInfo? _siInfo; //A temporary variable which we need during deserialization.

        private const string KeyComparerName = "KeyComparer"; // Do not rename (binary serialization)
        private const string ArrayListName = "ArrayList"; // Do not rename (binary serialization)
        private const string ReadOnlyName = "ReadOnly"; // Do not rename (binary serialization)
        private const string InitCapacityName = "InitialCapacity"; // Do not rename (binary serialization)

        public OrderedDictionary() : this(0)
        {
        }

        public OrderedDictionary(int capacity) : this(capacity, null)
        {
        }

        public OrderedDictionary(IEqualityComparer? comparer) : this(0, comparer)
        {
        }

        public OrderedDictionary(int capacity, IEqualityComparer? comparer)
        {
            _initialCapacity = capacity;
            _comparer = comparer;
        }

        private OrderedDictionary(OrderedDictionary dictionary)
        {
            Debug.Assert(dictionary != null);

            _readOnly = true;
            _objectsArray = dictionary._objectsArray;
            _objectsTable = dictionary._objectsTable;
            _comparer = dictionary._comparer;
            _initialCapacity = dictionary._initialCapacity;
        }

        [Obsolete(Obsoletions.LegacyFormatterImplMessage, DiagnosticId = Obsoletions.LegacyFormatterImplDiagId, UrlFormat = Obsoletions.SharedUrlFormat)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected OrderedDictionary(SerializationInfo info, StreamingContext context)
        {
            // We can't do anything with the keys and values until the entire graph has been deserialized
            // and getting Counts and objects won't fail.  For the time being, we'll just cache this.
            // The graph is not valid until OnDeserialization has been called.
            _siInfo = info;
        }

        /// <devdoc>
        /// Gets the size of the table.
        /// </devdoc>
        public int Count
        {
            get
            {
                if (_objectsArray == null)
                {
                    return 0;
                }
                return _objectsArray.Count;
            }
        }

        /// <devdoc>
        /// Indicates that the collection can grow.
        /// </devdoc>
        bool IDictionary.IsFixedSize
        {
            get
            {
                return _readOnly;
            }
        }

        /// <devdoc>
        /// Indicates that the collection is not read-only
        /// </devdoc>
        public bool IsReadOnly
        {
            get
            {
                return _readOnly;
            }
        }

        /// <devdoc>
        /// Indicates that this class is not synchronized
        /// </devdoc>
        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }

        /// <devdoc>
        /// Gets the collection of keys in the table in order.
        /// </devdoc>
        public ICollection Keys
        {
            get
            {
                ArrayList objectsArray = EnsureObjectsArray();
                Hashtable objectsTable = EnsureObjectsTable();
                return new OrderedDictionaryKeyValueCollection(objectsArray, objectsTable, _comparer);
            }
        }

        private ArrayList EnsureObjectsArray() => _objectsArray ??= new ArrayList(_initialCapacity);

        private Hashtable EnsureObjectsTable() => _objectsTable ??= new Hashtable(_initialCapacity, _comparer);

        /// <devdoc>
        /// The SyncRoot object.  Not used because IsSynchronized is false
        /// </devdoc>
        object ICollection.SyncRoot => this;

        /// <devdoc>
        /// Gets or sets the object at the specified index
        /// </devdoc>
        public object? this[int index]
        {
            get
            {
                ArrayList objectsArray = EnsureObjectsArray();
                return ((DictionaryEntry)objectsArray[index]!).Value;
            }
            set
            {
                if (_readOnly)
                {
                    throw new NotSupportedException(SR.OrderedDictionary_ReadOnly);
                }
                if (_objectsArray == null || index < 0 || index >= _objectsArray.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                ArrayList objectsArray = EnsureObjectsArray();
                Hashtable objectsTable = EnsureObjectsTable();
                object key = ((DictionaryEntry)objectsArray[index]!).Key;
                objectsArray[index] = new DictionaryEntry(key, value);
                objectsTable[key] = value;
            }
        }

        /// <devdoc>
        /// Gets or sets the object with the specified key
        /// </devdoc>
        public object? this[object key]
        {
            get
            {
                if (_objectsTable == null)
                {
                    return null;
                }
                return _objectsTable[key];
            }
            set
            {
                if (_readOnly)
                {
                    throw new NotSupportedException(SR.OrderedDictionary_ReadOnly);
                }
                Hashtable objectsTable = EnsureObjectsTable();
                if (objectsTable.Contains(key))
                {
                    objectsTable[key] = value;
                    ArrayList objectsArray = EnsureObjectsArray();
                    objectsArray[IndexOfKey(key)] = new DictionaryEntry(key, value);
                }
                else
                {
                    Add(key, value);
                }
            }
        }

        /// <devdoc>
        /// Returns an arrayList of the values in the table
        /// </devdoc>
        public ICollection Values
        {
            get
            {
                ArrayList objectsArray = EnsureObjectsArray();
                return new OrderedDictionaryKeyValueCollection(objectsArray);
            }
        }

        /// <devdoc>
        /// Adds a new entry to the table with the lowest-available index.
        /// </devdoc>
        public void Add(object key, object? value)
        {
            if (_readOnly)
            {
                throw new NotSupportedException(SR.OrderedDictionary_ReadOnly);
            }
            Hashtable objectsTable = EnsureObjectsTable();
            ArrayList objectsArray = EnsureObjectsArray();
            objectsTable.Add(key, value);
            objectsArray.Add(new DictionaryEntry(key, value));
        }

        /// <devdoc>
        /// Clears all elements in the table.
        /// </devdoc>
        public void Clear()
        {
            if (_readOnly)
            {
                throw new NotSupportedException(SR.OrderedDictionary_ReadOnly);
            }
            _objectsTable?.Clear();
            _objectsArray?.Clear();
        }

        /// <devdoc>
        /// Returns a readonly OrderedDictionary for the given OrderedDictionary.
        /// </devdoc>
        [CollectionAccess(CollectionAccessType.Read)]
        [return: CollectionAccess(CollectionAccessType.UpdatedContent)]
        public OrderedDictionary AsReadOnly()
        {
            return new OrderedDictionary(this);
        }

        /// <devdoc>
        /// Returns true if the key exists in the table, false otherwise.
        /// </devdoc>
        public bool Contains(object key)
        {
            ArgumentNullException.ThrowIfNull(key);

            if (_objectsTable == null)
            {
                return false;
            }
            return _objectsTable.Contains(key);
        }

        /// <devdoc>
        /// Copies the table to an array.  This will not preserve order.
        /// </devdoc>
        public void CopyTo(Array array, int index)
        {
            Hashtable objectsTable = EnsureObjectsTable();
            objectsTable.CopyTo(array, index);
        }

        private int IndexOfKey(object key)
        {
            if (_objectsArray == null)
            {
                return -1;
            }
            for (int i = 0; i < _objectsArray.Count; i++)
            {
                object o = ((DictionaryEntry)_objectsArray[i]!).Key;
                if (_comparer != null)
                {
                    if (_comparer.Equals(o, key))
                    {
                        return i;
                    }
                }
                else
                {
                    if (o.Equals(key))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        /// <devdoc>
        /// Inserts a new object at the given index with the given key.
        /// </devdoc>
        public void Insert(int index, object key, object? value)
        {
            if (_readOnly)
            {
                throw new NotSupportedException(SR.OrderedDictionary_ReadOnly);
            }
            ArgumentOutOfRangeException.ThrowIfGreaterThan(index, Count);
            ArgumentOutOfRangeException.ThrowIfNegative(index);
            Hashtable objectsTable = EnsureObjectsTable();
            ArrayList objectsArray = EnsureObjectsArray();
            objectsTable.Add(key, value);
            objectsArray.Insert(index, new DictionaryEntry(key, value));
        }

        /// <devdoc>
        /// Removes the entry at the given index.
        /// </devdoc>
        public void RemoveAt(int index)
        {
            if (_readOnly)
            {
                throw new NotSupportedException(SR.OrderedDictionary_ReadOnly);
            }
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Count);
            ArgumentOutOfRangeException.ThrowIfNegative(index);
            Hashtable objectsTable = EnsureObjectsTable();
            ArrayList objectsArray = EnsureObjectsArray();
            object key = ((DictionaryEntry)objectsArray[index]!).Key;
            objectsArray.RemoveAt(index);
            objectsTable.Remove(key);
        }

        /// <devdoc>
        /// Removes the entry with the given key.
        /// </devdoc>
        public void Remove(object key)
        {
            if (_readOnly)
            {
                throw new NotSupportedException(SR.OrderedDictionary_ReadOnly);
            }
            ArgumentNullException.ThrowIfNull(key);

            int index = IndexOfKey(key);
            if (index < 0)
            {
                return;
            }

            Hashtable objectsTable = EnsureObjectsTable();
            ArrayList objectsArray = EnsureObjectsArray();
            objectsTable.Remove(key);
            objectsArray.RemoveAt(index);
        }

#region IDictionary implementation
        public virtual IDictionaryEnumerator GetEnumerator()
        {
            ArrayList objectsArray = EnsureObjectsArray();
            return new OrderedDictionaryEnumerator(objectsArray, OrderedDictionaryEnumerator.DictionaryEntry);
        }
#endregion

#region IEnumerable implementation
        IEnumerator IEnumerable.GetEnumerator()
        {
            ArrayList objectsArray = EnsureObjectsArray();
            return new OrderedDictionaryEnumerator(objectsArray, OrderedDictionaryEnumerator.DictionaryEntry);
        }
#endregion

#region ISerializable implementation
        [Obsolete(Obsoletions.LegacyFormatterImplMessage, DiagnosticId = Obsoletions.LegacyFormatterImplDiagId, UrlFormat = Obsoletions.SharedUrlFormat)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            ArgumentNullException.ThrowIfNull(info);

            info.AddValue(KeyComparerName, _comparer, typeof(IEqualityComparer));
            info.AddValue(ReadOnlyName, _readOnly);
            info.AddValue(InitCapacityName, _initialCapacity);

            object[] serArray = new object[Count];
            ArrayList objectsArray = EnsureObjectsArray();
            objectsArray.CopyTo(serArray);
            info.AddValue(ArrayListName, serArray);
        }
#endregion

#region IDeserializationCallback implementation
        void IDeserializationCallback.OnDeserialization(object? sender)
        {
            OnDeserialization(sender);
        }

        protected virtual void OnDeserialization(object? sender)
        {
            if (_siInfo == null)
            {
                throw new SerializationException(SR.Serialization_InvalidOnDeser);
            }
            _comparer = (IEqualityComparer?)_siInfo.GetValue(KeyComparerName, typeof(IEqualityComparer));
            _readOnly = _siInfo.GetBoolean(ReadOnlyName);
            _initialCapacity = _siInfo.GetInt32(InitCapacityName);

            object[]? serArray = (object[]?)_siInfo.GetValue(ArrayListName, typeof(object[]));

            if (serArray != null)
            {
                Hashtable objectsTable = EnsureObjectsTable();
                ArrayList objectsArray = EnsureObjectsArray();
                foreach (object o in serArray)
                {
                    DictionaryEntry entry;
                    try
                    {
                        // DictionaryEntry is a value type, so it can only be casted.
                        entry = (DictionaryEntry)o;
                    }
                    catch
                    {
                        throw new SerializationException(SR.OrderedDictionary_SerializationMismatch);
                    }
                    objectsArray.Add(entry);
                    objectsTable.Add(entry.Key, entry.Value);
                }
            }
        }
#endregion

        /// <devdoc>
        /// OrderedDictionaryEnumerator works just like any other IDictionaryEnumerator, but it retrieves DictionaryEntries
        /// in the order by index.
        /// </devdoc>
        private sealed class OrderedDictionaryEnumerator : IDictionaryEnumerator
        {
            private readonly int _objectReturnType;
            internal const int Keys = 1;
            internal const int Values = 2;
            internal const int DictionaryEntry = 3;
            private readonly IEnumerator _arrayEnumerator;

            internal OrderedDictionaryEnumerator(ArrayList array, int objectReturnType)
            {
                _arrayEnumerator = array.GetEnumerator();
                _objectReturnType = objectReturnType;
            }

            /// <devdoc>
            /// Retrieves the current DictionaryEntry.  This is the same as Entry, but not strongly-typed.
            /// </devdoc>
            public object? Current
            {
                get
                {
                    Debug.Assert(_arrayEnumerator.Current != null);
                    if (_objectReturnType == Keys)
                    {
                        return ((DictionaryEntry)_arrayEnumerator.Current).Key;
                    }
                    if (_objectReturnType == Values)
                    {
                        return ((DictionaryEntry)_arrayEnumerator.Current).Value;
                    }
                    return Entry;
                }
            }

            /// <devdoc>
            /// Retrieves the current DictionaryEntry
            /// </devdoc>
            public DictionaryEntry Entry
            {
                get
                {
                    Debug.Assert(_arrayEnumerator.Current != null);
                    return new DictionaryEntry(((DictionaryEntry)_arrayEnumerator.Current).Key, ((DictionaryEntry)_arrayEnumerator.Current).Value);
                }
            }

            /// <devdoc>
            /// Retrieves the key of the current DictionaryEntry
            /// </devdoc>
            public object Key
            {
                get
                {
                    Debug.Assert(_arrayEnumerator.Current != null);
                    return ((DictionaryEntry)_arrayEnumerator.Current).Key;
                }
            }

            /// <devdoc>
            /// Retrieves the value of the current DictionaryEntry
            /// </devdoc>
            public object? Value
            {
                get
                {
                    Debug.Assert(_arrayEnumerator.Current != null);
                    return ((DictionaryEntry)_arrayEnumerator.Current).Value;
                }
            }

            /// <devdoc>
            /// Moves the enumerator pointer to the next member
            /// </devdoc>
            public bool MoveNext()
            {
                return _arrayEnumerator.MoveNext();
            }

            /// <devdoc>
            /// Resets the enumerator pointer to the beginning.
            /// </devdoc>
            public void Reset()
            {
                _arrayEnumerator.Reset();
            }
        }

        /// <devdoc>
        /// OrderedDictionaryKeyValueCollection implements IList for the Values and Keys properties
        /// that is "live"- it will reflect changes to the OrderedDictionary on the collection made after the getter
        /// was called.
        /// </devdoc>
        private sealed class OrderedDictionaryKeyValueCollection : IList
        {
            private readonly ArrayList _objects;
            private readonly Hashtable? _objectsTable;
            private readonly IEqualityComparer? _comparer;

            public OrderedDictionaryKeyValueCollection(ArrayList array, Hashtable objectsTable, IEqualityComparer? comparer)
            {
                _objects = array;
                _objectsTable = objectsTable;
                _comparer = comparer;
            }

            public OrderedDictionaryKeyValueCollection(ArrayList array)
            {
                _objects = array;
            }

            private bool IsKeys => _objectsTable is not null;

            void ICollection.CopyTo(Array array, int index)
            {
                ArgumentNullException.ThrowIfNull(array);

                ArgumentOutOfRangeException.ThrowIfNegative(index);
                foreach (object? o in _objects)
                {
                    Debug.Assert(o != null);
                    array.SetValue(IsKeys ? ((DictionaryEntry)o).Key : ((DictionaryEntry)o).Value, index);
                    index++;
                }
            }

            int ICollection.Count => _objects.Count;

            bool ICollection.IsSynchronized => false;

            object ICollection.SyncRoot => _objects.SyncRoot;

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new OrderedDictionaryEnumerator(_objects, IsKeys ? OrderedDictionaryEnumerator.Keys : OrderedDictionaryEnumerator.Values);
            }

            bool IList.Contains(object? value)
            {
                if (IsKeys)
                {
                    Debug.Assert(_objectsTable is not null);
                    return value != null && _objectsTable.ContainsKey(value);
                }

                foreach (object? o in _objects)
                {
                    Debug.Assert(o != null);
                    if (object.Equals(((DictionaryEntry)o).Value, value))
                    {
                        return true;
                    }
                }

                return false;
            }

            int IList.IndexOf(object? value)
            {
                for (int i = 0; i < _objects.Count; i++)
                {
                    if (IsKeys)
                    {
                        object entryKey = ((DictionaryEntry)_objects[i]!).Key;
                        if (_comparer != null)
                        {
                            if (_comparer.Equals(entryKey, value))
                            {
                                return i;
                            }
                        }
                        else if (entryKey.Equals(value))
                        {
                            return i;
                        }
                    }
                    else if (object.Equals(((DictionaryEntry)_objects[i]!).Value, value))
                    {
                        return i;
                    }
                }

                return -1;
            }

            bool IList.IsFixedSize => true;
            bool IList.IsReadOnly => true;

            object? IList.this[int index]
            {
                get
                {
                    DictionaryEntry entry = (DictionaryEntry)_objects[index]!;
                    return IsKeys ? entry.Key : entry.Value;
                }
                set => throw new NotSupportedException(GetNotSupportedErrorMessage());
            }

            void IList.Insert(int index, object? value)
            {
                throw new NotSupportedException(GetNotSupportedErrorMessage());
            }

            void IList.Remove(object? value)
            {
                throw new NotSupportedException(GetNotSupportedErrorMessage());
            }

            void IList.RemoveAt(int index)
            {
                throw new NotSupportedException(GetNotSupportedErrorMessage());
            }

            int IList.Add(object? value)
            {
                throw new NotSupportedException(GetNotSupportedErrorMessage());
            }

            void IList.Clear()
            {
                throw new NotSupportedException(GetNotSupportedErrorMessage());
            }

            private string GetNotSupportedErrorMessage()
            {
                return IsKeys ? SR.NotSupported_KeyCollectionSet : SR.NotSupported_ValueCollectionSet;
            }
        }
    }
}
