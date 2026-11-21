using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using System.Security.Claims;

namespace BlazorAuth.Auth
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private HttpClient _http;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ProtectedSessionStorage _sessionStorage;
        private ClaimsPrincipal _Anonymous = new ClaimsPrincipal(new ClaimsIdentity());

        public CustomAuthenticationStateProvider(
            IHttpContextAccessor httpContextAccessor,
            ProtectedSessionStorage sessionStorage,
            HttpClient http)
        {
            _httpContextAccessor = httpContextAccessor;
            _sessionStorage = sessionStorage;
            _http = http;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var userSessionResult = await _sessionStorage.GetAsync<UserSession>("UserSession");
            var userSession = userSessionResult.Success ? userSessionResult.Value : null;

            var user = httpContext?.User;   

            if(user?.Identity != null && user.Identity.IsAuthenticated)
            {
                return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(user)));
            }

            return await Task.FromResult(new AuthenticationState(_Anonymous));
        }  
    }

    public class UserSession
    {
        public string UserName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
