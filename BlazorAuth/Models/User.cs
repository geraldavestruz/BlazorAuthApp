using Microsoft.AspNetCore.Identity;

namespace BlazorAuth.Models
{
    public class User : IdentityUser<Guid>
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
    }
}
