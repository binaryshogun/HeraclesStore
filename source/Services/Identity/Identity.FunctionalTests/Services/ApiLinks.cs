namespace Identity.FunctionalTests.Services
{
    public class ApiLinks
    {
        public static string AuthController => "api/auth";
        public static string SignUp => $"{AuthController}/signup";
        public static string SignIn => $"{AuthController}/signin";
    }
}