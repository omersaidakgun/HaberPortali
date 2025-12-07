using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using prg1.Models;

[Authorize(Roles = "Admin")] 
public class AdminController : Controller
{
    private readonly AppDbContext _context; 

    public AdminController(AppDbContext context)
    {
        _context = context;
    }

    
    public IActionResult Index()
    {
        
        

        ViewBag.HaberSayisi = _context.News.Count();
        ViewBag.AktifHaberSayisi = _context.News.Where(x => x.IsActive).Count();
        ViewBag.UyeSayisi = _context.Users.Count();
        ViewBag.KategoriSayisi = _context.Categories.Count();

        return View();
    }
}