using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;

namespace UI.Pages;

[Authorize]
public class IndexModel : PageModel
{
    public async Task OnGetAsync()
    {
        var token = await HttpContext.GetTokenAsync("id_token");
        var claims = User.Claims;
    }
}
