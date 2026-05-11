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

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userId = _userManager.GetUserId(User);
            var task = await _db.TodoItems
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (task == null)
                return RedirectToPage("/Dashboard");

            TaskId = id;
            Title = task.Title;
            Description = task.Description;
            Deadline = task.Deadline;
            Priority = task.Priority;
            Status = task.Status;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Title))
            {
                ErrorMessage = "Titel må ikke være tom.";
                return Page();
            }

            var userId = _userManager.GetUserId(User);
            var task = await _db.TodoItems
                .FirstOrDefaultAsync(t => t.Id == TaskId && t.UserId == userId);

            if (task == null)
                return RedirectToPage("/Dashboard");

            task.Title = Title;
            task.Description = Description ?? "";
            task.Deadline = Deadline;
            task.Priority = Priority;
            task.Status = Status;

            await _db.SaveChangesAsync();

            TempData["Success"] = "Opgave gemt!";
            return RedirectToPage("/Dashboard");
        }
    }
}