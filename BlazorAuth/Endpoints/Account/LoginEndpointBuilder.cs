using BlazorAuth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlazorAuth.Endpoints.Account
{
    public static class LoginEndpointBuilder
    {
        public static void MapLoginEndpoint(this IEndpointRouteBuilder app)
        {
            var user = app.MapGroup("/account/user");
            user.MapPost("/login", LoginUser);
        }

        public static async Task<IResult> LoginUser(
            [FromBody] LoginRequest request,
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            var user = await userManager.FindByNameAsync(request.Username);

            if (user is null)
                return Results.Unauthorized();

            var result = await signInManager
                .CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);

            if (!result.Succeeded)
                return Results.Unauthorized();

            var roles = await userManager.GetRolesAsync(user);

            return Results.Ok();
        }
    }

    public record LoginRequest(string Username, string Password);
}
