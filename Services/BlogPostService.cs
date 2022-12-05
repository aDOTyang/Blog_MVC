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

        public async Task AddTagsToBlogPostAsync(IEnumerable<int> tagIds, int blogPostId)
        {
            try
            {
                BlogPost? blogPost = await _context.BlogPosts.FindAsync(blogPostId);

                foreach (int tagId in tagIds)
                {
                    Tag? tag = await _context.Tags.FindAsync(tagId);

                    if (blogPost != null && tag != null)
                    {
                        blogPost.Tags.Add(tag);
                    }
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// adds selected tag to blogpost if found, else creates new tag before adding to blogpost
        /// </summary>
        /// <param name="tagNames"></param>
        /// <param name="blogPostId"></param>
        /// <returns></returns>
        public async Task AddTagsToBlogPostAsync(string tagNames, int blogPostId)
        {
            try
            {
                BlogPost? blogPost = await _context.BlogPosts.FindAsync(blogPostId);

                // guard statement to eject from method instead of crashing in case of error
                if (blogPost == null) return;

                foreach(string tagName in tagNames.Split(","))
                {
                    if (string.IsNullOrEmpty(tagName.Trim())) continue;

                    Tag? tag = await _context.Tags.FirstOrDefaultAsync(t=>t.Name.Trim().ToLower() == tagName.Trim().ToLower());

                    if (tag != null)
                    {
                        blogPost.Tags.Add(tag);
                    } else
                    {
                        Tag newTag = new Tag() { Name = tagName.Trim() };
                        blogPost.Tags.Add(newTag);
                    }
                }
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<BlogPost>> GetAllBlogPostsAsync()
        {
            try
            {
                List<BlogPost> blogPosts = await _context.BlogPosts.Include(b => b.Comments).Include(b => b.Category).Include(b => b.Creator).Include(b => b.Tags).OrderByDescending(b => b.DateCreated).ToListAsync();
                return blogPosts;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //public async Task<List<Tag>> GetBlogPostTags(IEnumerable<int> tagIds, int blogPostId)
        //{
        //    try
        //    {
        //        BlogPost? blogPost = await _context.BlogPosts.FindAsync(blogPostId);

        //        foreach (int tagId in tagIds)
        //        {
        //            Tag? tag = await _context.Tags.FindAsync(tagId);

        //            if (blogPost != null && tag != null)
        //            {
        //                blogPost.Tags.Add(tag);
        //            }
        //        }
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}

        public async Task<List<Category>> GetCategoriesAsync()
        {
            try
            {
                return await _context.Categories.Include(c => c.BlogPosts).OrderByDescending(c=>c.BlogPosts.Count()).ToListAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<BlogPost>> GetPopularBlogPostsAsync()
        {
            try
            {
                List<BlogPost> blogPosts = await _context.BlogPosts.Where(b => b.IsDeleted == false && b.IsPublished == true).Include(b => b.Comments).ThenInclude(c => c.Author).Include(b => b.Category).Include(b => b.Tags).ToListAsync();
                return blogPosts.OrderByDescending(b => b.Comments.Count).ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<BlogPost>> GetRecentBlogPostsAsync(int count)
        {
            try
            {
                List<BlogPost> blogPosts = await _context.BlogPosts.Where(b => b.IsDeleted == false && b.IsPublished == true).Include(b => b.Comments).ThenInclude(c => c.Author).Include(b => b.Category).Include(b => b.Tags).ToListAsync();
                return blogPosts.OrderByDescending(b => b.DateCreated).Take(count).ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Tag>> GetTagsAsync()
        {

            try
            {
                List<Tag> tags = await _context.Tags.Include(t => t.BlogPosts).ToListAsync();
                return tags;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task RemoveAllTagsAsync(int blogPostId)
        {
            try
            {
                BlogPost? blogPost = await _context.BlogPosts.Include(t => t.Tags).FirstOrDefaultAsync(b => b.Id == blogPostId);

                blogPost!.Tags.Clear();
                _context.Update(blogPost);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IEnumerable<BlogPost> SearchBlogPosts(string searchString)
        {
            try
            {
                // interface can't be instantiated -> List allows us to represent as an IEnumerable
                IEnumerable<BlogPost> blogPosts = new List<BlogPost>();

                if (string.IsNullOrEmpty(searchString))
                {
                    return blogPosts;
                }
                else
                {
                    searchString = searchString.Trim().ToLower();

                    // no async needed on 'AsNoTracking'
                    blogPosts = _context.BlogPosts.Where(b=>b.IsDeleted == false && b.IsPublished == true)
                                                  .Where(b => b.Title!.ToLower().Contains(searchString) ||
                                                              b.Abstract!.ToLower().Contains(searchString) ||
                                                              b.Content!.ToLower().Contains(searchString) ||
                                                              b.Category!.Name!.ToLower().Contains(searchString) ||
                                                              b.Comments.Any(c => c.Body!.ToLower().Contains(searchString) ||
                                                                                c.Author!.FirstName!.ToLower().Contains(searchString) ||
                                                                                c.Author!.LastName!.ToLower().Contains(searchString)) ||
                                                              b.Tags!.Any(t => t.Name!.ToLower().Contains(searchString)))
                                                  .Include(b => b.Comments).ThenInclude(c => c.Author).Include(b=>b.Category).Include(b=>b.Tags).Include(b=>b.Creator)
                                                  .AsNoTracking().OrderByDescending(b=>b.DateCreated).AsEnumerable();
                    return blogPosts;
                }

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
                    // AsNoTracking returns a snapshot and cuts all ties - must recall if more info needed
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
