namespace WebPortal.API.Models
{
    public sealed class AccessTokenModel
    {
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public int IdleTimeoutSeconds { get; set; }
    }
}