using Blog_MVC.Models;

namespace Blog_MVC.Services.Interfaces
{
    public interface IBlogPostService
    {
        public Task<bool> ValidateSlugAsync(string title, int blogPostId);

        public Task<List<BlogPost>> GetAllBlogPostsAsync();
    }
}
