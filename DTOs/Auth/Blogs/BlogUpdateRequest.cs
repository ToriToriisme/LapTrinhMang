namespace BlogApi.DTOs.Blogs
{
    public class BlogUpdateRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}
