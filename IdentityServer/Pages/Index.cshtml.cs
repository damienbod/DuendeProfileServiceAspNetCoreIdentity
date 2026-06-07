using Duende.IdentityServer;
using Duende.IdentityServer.Licensing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Reflection;

namespace DuendeProfileServiceAspNetCoreIdentity.Pages.Home;

[AllowAnonymous]
public class Index : PageModel
{
    private readonly LicenseInformation _license;

    public Index(LicenseInformation license)
    {
        _license = license;
    }

    public void OnGet()
    {
        if (_license.IsConfigured)
        {
            // licensed behavior
        }
    }
}
