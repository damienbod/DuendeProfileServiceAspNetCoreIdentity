# Duende ProfileService ASP.NET Core Identity

[![.NET](https://github.com/damienbod/DuendeProfileServiceAspNetCoreIdentity/actions/workflows/dotnet.yml/badge.svg)](https://github.com/damienbod/DuendeProfileServiceAspNetCoreIdentity/actions/workflows/dotnet.yml)

[Using multiple external identity providers from ASP.NET Core Identity and Duende IdentityServer](https://damienbod.com)

## Setup

The application is used as an identity provider. This can be used for local users or for external users using OpenID Connect federation. All applications using the application are separated from the further authentication systems. By using Duende, it is possible to use the high end OAuth an OpenID Connect authentication flows which are not supported by other identity providers at present. It would also be possible to use OpenIddict in this setup. The users of the server authenticate using OpenID Connect. The claims need to be mapped as well as each of the external authentication providers. The Identity Callback UI is used to handle all of the external authentication flow results. The claims from each external authentication are different and need to be mapped to the claims used in the closed system.

![ASP.NET Core Identity](https://github.com/damienbod/DuendeProfileServiceAspNetCoreIdentity/blob/main/images/overview.drawio.png)

```
dotnet new install Duende.IdentityServer.Templates

dotnet new isaspid  
```


## Create Migrations directly

```
Add-Migration "InitIdentityNew" -c ApplicationDbContext
```

```
Update-Database
```

## History

2025-05-17 Updated packages
2025-05-04 Initial version

## Links

https://docs.duendesoftware.com/identityserver/reference/services/profile-service

https://duendesoftware.com/products/identityserver

https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity

https://learn.microsoft.com/en-us/aspnet/core/security/authentication/claims

https://github.com/damienbod/MulitipleClientClaimsMapping

https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/
