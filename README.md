# Duende ProfileService ASP.NET Core Identity

## Setup

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

2025-05-04 Initial version

## Links

https://docs.duendesoftware.com/identityserver/reference/services/profile-service

https://duendesoftware.com/products/identityserver

https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity

https://learn.microsoft.com/en-us/aspnet/core/security/authentication/claims

https://github.com/damienbod/MulitipleClientClaimsMapping

https://learn.microsoft.com/en-us/aspnet/core/security/authentication/social/