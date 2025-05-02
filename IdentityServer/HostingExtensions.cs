using Duende.IdentityServer;
using DuendeProfileServiceAspNetCoreIdentity.Data;
using DuendeProfileServiceAspNetCoreIdentity.Models;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
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

        builder.Services.AddAuthentication()
            .AddMicrosoftIdentityWebApp(options =>
            {
                builder.Configuration.Bind("AzureAd", options);
                options.SignInScheme = "entraidcookie";
                options.UsePkce = true;
                options.Events = new OpenIdConnectEvents
                {
                    OnTokenResponseReceived = context =>
                    {
                        var idToken = context.TokenEndpointResponse.IdToken;
                        return Task.CompletedTask;
                    }
                };
            }, copt => { }, "EntraID", "entraidcookie", false, "Entra ID")
            .EnableTokenAcquisitionToCallDownstreamApi(["User.Read"])
            .AddMicrosoftGraph()
            .AddDistributedTokenCaches();

        builder.Services.AddRazorPages()
         .AddMicrosoftIdentityUI();

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
