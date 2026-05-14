using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SmartTask.Pages
{
    public class RegisterModel : PageModel
    {
        // ASP.NET Core user manager - bruges til at håndtere brugere
        private readonly UserManager<IdentityUser> _userManager;

        // Dependency-inejction - usermanageren hentes altså i det øjeblik, klassen oprettes
        public RegisterModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public string Name { get; set; } = "";

        [BindProperty]
        public string Email { get; set; } = "";

        [BindProperty]
        public string Password { get; set; } = "";

        public string? ErrorMessage { get; set; }

        // Sender Async POST til ASP.NET Core Identity
        // EF Core opretter automatisk en INSERT på baggrund af informationerne
        public async Task<IActionResult> OnPostAsync()
        {
            //IdentityUser er en klasse leveret af ASP.NET core identity
            var user = new IdentityUser
            {
                UserName = Email,
                Email = Email
            };

            var result = await _userManager.CreateAsync(user, Password);

            if (result.Succeeded)
            {
                return RedirectToPage("/Login");
            }

            ErrorMessage = result.Errors.First().Description;
            return Page();
        }
    }
}