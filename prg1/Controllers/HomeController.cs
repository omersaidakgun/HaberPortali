using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using prg1.Models;
using prg1.ViewModels;

namespace prg1.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly INotyfService _notyf;
        private readonly AppDbContext _context;

        
        public HomeController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager, INotyfService notyf, AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _notyf = notyf;
            _context = context;
        }

        
        public IActionResult Index()
        {
            return View();
        }

        
        [Authorize(Roles = "Admin")]
        public IActionResult Panel()
        {
            return View();
        }

        
        [HttpGet]
        public IActionResult GetCounts()
        {
            var userCount = _userManager.Users.Count();
            var categoryCount = _context.Categories.Count();
            var todoCount = _context.Todos.Count();

            return Json(new { users = userCount, categories = categoryCount, todos = todoCount });
        }

        
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null)
            {
                _notyf.Error("Girilen Kullanıcı Adı Kayıtlı Değildir!");
                return View(model);
            }

            var signInResult = await _signInManager.PasswordSignInAsync(user, model.Password, model.KeepMe, true);

            if (signInResult.Succeeded)
            {
                
                return RedirectToAction("Index");
            }

            if (signInResult.IsLockedOut)
            {
                _notyf.Error("Hesap kilitlendi.");
                return View(model);
            }

            _notyf.Error("Kullanıcı Adı veya Parola Hatalı!");
            return View(model);
        }

       
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            
            string firstName = "";
            string lastName = "";

            if (!string.IsNullOrEmpty(model.FullName))
            {
                string[] names = model.FullName.Trim().Split(' ');
                firstName = names[0]; 
                
                lastName = names.Length > 1 ? string.Join(" ", names.Skip(1)) : "";
            }

            var user = new AppUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = firstName, 
                LastName = lastName,   
                ImagePath = "default.png"
            };

            var identityResult = await _userManager.CreateAsync(user, model.Password);

            if (!identityResult.Succeeded)
            {
                foreach (var item in identityResult.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                    _notyf.Error(item.Description);
                }
                return View(model);
            }

            
            if (!await _roleManager.RoleExistsAsync("Uye"))
            {
                await _roleManager.CreateAsync(new AppRole { Name = "Uye" });
            }

            
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new AppRole { Name = "Admin" });
            }

            
            await _userManager.AddToRoleAsync(user, "Uye");

            _notyf.Success("Üye Kaydı Yapılmıştır. Oturum Açınız");
            return RedirectToAction("Login");
        }

        
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}