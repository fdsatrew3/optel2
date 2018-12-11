namespace Optel2.Utils
{
    public static class User
    {
        public static class Roles
        {
            public const string Admin = "Admin";
            public const string Operator = "Operator";

            public static string[] GetAllRoles()
            {
                string[] roles = new string[] { Admin, Operator };
                return roles;
            }
        }
    }
}