using Duende.IdentityServer.Models;

namespace DuendeProfileServiceAspNetCoreIdentity.Pages.Reset;

public class ViewModel
{
    public ViewModel()
    {
    }

    public ViewModel(string error)
    {
        Error = new ErrorMessage { Error = error };
    }

    public ErrorMessage? Error { get; set; }
}
