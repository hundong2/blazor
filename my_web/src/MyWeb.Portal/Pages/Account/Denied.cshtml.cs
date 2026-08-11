using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MyWeb.Portal.Pages.Account;

[AllowAnonymous]
public sealed class DeniedModel : PageModel
{
    public void OnGet()
    {
    }
}
