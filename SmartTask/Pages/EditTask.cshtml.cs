using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SmartTask.Data;
using SmartTask.Models;

namespace SmartTask.Pages
{
    [Authorize]
    public class EditTaskModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        // Henter ApplicationDbContext og UserManager, når klassen oprettes
        public EditTaskModel(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [BindProperty]
        public int TaskId { get; set; }

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

        // Henter opgavens informationer - GET
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userId = _userManager.GetUserId(User);

            // Hent brugerens task ud fra både ID og UserID - dette er et ekstra sikkerhedstjek (ellers kan brugeren ændre URL)
            var task = await _db.TodoItems.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            // Hvis ikke opgaven findes, sendes brugeren til /Dashboard
            if (task == null)
                return RedirectToPage("/Dashboard");

            // Siden bliver udfyldt med de hentede data fra databasen
            TaskId = id;
            Title = task.Title;
            Description = task.Description;
            Deadline = task.Deadline;
            Priority = task.Priority;
            Status = task.Status;

            return Page();
        }

        // Redigér opgave - POST
        public async Task<IActionResult> OnPostAsync()
        {
            // Tjek for tom titel
            if (string.IsNullOrWhiteSpace(Title))
            {
                ErrorMessage = "Titel må ikke være tom.";
                return Page();
            }

            var userId = _userManager.GetUserId(User);

            // Hent brugerens task ud fra både ID og UserID - dette er et ekstra sikkerhedstjek (ellers kan brugeren ændre URL)
            var task = await _db.TodoItems.FirstOrDefaultAsync(t => t.Id == TaskId && t.UserId == userId);

            // Hvis ikke opgaven findes, sendes brugeren til /Dashboard
            if (task == null)
                return RedirectToPage("/Dashboard");

            // Her bliver alle de nye værdier fra <form>-feltet skrevet ind i opgaven (EF core husker kontekst)
            task.Title = Title;
            task.Description = Description ?? "";
            task.Deadline = Deadline;
            task.Priority = Priority;
            task.Status = Status;

            // Ændringerne gemmes
            await _db.SaveChangesAsync();

            // Brugeren sendes videre med notifikation
            TempData["Success"] = "Opgave gemt!";
            return RedirectToPage("/Dashboard");
        }
    }
}