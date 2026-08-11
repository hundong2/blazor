using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyWeb.Portal.Apps;

namespace MyWeb.Portal.Pages.Admin.Apps;

[Authorize(Policy = "OwnerOnly")]
public sealed class EditModel(AppRegistryService registry) : PageModel
{
    [BindProperty]
    public Guid Id { get; set; }

    [BindProperty]
    public AppDefinitionInput Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(Guid id)
    {
        var app = await registry.FindAsync(id);
        if (app is null)
        {
            return NotFound();
        }

        Id = app.Id;
        Input = new AppDefinitionInput
        {
            Name = app.Name,
            Slug = app.Slug,
            InternalUrl = app.InternalUrl,
            Category = app.Category,
            Icon = app.Icon,
            HealthPath = app.HealthPath,
            Enabled = app.Enabled,
            SortOrder = app.SortOrder
        };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await registry.UpdateAsync(Id, Input);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "서비스를 수정하지 못했습니다.");
            return Page();
        }

        TempData["Message"] = $"'{Input.Name}' 서비스를 수정했습니다.";
        return RedirectToPage("Index");
    }
}
