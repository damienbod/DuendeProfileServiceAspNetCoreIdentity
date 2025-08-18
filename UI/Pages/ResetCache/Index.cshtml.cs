using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace UI.Pages.ResetCache;

[AllowAnonymous]
public class Index : PageModel
{
    public IActionResult OnGet()
    {
        // clear the cookie cache
        foreach (var cookie in Request.Cookies.Keys)
        {
            Response.Cookies.Delete(cookie);
        }

        // Force a sign-in
        return Redirect("/");
    }
}
