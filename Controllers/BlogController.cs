using BlogApi.Common;
using BlogApi.Data;
using BlogApi.DTOs.Blogs;
using BlogApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlogQueryParams = BlogApi.DTOs.Blogs.BlogQueryParameters;
namespace BlogApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // yêu cầu JWT cho tất cả API trong controller này
    public class BlogController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BlogController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/blog
        [HttpGet]
        [AllowAnonymous] // public
        public async Task<IActionResult> GetAll([FromQuery] BlogQueryParams query)
        {
            var blogs = await _context.Blogs
                .Include(b => b.User)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

            if (!string.IsNullOrEmpty(query.Keyword))
            {
                blogs = blogs
                    .Where(b => b.Title.Contains(query.Keyword, StringComparison.OrdinalIgnoreCase) ||
                                b.Content.Contains(query.Keyword, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return Ok(ApiResponse<List<Blog>>.Ok(blogs, "Get blogs success"));
        }

        // GET: api/blog/{id}
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var blog = await _context.Blogs.Include(b => b.User).FirstOrDefaultAsync(b => b.Id == id);
            if (blog == null)
                return NotFound(ApiResponse<object>.Fail("Blog not found"));

            return Ok(ApiResponse<Blog>.Ok(blog, "Get blog success"));
        }

        // POST: api/blog
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BlogCreateRequest request)
        {
            var blog = new Blog
            {
                Title = request.Title,
                Content = request.Content,
                UserId = request.UserId,
                CreatedAt = DateTime.Now
            };

            _context.Blogs.Add(blog);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<Blog>.Ok(blog, "Blog created successfully"));
        }

        // PUT: api/blog/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] BlogUpdateRequest request)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
                return NotFound(ApiResponse<object>.Fail("Blog not found"));

            blog.Title = request.Title;
            blog.Content = request.Content;
            blog.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<Blog>.Ok(blog, "Blog updated successfully"));
        }

        // DELETE: api/blog/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
                return NotFound(ApiResponse<object>.Fail("Blog not found"));

            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<object?>.Ok(null, "Blog deleted successfully"));
        }
    }
}
