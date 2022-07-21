namespace TaskService
{
    public class Task
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }
        public TaskStatus TaskStatus { get; set; }
        public int CustomerId { get; set; }
    }

    public enum TaskStatus
    {
        STARTED,
        IN_PROGRESS,
        COMPLETED,
        FAILED
    }
}
