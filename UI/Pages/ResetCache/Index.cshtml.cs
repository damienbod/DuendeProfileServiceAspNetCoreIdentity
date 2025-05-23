using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace UI.Pages.ResetCache;

[AllowAnonymous]
public class Index : PageModel
{
    public async Task OnGet(string? errorId)
    {
        // clear cache if needed
        foreach (var cookie in Request.Cookies.Keys)
        {
            Response.Cookies.Delete(cookie);
        }
    }
}
