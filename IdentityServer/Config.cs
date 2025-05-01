using Duende.IdentityServer.Models;

namespace DuendeProfileServiceAspNetCoreIdentity;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope("scope1")
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            // interactive client using code flow + pkce
            new Client
            {
                ClientId = "my-client",
                ClientSecrets = { new Secret("44C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

                AllowedGrantTypes = GrantTypes.Code,
                RequirePushedAuthorization = true,

                RedirectUris = { "https://localhost:5015/signin-oidc" },
                FrontChannelLogoutUri = "https://localhost:5015/signout-oidc",
                PostLogoutRedirectUris = { "https://localhost:5015/signout-callback-oidc" },

                AllowOfflineAccess = true,
                AllowedScopes = { "openid", "profile", "scope1" }
            },
        };
}
