namespace BlogApi.DTOs.Blogs
{
    public class BlogCreateRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;

        // Tạm thời nhận từ client; sau sẽ lấy từ JWT claim
        public int UserId { get; set; }
    }
}
