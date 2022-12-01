using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Raime.Shop.Api.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private SignInManager<IdentityUser> _signInManager;
        private UserManager<IdentityUser> _userManager;

        [BindProperty]
        public RegisterForm Form { get; set; }
        public RegisterModel(SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
        public void OnGet(string returnUrl)
        {
            Form = new RegisterForm { ReturnUrl = returnUrl };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if(!ModelState.IsValid)
            {
                return Page();
            }

            var newUser = new IdentityUser(Form.Username);
            var existUser = _userManager.FindByNameAsync(newUser.UserName);
            if(existUser != null)
            {
                ModelState.AddModelError("Username", "User with the same username already exists");
                return Page();
            }
            var identityResult = await _userManager.CreateAsync(newUser, Form.Password);

            if(identityResult.Succeeded)
            {
                var signInResult = await _signInManager.PasswordSignInAsync(newUser, Form.Password, true, false);

                if(signInResult.Succeeded)
                {
                    return Redirect(Form.ReturnUrl);
                }
            }

            return Page();
        }

        public class RegisterForm
        {
            public string ReturnUrl { get; set; }

            public string Name { get; set; }
            public string Surname { get; set; }
            public string Email { get; set; }

            [Required]
            public string Username { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Compare("Password")]
            public string ConfirmPassword { get; set; }
        }
    }
}
