using Identity.Model.Constants.IdentityConfig;
using IdentityServer4;
using IdentityServer4.Models;

namespace Identity.API.Configuration
{
    public class IdentityConfiguration
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }
        public static IEnumerable<ApiResource> GetResourceApis()
        {
            return new List<ApiResource>
            {
                new ApiResource(name: InternalApis.MyAPI, displayName: "My Resource API") { Scopes = new List<string>() { InternalApis.MyAPI } },
                new ApiResource(name: InternalApis.IdentityServer, displayName: "Identity Server API") { Scopes = new List<string>() { InternalApis.IdentityServer } }
            };
        }

        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new[]
            {
                new ApiScope(name: InternalApis.MyAPI,   displayName: "My API Access"),
                new ApiScope(name: InternalApis.IdentityServer,   displayName: "Identity Server Api Access")
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client()
                {
                    ClientId = InternalClients.MyAPI,
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret(HashExtensions.Sha256("oilfGG9494FBBFF44gBBfsopFF4GG"))},
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        InternalApis.IdentityServer
                    }
                },
                new Client()
                {
                    ClientId = InternalClients.Mobile,
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets = { new Secret(HashExtensions.Sha256("oilfGG9494FBBskls44gBBfsopFF4GG")) },
                    AllowedScopes = new List<string>
                    {
                        InternalApis.MyAPI,
                        InternalApis.IdentityServer,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.OfflineAccess
                    },
                    AllowOfflineAccess = true,
                    RefreshTokenUsage = TokenUsage.OneTimeOnly,
                },
                new Client()
                {
                    ClientId = InternalClients.Web,
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets = {new Secret(HashExtensions.Sha256("3567fbffHFF4iofhkfFfH8")) },
                    AllowedScopes = new List<string>
                    {
                        InternalApis.MyAPI,
                        InternalApis.IdentityServer,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.OfflineAccess
                    },
                    AllowOfflineAccess = true,
                    RefreshTokenUsage = TokenUsage.OneTimeOnly,
                }
            };
        }


    }
}
