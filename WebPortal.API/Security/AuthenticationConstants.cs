namespace WebPortal.API.Security
{
    public static class AuthenticationConstants
    {
        public const string AuthenticationType = "OpaqueToken";
        public const string BankIdClaimType = "BankId";
        public const string BankNameClaimType = "BankName";
        public const int SessionTimeoutMinutes = 15;
    }
}