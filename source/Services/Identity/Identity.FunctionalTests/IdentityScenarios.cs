namespace Identity.FunctionalTests
{
    public class IdentityScenarios
    {
        [Fact]
        public async Task SignUpUser_ResponseStatusCodeShouldBeSuccess()
        {
            // Given
            using var client = new IdentityWebHostFactory().CreateClient();

            // When
            var response = await SignUpUser(client, "newuser@example.com", "NewUser", "SecretPassword123");

            // Then
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task SignUpUser_BadRequestData_ResponseStatusCodeShouldBeBadRequest()
        {
            // Given
            using var client = new IdentityWebHostFactory().CreateClient();

            // When
            var response = await SignUpUser(client, "wrongemail", "Wrong User", "WrOnG123");

            // Then
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task SignInUser_ResponseStatusCodeShouldBeSuccess()
        {
            // Given
            using var client = new IdentityWebHostFactory().CreateClient();

            await SignUpUser(client, "NewUser123", "newuser123@example.com", "Password123");

            // When
            var response = await SignInUser(client, "newuser123@example.com", "Password123");

            // Then
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task SignInUser_WrongCredentials_ResponseStatusCodeShouldBeUnauthorized()
        {
            // Given
            using var client = new IdentityWebHostFactory().CreateClient();

            // When
            var response = await SignInUser(client, "wrongemail@example.com", "WrOnG123");

            // Then
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task SignInUser_BadRequestData_ResponseStatusCodeShouldBeBadRequest()
        {
            // Given
            using var client = new IdentityWebHostFactory().CreateClient();

            // When
            var response = await SignInUser(client, "bademail", "badpassword");

            // Then
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        private async Task<HttpResponseMessage> SignUpUser(HttpClient client, string email, string username, string password)
        {
            var request = JsonContent.Create(new SignUpRequest()
            {
                Email = email,
                Username = username,
                Password = password
            });

            // When
            return await client.PostAsync(ApiLinks.SignUp, request);
        }

        private async Task<HttpResponseMessage> SignInUser(HttpClient client, string email, string password)
        {
            var request = JsonContent.Create(new SignInRequest()
            {
                Email = email,
                Password = password
            });

            return await client.PostAsync(ApiLinks.SignIn, request);
        }
    }
}