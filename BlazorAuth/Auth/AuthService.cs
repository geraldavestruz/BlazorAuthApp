namespace BlazorAuth.Auth
{
    public class AuthService(HttpClient http)
    {
        public async Task<bool> LoginAsync(CredsRequest request)
        {
            var response = await http.PostAsJsonAsync("/account/user/login", request);

            if (!response.IsSuccessStatusCode)
                return false;

            return true;
        }

        public async Task<bool> IsUserVerified(CredsRequest request)
        {
            var response = await http.PostAsJsonAsync("/account/user/verify", request);

            if (!response.IsSuccessStatusCode)
                return false;

            return true;
        }
    }

    public class CredsRequest
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        public string token { get; set; }
    }
}
