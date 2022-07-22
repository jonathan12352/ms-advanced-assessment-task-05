namespace TaskProcessorService
{
    public class Tasks
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }
        public TaskStatus TaskStatus { get; set; }
        public string CustomerId { get; set; }
    }

    public enum TaskStatus
    {
        STARTED,
        IN_PROGRESS,
        COMPLETED,
        FAILED
    }
}
