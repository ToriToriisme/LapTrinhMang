namespace BlogApi.DTOs
{
    public class FileUploadRequest
    {
        public IFormFile File { get; set; } = default!;
    }
}
