using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SmartTask.Data;
using SmartTask.Models;
using SmartTask.Services;

namespace SmartTask.Pages
{
    [Authorize] // Kun for brugere der er logget ind.
    public class DashboardModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly GroqService _groqService;

        // Her hentes db, usermanager og groqservice, når klassen startes.
        public DashboardModel(ApplicationDbContext db, UserManager<IdentityUser> userManager, GroqService groqService)
        {
            _db = db;
            _userManager = userManager;
            _groqService = groqService;
        }

        public List<TodoItem> TodoItems { get; set; } = new();

        // Statistik-kort - tæller antallet af opgaver i hver status
        public int CountNotStarted => TodoItems.Count(t => t.Status == Status.NotStarted);
        public int CountInProgress => TodoItems.Count(t => t.Status == Status.InProgress);
        public int CountDone => TodoItems.Count(t => t.Status == Status.Done);

        //Filterne kan udfyldes gennem URL'en, derfor SupportsGet = true
        [BindProperty(SupportsGet = true)]
        public string Filter { get; set; } = "all";

        [BindProperty]
        public string AiPrompt { get; set; } = "";

        public string? AiResponse { get; set; }

        // Henter alle brugerens opgaver fra databasen
        public async Task OnGetAsync()
        {
            var userId = _userManager.GetUserId(User);

            var query = _db.TodoItems.Where(t => t.UserId == userId);

            // En switch expression, som automatisk KUN henter de opgaver, som har det valgte filter, ved at ændre queryen inden den afsendes
            query = Filter switch
            {
                "notstarted" => query.Where(t => t.Status == Status.NotStarted),
                "inprogress" => query.Where(t => t.Status == Status.InProgress),
                "done" => query.Where(t => t.Status == Status.Done),
                "high" => query.Where(t => t.Priority == Priority.High),
                "medium" => query.Where(t => t.Priority == Priority.Medium),
                "low" => query.Where(t => t.Priority == Priority.Low),
                _ => query
            };

            // Her afsendes queryen
            TodoItems = await query
                .OrderBy(t => t.Deadline)
                .ToListAsync();
        }

        // Funktionalitet til at slette posts
        public async Task<IActionResult> OnPostDeleteTaskAsync(int id)
        {
            var userId = _userManager.GetUserId(User);
            var task = await _db.TodoItems.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            // Hvis opgaven findes, slettes den
            if (task != null)
            {
                _db.TodoItems.Remove(task);
                await _db.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        // Spørg AI funktionalitet POST
        public async Task<IActionResult> OnPostAskAiAsync()
        {
            var userId = _userManager.GetUserId(User);

            // Henter brugerens TodoItems fra databasen
            TodoItems = await _db.TodoItems
                .Where(t => t.UserId == userId)
                .OrderBy(t => t.Deadline)
                .ToListAsync();

            // Tjek om prompten er tom
            if (string.IsNullOrWhiteSpace(AiPrompt))
            {
                AiResponse = "Skriv et spørgsmål først.";
                return Page();
            }

            // Her sammensættes listen af opgaver til en string
            var taskList = string.Join("\n", TodoItems.Select(t => $"- {t.Title} (Beskrivelse: {t.Description}, Prioritet: {t.Priority}, Status: {t.Status}, Deadline: {t.Deadline?.ToString("dd/MM/yyyy") ?? "ingen"})"));

            // Prompten sammensættes - taskList og AiPromt, sendes samlet til AI'en
            var fullPrompt = $"Her er brugerens opgaver:\n{taskList}\n\nBrugerens spørgsmål: {AiPrompt}";

            AiResponse = await _groqService.GetAiResponseAsync(fullPrompt);

            return Page();
        }
    }
}