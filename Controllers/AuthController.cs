using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raime.Shop.Api.Controllers
{
    [Route("api/auth")]
    [ApiController]

    public class AuthController : ControllerBase
    {
        [HttpGet("logout")]
        public async Task<IActionResult> Logout(string logoutId,
            [FromServices] SignInManager<IdentityUser> signInManager,
            [FromServices] IIdentityServerInteractionService identityServerInteractionService)
        {
            await signInManager.SignOutAsync();

            var logoutContext = await identityServerInteractionService.GetLogoutContextAsync(logoutId);

            if (string.IsNullOrEmpty(logoutContext.PostLogoutRedirectUri))
            {
                return BadRequest();
            }

            return Redirect(logoutContext.PostLogoutRedirectUri);
        }
    }
}
