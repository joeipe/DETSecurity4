using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DETSecurity4.Client.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using IdentityModel.Client;
using System.Net.Http;
using Newtonsoft.Json;
using DETSecurity4.Client.Services;

namespace DETSecurity4.Client.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DETSecurity4APIHttpClient _detSecurity4APIHttpClient;

        public HomeController(ILogger<HomeController> logger, DETSecurity4APIHttpClient detSecurity4APIHttpClient)
        {
            _logger = logger;
            _detSecurity4APIHttpClient = detSecurity4APIHttpClient;
        }

        public async Task<IActionResult> Index()
        {
            await WriteOutIdentityInformation();

            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize(Policy = "AdminFromIndia")]
        public async Task<IActionResult> Address()
        {
            var httpclient = new HttpClient();
            var disco = await httpclient.GetDiscoveryDocumentAsync("https://localhost:44335/");

            var accessToken = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            var response = await httpclient.GetUserInfoAsync(new UserInfoRequest
            {
                Address = disco.UserInfoEndpoint,
                Token = accessToken
            });

            if (response.IsError)
            {
                throw new Exception(
                    "Problem accessing the UserInfo endpoint."
                    , response.Exception);
            }

            var address = response.Claims.FirstOrDefault(c => c.Type == "address")?.Value;
            var role = response.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
            string weatherForecast = "";
            {
                var httpClient = await _detSecurity4APIHttpClient.GetClient();
                var resp = await httpClient.GetAsync("weatherforecast").ConfigureAwait(false);
                if (resp.IsSuccessStatusCode)
                {
                    string resultContentString = await resp.Content.ReadAsStringAsync();
                    weatherForecast = resultContentString;
                }
                else if (resp.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {

                }
            }
            /*
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:44358");
                    var result = await client.GetAsync("/weatherforecast");
                    result.EnsureSuccessStatusCode();
                    string resultContentString = await result.Content.ReadAsStringAsync();
                    weatherForecast = resultContentString;
                }
            }
            */

            return View(new UserDetailsViewModel(address, role, weatherForecast));
        }

        public async Task Logout()
        {
            var httpclient = new HttpClient();
            var disco = await httpclient.GetDiscoveryDocumentAsync("https://localhost:44335/");

            // get the access token to revoke 
            var accessToken = await HttpContext
              .GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                var revokeAccessTokenResponse =
                    await httpclient.RevokeTokenAsync(new TokenRevocationRequest
                    {
                        Address = disco.RevocationEndpoint,
                        ClientId = "DETSecurity4Client_ClientId",
                        ClientSecret = "secret",

                        Token = accessToken
                    });

                if (revokeAccessTokenResponse.IsError)
                {
                    throw new Exception("Problem encountered while revoking the access token."
                        , revokeAccessTokenResponse.Exception);
                }
            }

            /*
            // revoke the refresh token as well
            var refreshToken = await HttpContext
             .GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);

            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                var revokeRefreshTokenResponse =
                    await revocationClient.RevokeRefreshTokenAsync(refreshToken);

                if (revokeRefreshTokenResponse.IsError)
                {
                    throw new Exception("Problem encountered while revoking the refresh token."
                        , revokeRefreshTokenResponse.Exception);
                }
            }*/

            // Clears the  local cookie ("Cookies" must match name from scheme)
            await HttpContext.SignOutAsync("Cookies");
            await HttpContext.SignOutAsync("oidc");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task WriteOutIdentityInformation()
        {
            // get the saved identity token
            var identityToken = await HttpContext
                .GetTokenAsync(OpenIdConnectParameterNames.IdToken);

            // write it out
            Debug.WriteLine($"Identity token: {identityToken}");

            // write out the user claims
            foreach (var claim in User.Claims)
            {
                Debug.WriteLine($"Claim type: {claim.Type} - Claim value: {claim.Value}");
            }
        }
    }
}
