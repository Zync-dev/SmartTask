namespace SmartTask.Models
{
    // Klassen for ToDoItem

    public enum Priority
    {
        Low,
        Medium,
        High
    }

    public enum Status
    {
        NotStarted,
        InProgress,
        Done
    }

    public class TodoItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public DateTime? Deadline { get; set; }
        public Priority Priority { get; set; } = Priority.Medium;
        public Status Status { get; set; } = Status.NotStarted;
        public string UserId { get; set; } = "";
    }
}