using System.Security.Claims;
using System.Threading.Tasks;
using DataInspector.SharedComponents.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace DataInspector.SharedComponents.Services
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private ClaimsPrincipal _currentUser = new ClaimsPrincipal(new ClaimsIdentity());

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            return Task.FromResult(new AuthenticationState(_currentUser));
        }

        public Task<bool> LoginAsync(UserAccount userAccount)
        {
            // This is a mock login. In a real app, you'd validate against a database or identity service.
            if (string.IsNullOrWhiteSpace(userAccount.UserName) || string.IsNullOrWhiteSpace(userAccount.Password) || string.IsNullOrWhiteSpace(userAccount.Otp))
            {
                return Task.FromResult(false);
            }

            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, userAccount.UserName),
                // Add other claims as needed
            }, "Custom Authentication");

            _currentUser = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));

            return Task.FromResult(true);
        }

        public void Logout()
        {
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
        }
    }
}
