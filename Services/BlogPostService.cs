using Blog_MVC.Data;
using Blog_MVC.Extensions;
using Blog_MVC.Models;
using Blog_MVC.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Blog_MVC.Services
{
    public class BlogPostService : IBlogPostService
    {
        private readonly ApplicationDbContext _context;

        public BlogPostService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<BlogPost>> GetAllBlogPostsAsync()
        {
            try
            {
                List<BlogPost> blogPosts = await _context.BlogPosts.Include(b => b.Comments).Include(b => b.Category).Include(b=>b.Tags).OrderByDescending(b => b.DateCreated).ToListAsync();
                return blogPosts;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> ValidateSlugAsync(string title, int blogPostId)
        {

            try
            {
                string newSlug = title.Slugify();

                if (blogPostId == 0)
                {
                    return !(await _context.BlogPosts.AnyAsync(b => b.Slug == newSlug));
                }
                else
                {
                    BlogPost? blogPost = await _context.BlogPosts.AsNoTracking().FirstOrDefaultAsync(c => c.Id == blogPostId);
                    string? oldSlug = blogPost?.Slug;

                    // check if the new blogpost title (newSlug) is same as old title (oldSlug)
                    if (!string.Equals(newSlug, oldSlug))
                    {
                        // check if the blogpost title (newSlug) exists in database
                        return !(await _context.BlogPosts.AnyAsync(b => b.Id == blogPostId && b.Slug == newSlug));
                    }
                }
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
