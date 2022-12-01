using IdentityServer4;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Raime.Shop.Api.Controllers
{
    [ApiController]
    public class SecretController : ControllerBase
    {
        [HttpGet("/secret")]
        [Authorize(Policy = IdentityServerConstants.LocalApi.PolicyName)]
        public string Secret() => "secret message from Raime.Shop.Api";

        [HttpGet("/mod")]
        [Authorize(Policy = "mod")]
        public string Mod() => "mod";
    }
}
