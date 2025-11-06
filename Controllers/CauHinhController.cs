using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Web_vuottai.Data;
using Web_vuottai.Models;

namespace Web_vuottai.Controllers
{
    public class CauHinhController : Controller
    {
        private readonly AppDbContext _db;

        public CauHinhController(AppDbContext db)
        {
            _db = db;
        }

        // Đơn giá giờ
        public async Task<IActionResult> DonGiaGio()
        {
            var data = await _db.DONGIAGIOs.Include(d => d.ChucDanh).ToListAsync();
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDonGia(int id, int donGia)
        {
            var dongiagio = await _db.DONGIAGIOs.FindAsync(id);
            if (dongiagio == null)
            {
                return NotFound();
            }

            dongiagio.DonGiaGio1 = donGia;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(DonGiaGio));
        }

        // Giờ chuẩn
        public async Task<IActionResult> GioChuan()
        {
            var data = await _db.CHUCDANH_GIOCHUANs.Include(d => d.ChucDanh).ToListAsync();
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateGioChuan(int id, int gioChuan)
        {
            var giochuan = await _db.CHUCDANH_GIOCHUANs.FindAsync(id);
            if (giochuan == null)
            {
                return NotFound();
            }

            giochuan.GioChuan = gioChuan;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(GioChuan));
        }
    }
}
