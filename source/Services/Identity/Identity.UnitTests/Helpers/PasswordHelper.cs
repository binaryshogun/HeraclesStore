namespace Identity.UnitTests.Helpers
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            return SHA256.HashData(Encoding.UTF8.GetBytes(password)).ToString() ?? "";
        }
    }
}