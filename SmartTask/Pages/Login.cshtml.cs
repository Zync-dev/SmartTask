using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SmartTask.Pages
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;

        public LoginModel(SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [BindProperty]
        public string Email { get; set; } = "";

        [BindProperty]
        public string Password { get; set; } = "";

        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var result = await _signInManager.PasswordSignInAsync(
                Email,
                Password,
                isPersistent: true,
                lockoutOnFailure: false
            );

            if (result.Succeeded)
            {
                return RedirectToPage("/Dashboard");
            }

            ErrorMessage = "Forkert email eller adgangskode.";
            return Page();
        }
    }
}