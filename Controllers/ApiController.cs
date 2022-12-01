using IdentityModel;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Raime.Shop.Api.Controllers
{
    [ApiController] 
    public class ApiController : ControllerBase
    {
        protected string UserId => GetCalim(JwtClaimTypes.Subject);
        protected string UserName => GetCalim(JwtClaimTypes.PreferredUserName);

        private string GetCalim(string claimType)
            => User.Claims.FirstOrDefault(x => x.Type.Equals(claimType))?.Value;
    }
}
