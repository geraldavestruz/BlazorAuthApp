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
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var userSessionResult = await _sessionStorage.GetAsync<UserSession>("UserSession");
                var userSession = userSessionResult.Success ? userSessionResult.Value : null;

                var user = httpContext?.User;

                if (userSession == null)
                    return new AuthenticationState(_Anonymous);

                var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userSession.UserName),
                    new Claim(ClaimTypes.Name, userSession.Role)
                }, "CustomAuth"));

                return new AuthenticationState(new ClaimsPrincipal(user!));
            }
            catch
            {
                return await Task.FromResult(new AuthenticationState(_Anonymous));
            }           
        }  


        public async Task UpdateAuthenticationState(UserSession? userSession)
        {
            ClaimsPrincipal claimsPrincipal;

            if(userSession != null)
            {
                await _sessionStorage.SetAsync("UserSession", userSession);

                claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userSession.UserName),
                    new Claim(ClaimTypes.Role, userSession.Role.ToString())
                }, "CustomAuth"));
            }
            else
            {
                await _sessionStorage.DeleteAsync("UserSession");
                claimsPrincipal = _Anonymous;
            }

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }

        public async Task Login(string username, int role)
        {
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new(ClaimTypes.Name, username),
                new(ClaimTypes.Role, role.ToString())
            }, "CustomAuth"));

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
        }
    }

    public class UserSession
    {
        public string UserName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
