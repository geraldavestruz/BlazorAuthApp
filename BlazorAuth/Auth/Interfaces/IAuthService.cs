using BlazorAuth.ViewModels;

namespace BlazorAuth.Auth.Interfaces
{
    public interface IAuthService
    {
        Task LoginUserAsync(LoginViewModel model);
    }
}
