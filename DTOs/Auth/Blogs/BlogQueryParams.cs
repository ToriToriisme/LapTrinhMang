namespace BlogApi.DTOs.Blogs
{
    public class BlogQueryParameters
    {
        public string? Keyword { get; set; }
        public string? SortBy { get; set; } = "createdAt";
        public bool Desc { get; set; } = true;
    }
}
