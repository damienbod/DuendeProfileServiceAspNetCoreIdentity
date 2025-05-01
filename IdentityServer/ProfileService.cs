using System.Security.Claims;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;

namespace DuendeProfileServiceAspNetCoreIdentity;

public class ProfileService: IProfileService
{
	public async Task GetProfileDataAsync(ProfileDataRequestContext context)
	{
        // context.Subject is the user for whom the result is being made
        // context.Subject.Claims is the claims collection from the user's session cookie at login time
        // context.IssuedClaims is the collection of claims that your logic has decided to return in the response

        if (context.Caller == "ClaimsProviderAccessToken")
        {
            // AT
        }

        // Add userinfo endpoint claims
        if (context.Caller == "UserInfoEndpoint")
        {
			context.IssuedClaims.Add(new Claim("UserInfo", "userinfo"));
        }

        // ALL
        context.IssuedClaims.Add(new Claim("test", "A"));

        var idpClaim = context.Subject.Claims.FirstOrDefault(c => c.Type == "idp");
		if (idpClaim?.Value == "EntraID")
		{

		}

		return;
	}

	public Task IsActiveAsync(IsActiveContext context)
	{
		context.IsActive = true;
		return Task.CompletedTask;
	}
}