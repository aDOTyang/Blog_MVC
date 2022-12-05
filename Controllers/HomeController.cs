using Blog_MVC.Data;
using Blog_MVC.Models;
using Blog_MVC.Services;
using Blog_MVC.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
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
        private readonly IEmailSender _emailService;
        private readonly UserManager<BlogUser> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IBlogPostService blogPostService, IEmailSender emailService, UserManager<BlogUser> userManager)
        {
            _logger = logger;
            _context = context;
            _blogPostService = blogPostService;
            _emailService = emailService;
            _userManager = userManager;
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

        [Authorize]
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ContactMe()
        {
            EmailData model = new()
            {
                EmailAddress = User.Identity!.Name,
                Subject = "Contact Me: From My Blog"
            };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ContactMe(EmailData model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _emailService.SendEmailAsync(model.EmailAddress!, model.Subject!, model.Message!);
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception)
                {
                    return RedirectToAction("Index", "Home");
                    throw;
                }
            }
            return RedirectToAction("ContactMe", "Home");
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