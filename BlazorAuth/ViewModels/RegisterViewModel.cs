using BlazorAuth.Enums;

namespace BlazorAuth.ViewModels
{
    public class RegisterViewModel
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Password { get; set; }
        public Role Role { get; set; }
    }
}
