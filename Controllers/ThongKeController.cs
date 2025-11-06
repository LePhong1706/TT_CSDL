using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Globalization;
using Web_vuottai.Data;
using Web_vuottai.Models;
using Web_vuottai.Services.Pdf;   // <-- namespace chứa VuotTaiPdfDocument

namespace Web_vuottai.Controllers
{
    public class ThongKeController : Controller
    {
        private readonly AppDbContext _db;

        public ThongKeController(AppDbContext db)
        {
            _db = db;
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public IActionResult Index() => View();

        /// <summary>
        /// Xuất PDF theo bộ lọc (đơn vị / giảng viên / năm học / học kỳ)
        /// hocKy: "all" | "1" | "2"
        /// namHoc: "all" | "2024-2025" ...
        /// </summary>
        // THAY THẾ HÀM ExportVuotTaiPdf CŨ BẰNG HÀM NÀY:
        [HttpGet]
        public async Task<IActionResult> ExportVuotTaiPdf(
            int donViId = 0,
            int giangVienId = 0,
            string namHoc = "all",
            string hocKy = "all")
        {
            IQueryable<v_VuotTai_TongQuan> query = _db.v_VuotTai_TongQuans.AsNoTracking();

            // (Giữ nguyên logic lọc của bạn)
            if (donViId != 0)
            {
                query = from v in query
                        join g in _db.GIANGVIENs.AsNoTracking() on v.GiangVienId equals g.GiangVienId
                        where g.DonViId == donViId
                        select v;
            }

            if (giangVienId != 0)
                query = query.Where(v => v.GiangVienId == giangVienId);

            if (!string.Equals(namHoc, "all", StringComparison.OrdinalIgnoreCase))
                query = query.Where(v => v.NamHoc == namHoc);
            
            int? hocKyInt = null;
            if (!string.Equals(hocKy, "all", StringComparison.OrdinalIgnoreCase) &&
                int.TryParse(hocKy, out var hk))
            {
                query = query.Where(v => v.HocKy == hk);
                hocKyInt = hk; // Lưu lại để truyền vào PDF
            }

            // --- THAY ĐỔI BẮT ĐẦU TỪ ĐÂY ---

            // 1. Lấy dữ liệu đã lọc VÀ chỉ lấy ai có thanh toán
            var data = await query
                .Where(v => v.ThanhTien > 0) // Chỉ lấy GV có thanh toán
                .OrderBy(v => v.TenDonVi)    // Sắp xếp theo Đơn vị
                .ThenBy(v => v.HoTen)     // Rồi sắp xếp theo Tên
                .ToListAsync();

            // 2. Nhóm dữ liệu lại bằng C# (giống hệt file Excel)
            var groupedData = data
                .GroupBy(v => v.TenDonVi ?? "Không có đơn vị")
                .ToDictionary(g => g.Key, g => g.ToList());

            // 3. Tắt chế độ Debug
            QuestPDF.Settings.EnableDebugging = false;

            // 4. Truyền dữ liệu ĐÃ NHÓM và thông tin lọc vào Document
            string? namHocFilter = (namHoc == "all") ? null : namHoc;
            var doc = new VuotTaiPdfDocument(groupedData, namHocFilter, hocKyInt);
            
            var bytes = doc.GeneratePdf();

            return File(bytes, "application/pdf",
                $"BaoCaoThanhToan_{DateTime.Now:yyyyMMddHHmmss}.pdf");
        }

        // ====== API chart / filter ======
        [HttpGet]
        public async Task<JsonResult> GetVuotTaiTrendData()
        {
            var data = await _db.v_VuotTai_TongQuans
                .AsNoTracking()
                .GroupBy(v => v.NamHoc)
                .Select(g => new { NamHoc = g.Key, TongGioVuotTai = g.Sum(v => v.GioVuot) })
                .OrderBy(x => x.NamHoc)
                .ToListAsync();

            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetThanhToanTrendData()
        {
            var data = await _db.v_VuotTai_TongQuans
                .AsNoTracking()
                .GroupBy(v => v.NamHoc)
                .Select(g => new { NamHoc = g.Key, TongThanhTien = g.Sum(v => v.ThanhTien) })
                .OrderBy(x => x.NamHoc)
                .ToListAsync();

            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetVuotTaiByChucDanhData()
        {
            var data = await _db.v_VuotTai_TongQuans
                .AsNoTracking()
                .GroupBy(p => p.TenChucDanh)
                .Select(g => new { ChucDanh = g.Key, TongGioVuotTai = g.Sum(p => p.GioVuot) })
                .Where(x => x.TongGioVuotTai > 0)
                .OrderByDescending(x => x.TongGioVuotTai)
                .ToListAsync();

            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetVuotTaiByDonViData()
        {
            var data = await _db.v_VuotTai_TongQuans
                .AsNoTracking()
                .GroupBy(p => p.TenDonVi)
                .Select(g => new { DonVi = g.Key, TongGioVuotTai = g.Sum(p => p.GioVuot) })
                .Where(x => x.TongGioVuotTai > 0)
                .OrderByDescending(x => x.TongGioVuotTai)
                .ToListAsync();

            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetGioGiangByLoaiHocVienData()
        {
            var data = await _db.v_PhanCong_ChiTiets
                .AsNoTracking()
                .GroupBy(p => p.TenLoaiHV)
                .Select(g => new { LoaiHocVien = g.Key, TongGioQuyDoi = g.Sum(p => p.GioQuyDoi) })
                .ToListAsync();

            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetGioGiangByNgonNguData()
        {
            var data = await _db.v_PhanCong_ChiTiets
                .AsNoTracking()
                .GroupBy(p => p.TenNgonNgu)
                .Select(g => new { NgonNgu = g.Key, TongGioQuyDoi = g.Sum(p => p.GioQuyDoi) })
                .ToListAsync();

            return Json(data);
        }

        [HttpGet]
        public async Task<JsonResult> GetDistinctNamHoc()
        {
            var data = await _db.v_VuotTai_TongQuans
                .AsNoTracking()
                .Select(v => v.NamHoc)
                .Distinct()
                .OrderByDescending(nh => nh)
                .ToListAsync();

            return Json(data);
        }
    }
}
