using System;
using System.Web.Mvc;

namespace Optel2.Utils
{
    public class AuthorizeRolesAttribute : AuthorizeAttribute
    {
        public AuthorizeRolesAttribute()
        {
            Roles = String.Join(",", User.Roles.GetAllRoles());
        }

        public AuthorizeRolesAttribute(params string[] roles)
        {
            Roles = String.Join(",", roles);
        }
    }
}