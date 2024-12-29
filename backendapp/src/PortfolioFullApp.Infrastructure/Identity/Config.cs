using IdentityServer4;
using IdentityServer4.Models;

namespace PortfolioFullApp.Infrastructure.Identity
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope>
            {
                new ApiScope("portfolio.read", "Read Portfolio Data"),
                new ApiScope("portfolio.write", "Write Portfolio Data"),
                new ApiScope("portfolio.full", "Full access to Portfolio"),
                new ApiScope("portfolio.mobile", "Mobile App Access")
            };

        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                // Frontend için interactive client
                new Client
                {
                    ClientId = "portfolio.web",
                    ClientName = "Portfolio Web App",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,

                    RedirectUris = { "http://localhost:3000/callback" },
                    PostLogoutRedirectUris = { "http://localhost:3000/" },
                    AllowedCorsOrigins = { "http://localhost:3000" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "portfolio.read",
                        "portfolio.write"
                    },

                    AllowOfflineAccess = true,
                    RequireConsent = false,
                    AccessTokenLifetime = 3600,
                    AbsoluteRefreshTokenLifetime = 2592000,
                    RefreshTokenUsage = TokenUsage.OneTimeOnly
                },

                // Mobil client (yeni)
                new Client
                {
                    ClientId = "portfolio.mobile",
                    ClientName = "Portfolio Mobile App",

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,
                    
                    // Mobil deep linking için URI şemaları
                    RedirectUris =
                    {
                        "com.portfolio.app://callback",
                        "com.portfolio.app://signin-oidc"
                    },
                    PostLogoutRedirectUris =
                    {
                        "com.portfolio.app://signout-callback-oidc",
                        "com.portfolio.app://logout"
                    },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "portfolio.read",
                        "portfolio.write",
                        "portfolio.mobile"
                    },

                    AllowOfflineAccess = true,
                    RequireConsent = false,
                    
                    // Token ayarları
                    AccessTokenLifetime = 3600,
                    RefreshTokenUsage = TokenUsage.OneTimeOnly,
                    RefreshTokenExpiration = TokenExpiration.Sliding,
                    AbsoluteRefreshTokenLifetime = 2592000,
                    SlidingRefreshTokenLifetime = 1296000,
                    
                    // Mobil için ek güvenlik ayarları
                    AllowPlainTextPkce = false,
                    RequireRefreshClientSecret = false,
                    UpdateAccessTokenClaimsOnRefresh = true
                }
            };
    }
}