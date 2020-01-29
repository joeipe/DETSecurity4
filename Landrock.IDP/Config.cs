using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Landrock.IDP
{
    public static class Config
    {
        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "d860efca-22d9-47fd-8249-791ba61b07c7",
                    Username = "Joe",
                    Password = "password",

                    Claims = new List<Claim>
                    {
                        new Claim("given_name", "Joe"),
                        new Claim("family_name", "Ipe"),
                        new Claim("address", "187 George St"),
                        new Claim("role", "Admin"),
                        new Claim("country", "IN"),
                        new Claim("subscriptionlevel", "Admin"),
                    }
                },
                new TestUser
                {
                    SubjectId = "b7539694-97e7-4dfe-84da-b4256e1ff5c7",
                    Username = "Josie",
                    Password = "password",

                    Claims = new List<Claim>
                    {
                        new Claim("given_name", "Josie"),
                        new Claim("family_name", "Riseley"),
                        new Claim("address", "Big Street 2"),
                        new Claim("role", "FreeUser"),
                        new Claim("country", "AU"),
                        new Claim("subscriptionlevel", "FreeUser"),
                    }
                }
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Address(),
                new IdentityResource("roles", "Your role(s)", new List<string>() { "role" }),
                new IdentityResource("country", "The country you're living in", new List<string>() { "country" }),
                new IdentityResource("subscriptionlevel", "Your subscription level", new List<string>() { "subscriptionlevel" })
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("detsecurity4api", "DETSecurity4 API", new List<string>() { "role" })
                {
                     ApiSecrets = { new Secret("apisecret".Sha256()) }
                }
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>()
            {
                new Client
                {
                    ClientName = "DETSecurity4",
                    ClientId = "DETSecurity4Client_ClientId",
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    RequireConsent = false,
                    AccessTokenType = AccessTokenType.Reference, // default is JWT (if this is set then 'ApiSecrets' has to be set)
                    //IdentityTokenLifetime = ... //default 5 min
                    //AuthorizationCodeLifetime = ... //default 5 min
                    AccessTokenLifetime = 300, //default 1 hr
                    AllowOfflineAccess = true,
                    //AbsoluteRefreshTokenLifetime = ... //default 30 days
                    UpdateAccessTokenClaimsOnRefresh = true,
                    RedirectUris = new List<string>()
                    {
                        "https://localhost:44392/signin-oidc"
                    },
                    PostLogoutRedirectUris = new List<string>()
                    {
                        "https://localhost:44392/signout-callback-oidc"
                    },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Address,
                        "roles",
                        "country",
                        "subscriptionlevel",
                        "detsecurity4api",

                    },
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    }
                }
            };
        }
    }
}
