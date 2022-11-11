using Blog_MVC.Data;
using Blog_MVC.Models;
using Blog_MVC.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using X.PagedList;

namespace Blog_MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IBlogPostService _blogPostService;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IBlogPostService blogPostService)
        {
            _logger = logger;
            _context = context;
            _blogPostService = blogPostService;
        }

        // GET & POST: Pagination
        public async Task<IActionResult> Index(int? pageNum)
        {
            int pageSize = 5;
            // if null, sets page = 1
            int page = pageNum ?? 1;
            
            IPagedList<BlogPost> model = (await _blogPostService.GetAllBlogPostsAsync()).Where(b=>b.IsDeleted == false && b.IsPublished == true).ToPagedList(page, pageSize);

            return View(model);
        }

        // GET & POST: custom search function
        public async Task<IActionResult> SearchIndex(string searchString, int? pageNum)
        {
            int pageSize = 5;
            int page = pageNum ?? 1;

            ViewData["SearchString"] = searchString;
            IPagedList<BlogPost> model = _blogPostService.SearchBlogPosts(searchString).ToPagedList(page, pageSize);

            return View(nameof(Index), model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}