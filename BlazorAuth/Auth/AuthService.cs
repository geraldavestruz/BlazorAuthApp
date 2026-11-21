using BlazorAuth.Auth.Interfaces;
using BlazorAuth.ViewModels;

namespace BlazorAuth.Auth
{
    public class AuthService(
        HttpClient http,
        CustomAuthenticationStateProvider AuthStateProvider) : IAuthService
    {
        public async Task LoginUserAsync(LoginViewModel model)
        {
            var response = await http.PostAsJsonAsync($"/account/user/login", model);

            if(response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResult>();
                await AuthStateProvider.Login(result!.Username, result.Role);
            }
        }
    }

    public class LoginResult
    {
        public string Username { get; set; }
        public int Role { get; set; }
    }
}
