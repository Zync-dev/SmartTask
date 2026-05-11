using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartTask.Data;
using SmartTask.Models;

namespace SmartTask.Pages
{
    [Authorize]
    public class CreateTaskModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public CreateTaskModel(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [BindProperty]
        public string Title { get; set; } = "";

        [BindProperty]
        public string Description { get; set; } = "";

        [BindProperty]
        public DateTime? Deadline { get; set; }

        [BindProperty]
        public Priority Priority { get; set; } = Priority.Medium;

        [BindProperty]
        public Status Status { get; set; } = Status.NotStarted;

        public string? ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                ErrorMessage = "Titel må ikke være tom.";
                return Page();
            }

            var userId = _userManager.GetUserId(User);

            var task = new TodoItem
            {
                Title = Title,
                Description = Description = Description ?? "",
                Deadline = Deadline,
                Priority = Priority,
                Status = Status,
                UserId = userId!
            };

            _db.TodoItems.Add(task);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Opgave oprettet!";
            return RedirectToPage("/Dashboard");
        }
    }
}