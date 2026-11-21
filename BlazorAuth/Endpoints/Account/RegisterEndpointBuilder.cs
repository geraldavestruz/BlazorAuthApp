using BlazorAuth.Constants;
using BlazorAuth.Enums;
using BlazorAuth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlazorAuth.Endpoints.Account
{
    public static class RegisterEndpointBuilder
    {
        public static void MapRegisterEndpoint(this IEndpointRouteBuilder app)
        {
            var user = app.MapGroup("/account/user/");
            user.MapPost("/register", RegisterUser);
        }
        public static async Task<IResult> RegisterUser(
            [FromBody] RegisterUserRequest request,
            UserManager<User> userManager)
        {
            var user = User.Create(request.Firstname, request.Lastname);
            user.PasswordHash = userManager.PasswordHasher.HashPassword(user, request.Password);

            var result = await userManager.CreateAsync(user);

            await userManager.AddToRoleAsync(user, GetIdentityRoleName(request.Role));

            return result.Succeeded
                ? Results.Ok("User Created Susccesfully.")
                : Results.BadRequest($"Error {result.Errors.Select(err => err.Description)}");
        }

        private static string GetIdentityRoleName(Role role)
        {
            return role switch
            {
                Role.Admin => RoleConstant.Admin,
                Role.Member => RoleConstant.Member,
                _ => throw new ArgumentOutOfRangeException(nameof(role), role, "Role provided is not supported.")
            };
        }

    }

    public class RegisterUserRequest
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Password { get; set; }
        public Role Role { get; set; }
    }
}
