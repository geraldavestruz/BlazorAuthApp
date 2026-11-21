using Microsoft.AspNetCore.Identity;

namespace BlazorAuth.Models
{
    public class User : IdentityUser<Guid>
    {
        
        public string Firstname { get; set; }
        public string Lastname { get; set; }


        public static User Create(string firstname, string lastname)
        {
            return new User
            {
                UserName = $"{firstname}{lastname}",
                Firstname = firstname,
                Lastname = lastname
            };
        }
    }
}
