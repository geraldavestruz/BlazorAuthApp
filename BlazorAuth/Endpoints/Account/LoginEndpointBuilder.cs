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
            user.MapPost("/verify", VerifyCredentials);
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

            await userManager.GetRolesAsync(user);

            return Results.Ok(result);
        }

        public static async Task<IResult> VerifyCredentials(
            [FromBody] LoginRequest verifyRequest,
            UserManager<User> userManager)
        {
            var user = await userManager.FindByNameAsync(verifyRequest.Username);

            if (user == null || !await userManager.CheckPasswordAsync(user, verifyRequest.Password))
                return Results.Unauthorized();

            return Results.Ok();
        }
    }

    public record LoginRequest(string Username, string Password);
}
