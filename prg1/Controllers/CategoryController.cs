using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prg1.Models; 

namespace prg1.Controllers
{
    [Authorize(Roles = "Admin")] 
    public class CategoryController : Controller
    {
        private readonly AppDbContext _context;
        private readonly INotyfService _notyf;

        public CategoryController(AppDbContext context, INotyfService notyf)
        {
            _context = context;
            _notyf = notyf;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await _context.Categories.ToListAsync();
            return View(categories);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Category category)
        {
            if (ModelState.IsValid)
            {
                await _context.Categories.AddAsync(category);
                await _context.SaveChangesAsync();
                _notyf.Success("Kategori Eklendi");
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public async Task<IActionResult> Update(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Update(Category category)
        {
            if (ModelState.IsValid)
            {
                _context.Categories.Update(category);
                await _context.SaveChangesAsync();
                _notyf.Success("Kategori Güncellendi");
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Category model)
        {
            var category = await _context.Categories.FindAsync(model.Id);
            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            _notyf.Success("Kategori Silindi");
            return RedirectToAction("Index");
        }
    }
}