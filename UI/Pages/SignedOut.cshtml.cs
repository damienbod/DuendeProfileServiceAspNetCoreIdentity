using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace UI.Pages;

[AllowAnonymous]
public class SignedOutModel : PageModel
{
    public void OnGet()
    {
    }
}