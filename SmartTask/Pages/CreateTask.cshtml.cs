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

        // db og usermanager hentes når klassen oprettes
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

        // Opgave oprettes POST
        public async Task<IActionResult> OnPostAsync()
        {
            // Tjek om titel er tom
            if (string.IsNullOrWhiteSpace(Title))
            {
                ErrorMessage = "Titel må ikke være tom.";
                return Page();
            }

            var userId = _userManager.GetUserId(User);

            // Opret nyt task objekt og udfyld med brugerens indtastede data fra <form>-feltet
            var task = new TodoItem
            {
                Title = Title,
                Description = Description = Description ?? "",
                Deadline = Deadline,
                Priority = Priority,
                Status = Status,
                UserId = userId!
            };

            // Den opdaterede task bliver oprettet i databasen af EF Core
            _db.TodoItems.Add(task);
            await _db.SaveChangesAsync();

            // Brugeren sendes tilbage til /Dashboard med en succes-notifikation
            TempData["Success"] = "Opgave oprettet!";
            return RedirectToPage("/Dashboard");
        }
    }
}