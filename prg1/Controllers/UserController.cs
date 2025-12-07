using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;
using prg1.Models;
using prg1.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

namespace prg1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly IMapper _mapper;
        private readonly INotyfService _notyf;
        private readonly IFileProvider _fileProvider;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public UserController(IMapper mapper, INotyfService notyf, IFileProvider fileProvider, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _mapper = mapper;
            _notyf = notyf;
            _fileProvider = fileProvider;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            
            var userModels = users.Select(x => new UserModel
            {
                Id = x.Id,
                FullName = x.FirstName + " " + x.LastName, 
                UserName = x.UserName,
                Email = x.Email,
                Role = "Uye", 
                ImagePath = x.ImagePath
            }).ToList();

            return View(userModels);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(UserModel model)
        {
            ModelState.Remove("Id");
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new AppUser();

            
            string[] names = model.FullName.Trim().Split(' ');
            user.FirstName = names[0]; 
            
            user.LastName = names.Length > 1 ? string.Join(" ", names.Skip(1)) : "";

            user.UserName = model.UserName;
            user.Email = model.Email;
            user.ImagePath = "no-img.png";

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

            
            var roleExist = await _roleManager.RoleExistsAsync("Uye");
            if (!roleExist)
            {
                var role = new AppRole { Name = "Uye" };
                await _roleManager.CreateAsync(role);
            }

            await _userManager.AddToRoleAsync(user, "Uye");
            _notyf.Success("Üye Başarıyla Eklendi");

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Update(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var model = new UserModel
            {
                Id = user.Id,
                FullName = user.FirstName + " " + user.LastName, 
                UserName = user.UserName,
                Email = user.Email,
                
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UserModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);

            
            string[] names = model.FullName.Trim().Split(' ');
            user.FirstName = names[0];
            user.LastName = names.Length > 1 ? string.Join(" ", names.Skip(1)) : "";

            user.UserName = model.UserName;
            user.Email = model.Email;
            

            await _userManager.UpdateAsync(user);
            _notyf.Success("Üye Güncellendi");
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            
            var model = new UserModel
            {
                Id = user.Id,
                FullName = user.FirstName + " " + user.LastName,
                UserName = user.UserName
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(UserModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);

            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                _notyf.Error("Yönetici Üye Silinemez!");
                return RedirectToAction("Index");
            }
            await _userManager.DeleteAsync(user);
            _notyf.Success("Üye Silindi");
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Profile()
        {
            var userName = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(userName);

            
            var model = new RegisterModel
            {
                FullName = user.FirstName + " " + user.LastName,
                UserName = user.UserName,
                Email = user.Email
                
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Profile(RegisterModel model)
        {
            var userName = User.Identity.Name;
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == userName);

            if (model.Password != model.PasswordConfirm)
            {
                ModelState.AddModelError("", "Parolalar uyuşmuyor.");
                return View(model);
            }

            
            string[] names = model.FullName.Trim().Split(' ');
            user.FirstName = names[0];
            user.LastName = names.Length > 1 ? string.Join(" ", names.Skip(1)) : "";

            user.UserName = model.UserName;
            user.Email = model.Email;

            
            if (model.PhotoFile != null)
            {
                
                
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "userphotos");

                
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                
                var filename = Guid.NewGuid().ToString() + Path.GetExtension(model.PhotoFile.FileName);
                var photoPath = Path.Combine(folderPath, filename);

                
                using (var stream = new FileStream(photoPath, FileMode.Create))
                {
                    await model.PhotoFile.CopyToAsync(stream);
                }

                user.ImagePath = filename;
            }
            

            var result = await _userManager.UpdateAsync(user);

            if (!string.IsNullOrEmpty(model.Password))
            {
                await _userManager.RemovePasswordAsync(user);
                await _userManager.AddPasswordAsync(user, model.Password);
            }

            if (result.Succeeded)
            {
                _notyf.Success("Profiliniz güncellendi.");
                return RedirectToAction("Profile");
            }
            else
            {
                _notyf.Error("Bir hata oluştu.");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> UserRole(string id)
        {
            
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return RedirectToAction("Index");

            
            var roles = _roleManager.Roles.ToList();

            
            var userRoles = await _userManager.GetRolesAsync(user);

            
            List<UserRoleModel> model = new List<UserRoleModel>();

            foreach (var role in roles)
            {
                UserRoleModel m = new UserRoleModel();
                m.RoleId = role.Id;
                m.RoleName = role.Name;

                
                m.IsSelected = userRoles.Contains(role.Name);

                model.Add(m);
            }

            ViewBag.UserId = user.Id;
            ViewBag.UserName = user.UserName ?? user.FirstName; 

            return View(model);
        }

        
        [HttpPost]
        public async Task<IActionResult> UserRole(List<UserRoleModel> model, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            foreach (var item in model)
            {
                if (item.IsSelected)
                {
                    
                    await _userManager.AddToRoleAsync(user, item.RoleName);
                }
                else
                {
                    
                    await _userManager.RemoveFromRoleAsync(user, item.RoleName);
                }
            }

            return RedirectToAction("Index");
        }








    }
}