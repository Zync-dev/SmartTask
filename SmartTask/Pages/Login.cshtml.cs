using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SmartTask.Pages
{
    public class LoginModel : PageModel
    {
        // ASP.NET Identity klasse, indeholder information om brugerens login status
        private readonly SignInManager<IdentityUser> _signInManager;

        // Hent signInManager når klassen oprettes
        public LoginModel(SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [BindProperty]
        public string Email { get; set; } = "";

        [BindProperty]
        public string Password { get; set; } = "";

        public string? ErrorMessage { get; set; }

        // Her logges brugeren ind
        public async Task<IActionResult> OnPostAsync()
        {
            // Tjek om brugeren findes, og om alle data er korrekte
            var result = await _signInManager.PasswordSignInAsync(
                Email,
                Password,
                isPersistent: true,
                lockoutOnFailure: false
            );

            // Hvis ja, log ind
            if (result.Succeeded)
            {
                return RedirectToPage("/Dashboard");
            }

            // Hvis nej, vis fejl
            ErrorMessage = "Forkert email eller adgangskode.";
            return Page();
        }
    }
}