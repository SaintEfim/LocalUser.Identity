namespace Users.Domain.Entity
{
    public class LoginResponse
    {
        public LocalUser User { get; set; }
        public string Token { get; set; }
    }
}
