using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyWeb.Portal.Apps;

namespace MyWeb.Portal.Pages.Admin.Apps;

[Authorize(Policy = "OwnerOnly")]
public sealed class DeleteModel(AppRegistryService registry) : PageModel
{
    [BindProperty]
    public Guid Id { get; set; }

    public string Name { get; private set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var app = await registry.FindAsync(id);
        if (app is null)
        {
            return NotFound();
        }

        Id = app.Id;
        Name = app.Name;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!await registry.DeleteAsync(Id))
        {
            return NotFound();
        }

        TempData["Message"] = "서비스를 삭제했습니다.";
        return RedirectToPage("Index");
    }
}
