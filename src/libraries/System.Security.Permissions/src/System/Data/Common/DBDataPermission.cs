// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Security;
using System.Security.Permissions;

namespace System.Data.Common
{
#if NET
    [Obsolete(Obsoletions.CodeAccessSecurityMessage, DiagnosticId = Obsoletions.CodeAccessSecurityDiagId, UrlFormat = Obsoletions.SharedUrlFormat)]
#endif
    public abstract class DBDataPermission : CodeAccessPermission, IUnrestrictedPermission
    {
        protected DBDataPermission() { }
        protected DBDataPermission(DBDataPermission permission) { }
        protected DBDataPermission(DBDataPermissionAttribute permissionAttribute) { }
        protected DBDataPermission(PermissionState state) { }
        protected DBDataPermission(PermissionState state, bool allowBlankPassword) { }
        public bool AllowBlankPassword { get; set; }
        public virtual void Add(string connectionString, string restrictions, KeyRestrictionBehavior behavior) { }
        protected void Clear() { }
        protected virtual DBDataPermission CreateInstance() => null;
        public override IPermission Copy() => null;
        public override void FromXml(SecurityElement securityElement) { }
        public override IPermission Intersect(IPermission target) => null;
        public override bool IsSubsetOf(IPermission target) => false;
        public bool IsUnrestricted() => false;
        public override SecurityElement ToXml() => null;
        public override IPermission Union(IPermission target) { return default(IPermission); }
    }
}
