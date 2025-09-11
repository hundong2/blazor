using System.Security.Claims;
using System.Threading.Tasks;
using DataInspector.SharedComponents.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System;
using System.Text.Json;

namespace DataInspector.SharedComponents.Services
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly IJSRuntime _jsRuntime;
        private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        private ClaimsPrincipal _currentUser = new ClaimsPrincipal(new ClaimsIdentity());

        public CustomAuthenticationStateProvider(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            try
            {
                var userSessionJson = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "userSession");
                if (string.IsNullOrWhiteSpace(userSessionJson))
                {
                    return new AuthenticationState(_anonymous);
                }

                var userSession = JsonSerializer.Deserialize<UserSession>(userSessionJson);
                var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, userSession.UserName)
                }, "CustomAuth"));

                return new AuthenticationState(claimsPrincipal);
            }
            catch
            {
                return new AuthenticationState(_anonymous);
            }
        }

        public async Task<bool> LoginAsync(UserAccount userAccount)
        {
            // Hardcoded user for demonstration
            if (userAccount.UserName == "admin" && userAccount.Password == "password")
            {
                // Placeholder for OTP verification
                bool isOtpValid = await VerifyOtpAsync(userAccount.Otp);
                if (isOtpValid)
                {
                    var userSession = new UserSession { UserName = userAccount.UserName };
                    var userSessionJson = JsonSerializer.Serialize(userSession);
                    await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "userSession", userSessionJson);

                    var identity = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, userAccount.UserName)
                    }, "CustomAuth");

                    _currentUser = new ClaimsPrincipal(identity);

                    NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
                    return true;
                }
            }
            return false;
        }

        public async Task LogoutAsync()
        {
            await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", "userSession");
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
        }

        private Task<bool> VerifyOtpAsync(string otp)
        {
            // This is the placeholder function for OTP verification.
            // In a real application, this would involve a call to a service.
            // For now, we'll consider any non-empty OTP as valid for demonstration.
            Console.WriteLine($"OTP received: {otp}. Verification result: {!string.IsNullOrWhiteSpace(otp)}");
            return Task.FromResult(!string.IsNullOrWhiteSpace(otp));
        }
    }

    public class UserSession
    {
        public string? UserName { get; set; }
    }
}
