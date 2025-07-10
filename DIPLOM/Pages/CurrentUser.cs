// CurrentUser.cs
using DIPLOM.Connection;

namespace DIPLOM
{
    public static class CurrentUser
    {
        public static int UserId { get; set; }
        public static string FirstName { get; set; }
        public static string LastName { get; set; }
        public static string Email { get; set; }
        public static int RoleId { get; set; }

        public static void SetUser(Users user)
        {
            UserId = user.UserId;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            RoleId = user.RoleId;
        }

        public static void Clear()
        {
            UserId = 0;
            FirstName = string.Empty;
            LastName = string.Empty;
            Email = string.Empty;
            RoleId = 0;
        }
    }
}