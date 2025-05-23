using Duende.IdentityServer;
using DuendeProfileServiceAspNetCoreIdentity.Data;
using DuendeProfileServiceAspNetCoreIdentity.Models;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Serilog;

namespace DuendeProfileServiceAspNetCoreIdentity;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddRazorPages();

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        builder.Services
            .AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
            .AddInMemoryIdentityResources(Config.IdentityResources)
            .AddInMemoryApiScopes(Config.ApiScopes)
            .AddInMemoryClients(Config.Clients)
            .AddAspNetIdentity<ApplicationUser>()
            .AddLicenseSummary()
            .AddProfileService<ProfileService>();

        builder.Services.AddDistributedMemoryCache();

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
            options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
            options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
        })
        .AddOpenIdConnect("Auth0Scheme", "Auth0", options =>
        {
            options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
            options.SignOutScheme = IdentityConstants.ApplicationScheme;
            options.CallbackPath = new PathString("/signin-oidc-auth0");
            options.RemoteSignOutPath = new PathString("/signout-callback-oidc-auth0");
            options.SignedOutCallbackPath = new PathString("/signout-oidc-auth0");

            options.Authority = $"https://{builder.Configuration["Auth0:Domain"]}";
            options.ClientId = builder.Configuration["Auth0:ClientId"];
            options.ClientSecret = builder.Configuration["Auth0:ClientSecret"];
            options.ResponseType = OpenIdConnectResponseType.Code;
            options.Scope.Clear();
            options.Scope.Add("openid");
            options.Scope.Add("profile");
            options.Scope.Add("email");
            options.Scope.Add("auth0-user-api-one");
                
            options.ClaimsIssuer = "Auth0";
            options.SaveTokens = true;
            options.UsePkce = true;
            options.GetClaimsFromUserInfoEndpoint = true;
            options.TokenValidationParameters.NameClaimType = "name";
            options.Events = new OpenIdConnectEvents
            {
                OnTokenResponseReceived = context =>
                {
                    var idToken = context.TokenEndpointResponse.IdToken;
                    return Task.CompletedTask;
                }
            };

            options.Events = new OpenIdConnectEvents
            {
                // handle the logout redirection 
                OnRedirectToIdentityProviderForSignOut = (context) =>
                {
                    var logoutUri = $"https://{builder.Configuration["Auth0:Domain"]}/v2/logout?client_id={builder.Configuration["Auth0:ClientId"]}";

                    var postLogoutUri = context.Properties.RedirectUri;
                    if (!string.IsNullOrEmpty(postLogoutUri))
                    {
                        if (postLogoutUri.StartsWith("/"))
                        {
                            // transform to absolute
                            var request = context.Request;
                            postLogoutUri = request.Scheme + "://" + request.Host + request.PathBase + postLogoutUri;
                        }
                        logoutUri += $"&returnTo={Uri.EscapeDataString(postLogoutUri)}";
                    }

                    context.Response.Redirect(logoutUri);
                    context.HandleResponse();

                    return Task.CompletedTask;
                },
                OnRedirectToIdentityProvider = context =>
                {
                    // The context's ProtocolMessage can be used to pass along additional query parameters
                    // to Auth0's /authorize endpoint.
                    // 
                    // Set the audience query parameter to the API identifier to ensure the returned Access Tokens can be used
                    // to call protected endpoints on the corresponding API.
                    context.ProtocolMessage.SetParameter("audience", "https://auth0-api1");
                    context.ProtocolMessage.AcrValues = "http://schemas.openid.net/pape/policies/2007/06/multi-factor";

                    return Task.FromResult(0);
                }
            };
        })
        .AddOpenIdConnect("EntraID", "EntraID", oidcOptions =>
        {
            builder.Configuration.Bind("AzureAd", oidcOptions);
            oidcOptions.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
            oidcOptions.SignOutScheme = IdentityConstants.ApplicationScheme;

            oidcOptions.CallbackPath = new PathString("/signin-oidc");
            oidcOptions.RemoteSignOutPath = new PathString("/signout-callback-oidc");
            oidcOptions.SignedOutCallbackPath = new PathString("/signout-oidc");

            oidcOptions.Scope.Add(OpenIdConnectScope.OpenIdProfile);
            oidcOptions.Scope.Add("user.read");
            oidcOptions.Scope.Add(OpenIdConnectScope.OfflineAccess);
            oidcOptions.Authority = $"https://login.microsoftonline.com/{builder.Configuration["AzureAd:TenantId"]}/v2.0/";
            oidcOptions.ClientId = builder.Configuration["AzureAd:ClientId"];
            oidcOptions.ClientSecret = builder.Configuration["AzureAd:ClientSecret"];
            oidcOptions.ResponseType = OpenIdConnectResponseType.Code;
            oidcOptions.MapInboundClaims = false;
            oidcOptions.SaveTokens = true;
            oidcOptions.TokenValidationParameters.NameClaimType = JwtRegisteredClaimNames.Name;
            oidcOptions.TokenValidationParameters.RoleClaimType = "role";

            oidcOptions.Events = new OpenIdConnectEvents
            {
                OnMessageReceived = async context =>
                {
                    var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                    logger.LogInformation("OnMessageReceived from identity provider. Scheme: {Scheme: }", context.Scheme.Name);

                    context.HandleResponse();
                    context.Response.Redirect($"/Home/Test");
                    await Task.CompletedTask;
                },
            };
        });
        // This code creates it own scheme and so does not fit well into identity and external authentication
        //.AddMicrosoftIdentityWebApp(options =>
        //{
        //    builder.Configuration.Bind("AzureAd", options);
        //    options.SignInScheme = "mscreatedscheme"; // you would need to add this to the callback an the logout
        //    options.SignOutScheme = IdentityConstants.ApplicationScheme;
        //
        //    options.CallbackPath = "/signin-oidc";
        //    options.RemoteSignOutPath = "/signout-callback-oidc";
        //    options.SignedOutCallbackPath = "/signout-oidc";
        //
        //    options.MapInboundClaims = false;
        //    options.UsePkce = true;
        //    options.Events = new OpenIdConnectEvents
        //    {
        //        OnRedirectToIdentityProvider = context =>
        //        {
        //            context.ProtocolMessage.AcrValues = "http://schemas.openid.net/pape/policies/2007/06/multi-factor";
        //            return Task.FromResult(0);
        //        },
        //        OnTokenResponseReceived = context =>
        //        {
        //            var idToken = context.TokenEndpointResponse.IdToken;
        //            return Task.CompletedTask;
        //        }
        //    };
        //}, copt => { }, "EntraID", "mscreatedscheme", false, "Entra ID")
        //.EnableTokenAcquisitionToCallDownstreamApi(["User.Read"])
        //.AddMicrosoftGraph()
        //.AddDistributedTokenCaches();

        builder.Services.AddRazorPages();
           // .AddMicrosoftIdentityUI();

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseStaticFiles();
        app.UseRouting();
        app.UseIdentityServer();
        app.UseAuthorization();

        app.MapRazorPages()
            .RequireAuthorization();

        return app;
    }
}
