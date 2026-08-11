using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyWeb.Portal.Data;

namespace MyWeb.Portal.Pages.Account;

[AllowAnonymous]
public sealed class LoginWith2faModel(SignInManager<ApplicationUser> signInManager) : PageModel
{
    [BindProperty]
    public TwoFactorInput Input { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool RememberMe { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        return await signInManager.GetTwoFactorAuthenticationUserAsync() is null
            ? RedirectToPage("/Account/Login")
            : Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (await signInManager.GetTwoFactorAuthenticationUserAsync() is null)
        {
            return RedirectToPage("/Account/Login");
        }

        var code = Input.Code.Replace(" ", string.Empty, StringComparison.Ordinal)
            .Replace("-", string.Empty, StringComparison.Ordinal);
        var result = await signInManager.TwoFactorAuthenticatorSignInAsync(
            code,
            RememberMe,
            Input.RememberMachine);

        if (result.Succeeded)
        {
            return LocalRedirect(string.IsNullOrWhiteSpace(ReturnUrl) ? "/" : ReturnUrl);
        }

        if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty, "로그인 시도가 잠시 제한되었습니다.");
            return Page();
        }

        ModelState.AddModelError(string.Empty, "인증 코드가 올바르지 않습니다.");
        return Page();
    }

    public sealed class TwoFactorInput
    {
        [Required, StringLength(12, MinimumLength = 6)]
        public string Code { get; set; } = string.Empty;

        public bool RememberMachine { get; set; }
    }
}
