using AspNetCoreHero.ToastNotification.Abstractions;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR; // SignalR kütüphanesi
using prg1.Hubs; // GeneralHub'ın olduğu yer (Burası önemli!)
using prg1.Models;
using prg1.Repositories;
using prg1.ViewModels;

namespace prg1.Controllers
{
    [Authorize]
    public class HaberController : Controller
    {
        private readonly HaberRepository _haberRepository;
        private readonly CategoryRepository _categoryRepository;
        private readonly INotyfService _notyf;
        private readonly IMapper _mapper;
        private readonly IHubContext<GeneralHub> _generalHub; // SignalR Bağlantısı

        public HaberController(HaberRepository haberRepository, CategoryRepository categoryRepository, INotyfService notyf, IMapper mapper, IHubContext<GeneralHub> generalHub)
        {
            _haberRepository = haberRepository;
            _categoryRepository = categoryRepository;
            _notyf = notyf;
            _mapper = mapper;
            _generalHub = generalHub; // Dependency Injection ile alıyoruz
        }

        public async Task<IActionResult> Index()
        {
            var haberler = await _haberRepository.GetAllAsync();
            var model = _mapper.Map<List<HaberModel>>(haberler);
            return View(model);
        }

        public async Task<IActionResult> Add()
        {
            ViewBag.Categories = new SelectList(await _categoryRepository.GetAllAsync(), "Id", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(HaberModel model, IFormFile? resim)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(await _categoryRepository.GetAllAsync(), "Id", "Name");
                return View(model);
            }

            string resimAdi = "no-image.png";
            if (resim != null)
            {
                string uzanti = Path.GetExtension(resim.FileName);
                resimAdi = Guid.NewGuid() + uzanti;
                string yol = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", resimAdi);
                using (var stream = new FileStream(yol, FileMode.Create))
                {
                    await resim.CopyToAsync(stream);
                }
            }

            var haber = _mapper.Map<Haber>(model);
            haber.ResimYolu = resimAdi;
            haber.Created = DateTime.Now;
            haber.Updated = DateTime.Now;

            await _haberRepository.AddAsync(haber);

            // SignalR: Tüm client'lara yeni haber sayısını gönder
            int haberCount = _haberRepository.Where(h => h.IsActive == true).Count();
            await _generalHub.Clients.All.SendAsync("onHaberAdd", haberCount);

            _notyf.Success("Haber Başarıyla Eklendi");
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Update(int id)
        {
            var haber = await _haberRepository.GetByIdAsync(id);
            var model = _mapper.Map<HaberModel>(haber);
            ViewBag.Categories = new SelectList(await _categoryRepository.GetAllAsync(), "Id", "Name", haber.CategoryId);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(HaberModel model, IFormFile? resim)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(await _categoryRepository.GetAllAsync(), "Id", "Name");
                return View(model);
            }

            var haber = await _haberRepository.GetByIdAsync(model.Id);

            if (resim != null)
            {
                string uzanti = Path.GetExtension(resim.FileName);
                string resimAdi = Guid.NewGuid() + uzanti;
                string yol = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", resimAdi);
                using (var stream = new FileStream(yol, FileMode.Create))
                {
                    await resim.CopyToAsync(stream);
                }
                haber.ResimYolu = resimAdi;
            }

            haber.Baslik = model.Baslik;
            haber.Icerik = model.Icerik;
            haber.CategoryId = model.CategoryId;
            haber.IsActive = model.IsActive;
            haber.Updated = DateTime.Now;

            await _haberRepository.UpdateAsync(haber);

            // SignalR: Güncelleme bildirimi (isteğe bağlı ama sayısı değişebilir diye ekledim)
            int haberCount = _haberRepository.Where(h => h.IsActive == true).Count();
            await _generalHub.Clients.All.SendAsync("onHaberUpdate", haberCount);

            _notyf.Success("Haber Güncellendi");
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var haber = await _haberRepository.GetByIdAsync(id);
            var model = _mapper.Map<HaberModel>(haber);
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(HaberModel model)
        {
            await _haberRepository.DeleteAsync(model.Id);

            // SignalR: Silinince sayıyı düşür
            int haberCount = _haberRepository.Where(h => h.IsActive == true).Count();
            await _generalHub.Clients.All.SendAsync("onHaberDelete", haberCount);

            _notyf.Success("Haber Silindi");
            return RedirectToAction("Index");
        }
    }
}