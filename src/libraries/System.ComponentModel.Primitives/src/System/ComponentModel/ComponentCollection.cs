// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Collections.Generic;

namespace System.ComponentModel
{
    public class ComponentCollection : ReadOnlyCollectionBase
    {
        public ComponentCollection(IComponent[] components) => InnerList.AddRange(components);

        /// <summary>
        /// Gets a specific <see cref='System.ComponentModel.Component'/> in the
        /// <see cref='System.ComponentModel.IContainer'/>.
        /// </summary>
        public virtual IComponent? this[string? name]
        {
            [CollectionAccess(CollectionAccessType.Read)]
            get
            {
                if (name != null)
                {
                    IList list = InnerList;
                    foreach (IComponent? comp in list)
                    {
                        if (comp != null && comp.Site != null && comp.Site.Name != null && string.Equals(comp.Site.Name, name, StringComparison.OrdinalIgnoreCase))
                        {
                            return comp;
                        }
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Gets a specific <see cref='System.ComponentModel.Component'/> in the
        /// <see cref='System.ComponentModel.IContainer'/>.
        /// </summary>
        public virtual IComponent? this[int index]
        {
            [CollectionAccess(CollectionAccessType.Read)]
            get => (IComponent?)InnerList[index];
        }

        [CollectionAccess(CollectionAccessType.Read)]
        public void CopyTo(IComponent[] array, int index) => InnerList.CopyTo(array, index);
    }
}
