namespace LoginApi.Models
{
    public class Response
    {
        public string Stutscode { get; set; }
        public string Message { get; set; }
        public bool IsAuthenticated { get; set; }
        public string Email{ get; set; }
        public string Token{ get; set; }
        public DateTime Expireson{ get; set; }
        public List<string> Role { get; set; }
    }
    public class TokenRequestModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class AddRoleModel
    {
        public string UserId { get; set; }
        public string Role { get; set; }
    }
    public class AuthenticatedResponse
    {
        public string Email { get; set; }
        public string? Token { get; set; }
        public string Message { get; internal set; }
    }
}
