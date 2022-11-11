using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Blog_MVC.Data;
using Blog_MVC.Models;
using Blog_MVC.Services.Interfaces;
using Blog_MVC.Services;
using Microsoft.AspNetCore.Authorization;
using Blog_MVC.Extensions;
using Microsoft.AspNetCore.Identity;
using X.PagedList;

namespace Blog_MVC.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class BlogPostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageService _imageService;
        private readonly IBlogPostService _blogPostService;
        private readonly UserManager<BlogUser> _userManager;

        public BlogPostsController(ApplicationDbContext context, IImageService imageService, IBlogPostService blogPostService, UserManager<BlogUser> userManager)
        {
            _context = context;
            _imageService = imageService;
            _blogPostService = blogPostService;
            _userManager = userManager;
        }

        // GET: BlogPosts
        [AllowAnonymous]
        public async Task<IActionResult> Index(int? blogPostId)
        {
            List<BlogPost> blogPosts = new List<BlogPost>();
            //List<Tag> blogPostTags = await _context.Tags.ToListAsync();

            //if ()
            //{
            //    blogPostTags = await _context.Tags.Include(b => b.BlogPosts).ToListAsync();
            //    return blogPostTags;
            //}

            var applicationDbContext = _context.BlogPosts.Include(b => b.Category).Include(t => t.Tags);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: BlogPosts/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(string? slug)
        {
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts
                .Include(b => b.Category).Include(c => c.Comments).ThenInclude(c => c.Author)
                .FirstOrDefaultAsync(m => m.Slug == slug);

            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        // default blog image from <a href="https://storyset.com/internet">Internet illustrations by Storyset</a>
        // GET: BlogPosts/Create
        public async Task<IActionResult> Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            ViewData["BlogPostTags"] = new MultiSelectList(await _blogPostService.GetTagsAsync(), "Id", "Name");
            return View(new BlogPost());
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Content,CategoryId,Abstract,IsDeleted,IsPublished,BlogPostImage")] BlogPost blogPost, IEnumerable<int> SelectedTags)
        {

            ModelState.Remove("CreatorId");

            if (ModelState.IsValid)
            {
                blogPost.CreatorId = _userManager.GetUserId(User);

                // get slug
                if (!await _blogPostService.ValidateSlugAsync(blogPost.Title!, blogPost.Id))
                {
                    ModelState.AddModelError("Title", "This title already exists!");
                    ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", blogPost.CategoryId);
                    return View(blogPost);
                }
                blogPost.Slug = blogPost.Title!.Slugify();

                blogPost.DateCreated = DateTime.UtcNow;

                if (blogPost.BlogPostImage != null)
                {
                    blogPost.ImageData = await _imageService.ConvertFileToByteArrayAsync(blogPost.BlogPostImage);
                    blogPost.ImageType = blogPost.BlogPostImage.ContentType;
                }

                _context.Add(blogPost);
                await _context.SaveChangesAsync();

                // add list of selected tags
                await _blogPostService.AddTagsToBlogPostAsync(SelectedTags, blogPost.Id);
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", blogPost.CategoryId);
            ViewData["BlogPostTags"] = new MultiSelectList(await _blogPostService.GetTagsAsync(), "Id", "Name");
            return View(blogPost);
        }

        // GET: BlogPosts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts.Include(t => t.Tags).FirstOrDefaultAsync(b => b.Id == id);

            if (blogPost == null)
            {
                return NotFound();
            }

            // select allows us to focus on just one attribute of the Tags list
            IEnumerable<int> blogPostTags = blogPost.Tags.Select(t => t.Id);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", blogPost.CategoryId);
            ViewData["BlogPostTags"] = new MultiSelectList(await _blogPostService.GetTagsAsync(), "Id", "Name", blogPostTags);
            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,CategoryId,Abstract,IsDeleted,IsPublished,BlogPostImage")] BlogPost blogPost, IEnumerable<int> selectedTags)
        {
            if (id != blogPost.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    blogPost.DateCreated = DateTime.SpecifyKind(blogPost.DateCreated, DateTimeKind.Utc);
                    blogPost.LastUpdated = DateTime.UtcNow;

                    if (blogPost.BlogPostImage != null)
                    {
                        blogPost.ImageData = await _imageService.ConvertFileToByteArrayAsync(blogPost.BlogPostImage);
                        blogPost.ImageType = blogPost.BlogPostImage.ContentType;
                    }

                    if (!await _blogPostService.ValidateSlugAsync(blogPost.Title!, blogPost.Id))
                    {
                        ModelState.AddModelError("Title", "This title already exists!");
                        ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", blogPost.CategoryId);
                        return View(blogPost);
                    }
                    blogPost.Slug = blogPost.Title!.Slugify();

                    _context.Update(blogPost);
                    await _context.SaveChangesAsync();

                    await _blogPostService.RemoveAllTagsAsync(blogPost.Id);
                    await _blogPostService.AddTagsToBlogPostAsync(selectedTags, blogPost.Id);

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogPostExists(blogPost.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", blogPost.CategoryId);
            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.BlogPosts == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts
                .Include(b => b.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.BlogPosts == null)
            {
                return Problem("Entity set 'ApplicationDbContext.BlogPosts'  is null.");
            }
            var blogPost = await _context.BlogPosts.FindAsync(id);
            if (blogPost != null)
            {
                // revised to soft delete so posts can be retrieved
                blogPost.IsDeleted = true;
                //_context.BlogPosts.Remove(blogPost);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogPostExists(int id)
        {
            return _context.BlogPosts.Any(e => e.Id == id);
        }
    }
}
