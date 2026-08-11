using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MyWeb.Portal.Apps;

namespace MyWeb.Portal.Pages.Admin.Apps;

[Authorize(Policy = "OwnerOnly")]
public sealed class CreateModel(AppRegistryService registry) : PageModel
{
    [BindProperty]
    public AppDefinitionInput Input { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await registry.CreateAsync(Input);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "서비스를 저장하지 못했습니다.");
            return Page();
        }

        TempData["Message"] = $"'{Input.Name}' 서비스를 추가했습니다.";
        return RedirectToPage("Index");
    }
}
