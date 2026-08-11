using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using MyWeb.Portal.Configuration;
using MyWeb.Portal.Data;

namespace MyWeb.Portal.Pages.Account;

[Authorize]
public sealed class Setup2faModel(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IOptions<MyWebOptions> myWebOptions) : PageModel
{
    [BindProperty]
    public VerificationInput Input { get; set; } = new();

    public string SharedKey { get; private set; } = string.Empty;

    public string AuthenticatorUri { get; private set; } = string.Empty;

    public bool IsEnabled { get; private set; }

    public IReadOnlyList<string> RecoveryCodes { get; private set; } = [];

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        IsEnabled = await userManager.GetTwoFactorEnabledAsync(user);
        if (!IsEnabled)
        {
            await LoadSharedKeyAsync(user);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await userManager.GetUserAsync(User);
        if (user is null)
        {
            return Challenge();
        }

        if (!ModelState.IsValid)
        {
            await LoadSharedKeyAsync(user);
            return Page();
        }

        var code = Input.Code.Replace(" ", string.Empty, StringComparison.Ordinal)
            .Replace("-", string.Empty, StringComparison.Ordinal);
        var valid = await userManager.VerifyTwoFactorTokenAsync(
            user,
            userManager.Options.Tokens.AuthenticatorTokenProvider,
            code);
        if (!valid)
        {
            ModelState.AddModelError(string.Empty, "인증 코드가 올바르지 않습니다.");
            await LoadSharedKeyAsync(user);
            return Page();
        }

        await userManager.SetTwoFactorEnabledAsync(user, true);
        var codes = await userManager.GenerateNewTwoFactorRecoveryCodesAsync(user, 10);
        RecoveryCodes = codes?.ToArray() ?? [];
        await signInManager.SignOutAsync();
        IsEnabled = false;
        return Page();
    }

    private async Task LoadSharedKeyAsync(ApplicationUser user)
    {
        var key = await userManager.GetAuthenticatorKeyAsync(user);
        if (string.IsNullOrEmpty(key))
        {
            await userManager.ResetAuthenticatorKeyAsync(user);
            key = await userManager.GetAuthenticatorKeyAsync(user);
        }

        SharedKey = FormatKey(key ?? throw new InvalidOperationException("Authenticator key was not generated."));
        var issuer = Uri.EscapeDataString("My Web");
        var account = Uri.EscapeDataString(user.Email ?? myWebOptions.Value.OwnerEmail);
        AuthenticatorUri = string.Format(
            CultureInfo.InvariantCulture,
            "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6",
            issuer,
            account,
            key);
    }

    private static string FormatKey(string unformattedKey)
    {
        var result = new StringBuilder();
        for (var index = 0; index < unformattedKey.Length; index += 4)
        {
            if (result.Length > 0)
            {
                result.Append(' ');
            }

            result.Append(unformattedKey.AsSpan(index, Math.Min(4, unformattedKey.Length - index)).ToString().ToLowerInvariant());
        }

        return result.ToString();
    }

    public sealed class VerificationInput
    {
        [Required, StringLength(12, MinimumLength = 6)]
        public string Code { get; set; } = string.Empty;
    }
}
