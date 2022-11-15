﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blog_MVC.Data;
using Blog_MVC.Models;
using Blog_MVC.Services;
using Blog_MVC.Services.Interfaces;

namespace Blog_MVC.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogPostsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IBlogPostService _blogPostService;

        public BlogPostsController(ApplicationDbContext context, IBlogPostService blogPostService)
        {
            _context = context;
            _blogPostService = blogPostService;
        }

        // GET: api/BlogPosts
        // ActionResult returns a JSON result for the queried data
        /// <summary>
        /// Get the most recent blog posts according to parameter
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("GetRecentBlogPosts/{num}")]
        public async Task<ActionResult<IEnumerable<BlogPost>>> GetBlogPosts(int num)
        {
            num = num == 0 ? 3 : num;
            return await _context.BlogPosts.Where(b => b.IsPublished == true && b.IsDeleted == false).OrderByDescending(b=>b.DateCreated).Take(num).ToListAsync();
        }

        // GET: api/BlogPosts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BlogPost>> GetBlogPost(int id)
        {
            var blogPost = await _context.BlogPosts.FindAsync(id);

            if (blogPost == null)
            {
                return NotFound();
            }

            return blogPost;
        }

        // PUT: api/BlogPosts/5
        // For APIs, "put" is updating an existing item - similar to edit
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBlogPost(int id, BlogPost blogPost)
        {
            if (id != blogPost.Id)
            {
                return BadRequest();
            }

            _context.Entry(blogPost).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BlogPostExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/BlogPosts
        // For APIs, "post" creates a new record
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BlogPost>> PostBlogPost(BlogPost blogPost)
        {
            _context.BlogPosts.Add(blogPost);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBlogPost", new { id = blogPost.Id }, blogPost);
        }

        // DELETE: api/BlogPosts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlogPost(int id)
        {
            var blogPost = await _context.BlogPosts.FindAsync(id);
            if (blogPost == null)
            {
                return NotFound();
            }

            _context.BlogPosts.Remove(blogPost);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BlogPostExists(int id)
        {
            return _context.BlogPosts.Any(e => e.Id == id);
        }
    }
}
