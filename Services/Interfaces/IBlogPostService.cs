using Blog_MVC.Models;

namespace Blog_MVC.Services.Interfaces
{
    public interface IBlogPostService
    {
        public Task<List<BlogPost>> GetAllBlogPostsAsync();

        public Task<List<BlogPost>> GetPopularBlogPostsAsync();

        public Task<List<BlogPost>> GetRecentBlogPostsAsync(int count);

        public Task<List<Category>> GetCategoriesAsync();

        public Task<List<Tag>> GetTagsAsync();

        //public Task<List<Tag>> GetBlogPostTags(IEnumerable<int> tagIds, int blogPostId);

        public Task RemoveAllTagsAsync(int blogPostId);

        public Task AddTagsToBlogPostAsync(IEnumerable<int> tagIds, int blogPostId);
        public Task AddTagsToBlogPostAsync(string tagNames, int blogPostId);

        public IEnumerable<BlogPost> SearchBlogPosts(string searchString);

        public Task<bool> ValidateSlugAsync(string title, int blogPostId);
    }
}
