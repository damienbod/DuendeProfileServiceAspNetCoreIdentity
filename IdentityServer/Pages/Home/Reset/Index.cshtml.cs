using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DuendeProfileServiceAspNetCoreIdentity.Pages.Reset;

[AllowAnonymous]
[SecurityHeaders]
public class Index : PageModel
{
    private readonly IIdentityServerInteractionService _interaction;
    private readonly IWebHostEnvironment _environment;

    public ViewModel View { get; set; } = new();

    public Index(IIdentityServerInteractionService interaction, IWebHostEnvironment environment)
    {
        _interaction = interaction;
        _environment = environment;
    }

    public async Task OnGet(string? errorId, string? remoteError)
    {
        if (remoteError != null)
        {
            View.Error = new ErrorMessage
            {
                Error = "Remote authentication error",
                ErrorDescription = remoteError
            };
        }
        else
        {
            // retrieve error details from identity server
            var message = await _interaction.GetErrorContextAsync(errorId);

            if (message != null)
            {
                View.Error = message;

                if (!_environment.IsDevelopment())
                {
                    // only show in development
                    message.ErrorDescription = null;
                }
            }
        }
    }

    public IActionResult OnPost()
    {
        // clear cache if needed
        foreach (var cookie in Request.Cookies.Keys)
        {
            Response.Cookies.Delete(cookie);
        }

        // bubble up to a UI application if required
        return Redirect("https://localhost:5015/resetcache");
    }
}
