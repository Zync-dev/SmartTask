using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SmartTask.Pages
{
    public class LogoutModel : PageModel
    {
        // ASP.NET Identity klasse, indeholder information om brugerens login status
        private readonly SignInManager<IdentityUser> _signInManager;

        // Hent signInManager når klassen oprettes
        public LogoutModel(SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
        }

        // Her bruges SignInManager til at logge brugeren ud af systemet
        public async Task<IActionResult> OnPostAsync()
        {
            await _signInManager.SignOutAsync();
            return RedirectToPage("/Index");
        }
    }
}