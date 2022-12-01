using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raime.Shop.Api.Controllers.ModerationControllers
{
    [ApiController]
    [Route("api/users-moderation")]
    public class UsersModerationController : ControllerBase
    {
        private readonly AppIdentityDbContext identityDbContext;

        public UsersModerationController(AppIdentityDbContext identityDbContext)
        {
            this.identityDbContext = identityDbContext;
        }

        [HttpGet("allCustomers")]
        public IActionResult GetAllUsers()
        {
            return Ok(identityDbContext.Users.ToList());
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> RemoveUser(string userId)
        {
            var user = await identityDbContext.Users.FindAsync(userId);

            identityDbContext.Users.Remove(user);

            await identityDbContext.SaveChangesAsync();

            return Ok($"user with id: {userId} sucsesfully removed from store");
        }
    }
}
