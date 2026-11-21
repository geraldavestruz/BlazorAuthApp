namespace BlazorAuth.Constants
{
    public static class RoleConstant
    {
        public static readonly Guid AdminRoleId= Guid.Parse("33567833-3563-4345-9f05-e391e1b009b3");
        public static readonly Guid MemberRoleId = Guid.Parse("5c61a549-6b5d-4352-9049-bc897ce97b77");

        public const string Admin = "Admin";
        public const string Member = "Member";
    }
}
