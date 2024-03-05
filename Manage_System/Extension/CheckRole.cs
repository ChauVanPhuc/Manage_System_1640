using AspNetCoreHero.ToastNotification.Abstractions;
using Manage_System.models;
using Manage_System.Service;
using Microsoft.EntityFrameworkCore;

namespace Manage_System.Extension
{
    public static class CheckRole
    {

        private static readonly ManageSystem1640Context _db;

        public static string CheckRoleLogin(int accountId)
        {
            string role = "";

            User account = _db.Users.FirstOrDefault(x => x.Id== accountId);

            if (account == null)
            {
                return "False";
            }

            if (account.Role.Name.Equals("Guest"))
            {
                return role = "Guest";
            }
            else if(account.Role.Name.Equals("Student"))
            {
                return role = "Student";
            }
            else if (account.Role.Name.Equals("Coordinator"))
            {
                return role = "Coordinator";
            }
            else if (account.Role.Name.Equals("Maketting"))
            {
                return role = "Maketting";
            }
            else
            {
                return role = "Admmin";
            }
        }
    }
}
