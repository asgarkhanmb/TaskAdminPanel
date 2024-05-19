using Fiorello_PB101.Data;
using Fiorello_PB101.Models;
using Fiorello_PB101.Services;
using Fiorello_PB101.Services.Interfaces;
using Fiorello_PB101.ViewModels.Blog;
using Fiorello_PB101.ViewModels.Categories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Fiorello_PB101.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BlogController : Controller
    {
        private readonly IBlogService _blogService;

        private readonly AppDbContext _context;
        public BlogController(IBlogService blogService
                                   ,AppDbContext context)
        {
            _blogService = blogService;
            _context = context;
            
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _blogService.GetAllAsync());
        }
        [HttpGet]
        public IActionResult Create() 
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BlogCreateVM blog)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            bool existBlog = await _blogService.ExistAsync(blog.Title);
            if (existBlog)
            {
                ModelState.AddModelError("Description", "This blog already exist");
                return View();
            }
           
            await _blogService.CreateAsync(new Blog { Description = blog.Description,Title=blog.Title,Image=blog.Image= "blog-feature-img-1.jpg" });
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null) return BadRequest();

            var category = await _blogService.GetByIdAsync((int)id);
            if (category is null) return NotFound();
            await _blogService.DeleteAsync(category);

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult>Detail(int? id)
        {
            Blog blog = await _context.Blogs.FirstOrDefaultAsync(m => m.Id == id);
            return View(blog);
        }
    }
}
