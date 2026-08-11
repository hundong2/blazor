using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyWeb.Portal.Apps;
using MyWeb.Portal.Data;

namespace MyWeb.Portal.Pages.Admin.Apps;

[Authorize(Policy = "OwnerOnly")]
public sealed class IndexModel(AppRegistryService registry) : PageModel
{
    public IReadOnlyList<AppDefinition> Applications { get; private set; } = [];

    public async Task OnGetAsync()
    {
        Applications = await registry.ListAsync();
    }
}
