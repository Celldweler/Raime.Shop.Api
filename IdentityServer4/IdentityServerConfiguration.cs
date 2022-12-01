using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;

namespace Raime.Shop.Api.IdentityServer4
{
    public static class IdentityServerConfiguration
    {
        public static IEnumerable<ApiScope> ApiScopes =>
            new[]
            {
                new ApiScope(IdentityServerConstants.LocalApi.ScopeName, new[] { RaimeShopConstants.Claims.Role}),
            };

        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                new IdentityResources.Profile(),
                new IdentityResources.OpenId(),
                new IdentityResources.Email(),

                //new IdentityResources.Phone(),
                new IdentityResources.Address(),

                new IdentityResource("phone_number", new[] {"phone_number"}),

                new IdentityResource(RaimeShopConstants.IdentityResources.RoleScope, new[] { RaimeShopConstants.Claims.Role }),
            };

        public static IEnumerable<Client> Clients =>
            new[]
            {
                new Client
                {
                    UpdateAccessTokenClaimsOnRefresh = true,
                    ClientId = "web-client",

                    RedirectUris = {
                        "https://localhost:3000/oidc/signin-callback.html",
                        "https://localhost:3000/oidc/signin-silent-callback.html",
                    },
                    PostLogoutRedirectUris = {"https://localhost:3000"},
                    AllowedCorsOrigins = {"https://localhost:3000"},

                    AllowedScopes =
                    {
                        IdentityServerConstants.LocalApi.ScopeName,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        
                        IdentityServerConstants.StandardScopes.Address,
                        IdentityServerConstants.StandardScopes.Phone,
                        "phone_number",
                        //IdentityServerConstants.StandardScopes.OfflineAccess,
                        RaimeShopConstants.IdentityResources.RoleScope,
                    },

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireConsent = false,
                    RequireClientSecret = false,

                    //// setings for renew access_token
                    //AllowOfflineAccess = true,
                    
                    //AccessTokenLifetime = 1, // 1.5 minutes
                    //AbsoluteRefreshTokenLifetime = 0,

                    //UpdateAccessTokenClaimsOnRefresh = true,
                    //RefreshTokenExpiration = TokenExpiration.Sliding,

                    //RefreshTokenUsage = TokenUsage.OneTimeOnly,
                }
            };
    }
}
