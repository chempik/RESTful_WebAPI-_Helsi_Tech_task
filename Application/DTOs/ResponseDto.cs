namespace Application.DTOs
{
    public class ResponseDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string OwnerId { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
