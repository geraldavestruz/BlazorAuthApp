using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace BlazorAuth.Auth
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomAuthenticationStateProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var user = httpContext?.User;

            if(user?.Identity != null && user.Identity.IsAuthenticated)
            {
                return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(user)));
            }

            return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())));
        }
    }
}
