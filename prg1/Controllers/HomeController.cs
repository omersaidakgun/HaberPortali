using AspNetCoreHero.ToastNotification.Abstractions;
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

        
        public HomeController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<AppRole> roleManager, INotyfService notyf)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _notyf = notyf;
        }

        public IActionResult Index()
        {
            return View();
        }

        
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.KeepMe, false);
                if (result.Succeeded)
                {
                    _notyf.Success("Giriş Başarılı");
                    return RedirectToAction("Index", "User"); 
                }
                else
                {
                    ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı!");
                    _notyf.Error("Giriş Başarısız");
                }
            }
            return View(model);
        }

        
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser();

                
                string[] names = model.FullName.Trim().Split(' ');
                user.FirstName = names[0];
                user.LastName = names.Length > 1 ? string.Join(" ", names.Skip(1)) : "";

                user.UserName = model.UserName;
                user.Email = model.Email;
                user.ImagePath = "no-img.png"; 

               
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    

                    
                    if (!await _roleManager.RoleExistsAsync("Admin"))
                        await _roleManager.CreateAsync(new AppRole { Name = "Admin" });

                    
                    if (!await _roleManager.RoleExistsAsync("Uye"))
                        await _roleManager.CreateAsync(new AppRole { Name = "Uye" });

                    
                    
                    if (_userManager.Users.Count() < 10 || user.UserName.ToLower().Contains("admin"))
                    {
                        await _userManager.AddToRoleAsync(user, "Admin");
                    }
                    else
                    {
                        
                        await _userManager.AddToRoleAsync(user, "Uye");
                    }

                    _notyf.Success("Kayıt Başarılı! Giriş yapabilirsiniz.");
                    return RedirectToAction("Login");
                }

                
                foreach (var item in result.Errors)
                {
                    ModelState.AddModelError("", item.Description);
                    _notyf.Error(item.Description);
                }
            }
            return View(model);
        }

        
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _notyf.Information("Çıkış Yapıldı");
            return RedirectToAction("Login");
        }

        
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}