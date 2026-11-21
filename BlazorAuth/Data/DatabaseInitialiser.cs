using BlazorAuth.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BlazorAuth.Data
{
    public class DatabaseInitialiser
    {
        public static async Task SeedRole(DbContext context)
        {
            if(!context.Set<IdentityRole<Guid>>().Any())
            {
                var roles = new List<IdentityRole<Guid>>
                {
                    new IdentityRole<Guid>()
                    {
                        Id = RoleConstant.AdminRoleId,
                        Name = RoleConstant.Admin,
                        NormalizedName =  RoleConstant.Admin.ToUpper()
                    },
                    new IdentityRole<Guid>()
                    {
                        Id = RoleConstant.MemberRoleId,
                        Name = RoleConstant.Member,
                        NormalizedName = RoleConstant.Member.ToUpper()
                    }
                };

                context.Set<IdentityRole<Guid>>().AddRange(roles);
                context.SaveChanges();
            }
        }
    }
}
