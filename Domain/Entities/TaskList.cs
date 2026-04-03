namespace Domain.Entities
{
    public class TaskList
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string OwnerId { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<string> SharedWithUserIds { get; set; } = new();
    }
}
