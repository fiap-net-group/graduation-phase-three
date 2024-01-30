namespace TechBlog.NewsManager.API.Domain.Authentication
{
    public class AccessTokenModel
    {
        public string TokenType { get; set; }
        public string AccessToken { get; set; }
        public DateTime Expires { get; set; }
        public string UserId { get; set; }

        public bool Valid =>
                   !string.IsNullOrWhiteSpace(TokenType) &&
                   !string.IsNullOrWhiteSpace(AccessToken) &&
                   Expires != DateTime.MinValue &&
                   !string.IsNullOrWhiteSpace(UserId);

        public AccessTokenModel()
        {
            TokenType = string.Empty;
            AccessToken = string.Empty;
            Expires = DateTime.MinValue;
            UserId = string.Empty;
        }

        public AccessTokenModel(string tokenType, string accessToken, DateTime expires, string userId)
        {
            TokenType = tokenType;
            AccessToken = accessToken;
            Expires = expires;
            UserId = userId;
        }
    }
}
