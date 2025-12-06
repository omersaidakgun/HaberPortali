using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prg1.Models;
using prg1.ViewModels; 

namespace prg1.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TodoController : Controller
    {
        private readonly AppDbContext _context;

        public TodoController(AppDbContext context)
        {
            _context = context;
        }

        
        public IActionResult Index()
        {
            return View();
        }

        
        public async Task<IActionResult> ListAjax()
        {
            var todos = await _context.Todos
                .OrderByDescending(x => x.Created) 
                .Select(x => new
                {
                    id = x.Id,
                    title = x.Title,
                    description = x.Description,
                    isOK = x.IsOK 
                })
                .ToListAsync();

            return Json(todos);
        }

        
        [HttpPost]
        public async Task<IActionResult> AddUpdateAjax(Todo model)
        {
            var result = new ResultModel();

            try
            {
                if (model.Id == 0) 
                {
                    model.Created = DateTime.Now;
                    model.IsActive = true;
                    await _context.Todos.AddAsync(model);
                    result.Message = "Kayıt Başarıyla Eklendi";
                }
                else 
                {
                    var todo = await _context.Todos.FindAsync(model.Id);
                    if (todo == null)
                    {
                        result.Status = false;
                        result.Message = "Kayıt Bulunamadı!";
                        return Json(result);
                    }

                    todo.Title = model.Title;
                    todo.Description = model.Description;
                    todo.IsOK = model.IsOK;
                    todo.Updated = DateTime.Now;

                    _context.Todos.Update(todo);
                    result.Message = "Kayıt Başarıyla Güncellendi";
                }

                await _context.SaveChangesAsync();
                result.Status = true;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Message = "Hata Oluştu: " + ex.Message;
            }

            return Json(result);
        }

        
        [HttpGet]
        public async Task<IActionResult> GetByIdAjax(int id)
        {
            var todo = await _context.Todos.FindAsync(id);
            return Json(todo);
        }

        
        [HttpGet]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            var result = new ResultModel();
            try
            {
                var todo = await _context.Todos.FindAsync(id);
                if (todo != null)
                {
                    _context.Todos.Remove(todo);
                    await _context.SaveChangesAsync();
                    result.Status = true;
                    result.Message = "Kayıt Silindi";
                }
                else
                {
                    result.Status = false;
                    result.Message = "Kayıt Bulunamadı";
                }
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.Message = "Hata: " + ex.Message;
            }

            return Json(result);
        }
    }
}