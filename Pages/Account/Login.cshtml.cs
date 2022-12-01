using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Raime.Shop.Api.Pages.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public LoginForm Form { get; set; }

        public void OnGet(string returnUrl)
        {
            Form = new LoginForm { ReturnUrl = returnUrl, Username = "", Password = "admin"};
        }

        public async Task<IActionResult> OnPost(
            [FromServices] SignInManager<IdentityUser> signInManager)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var signInResult = await signInManager.PasswordSignInAsync(Form.Username, Form.Password, true, false);

            if (signInResult.Succeeded)
            {
                return Redirect(Form.ReturnUrl);
            }

            return Page();
        }
        public class LoginForm
        {
            [Required] public string ReturnUrl { get; set; }
            [Required] public string Username { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }
    }
}
