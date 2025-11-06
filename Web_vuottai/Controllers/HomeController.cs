using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Web_vuottai.Models;
using Web_vuottai.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering; // <-- Thêm using này

namespace Web_vuottai.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;

        public HomeController(AppDbContext db)
        {
            _db = db;
        }

        // Sửa: Thêm tham số namHoc và hocKy (nullable int)
        public async Task<IActionResult> Index(string? namHoc, int? hocKy)
        {
            // --- 1. Chuẩn bị dữ liệu cho Dropdown Lọc ---

            // Lấy danh sách Năm học từ CSDL
            var namHocOptions = await _db.v_VuotTai_TongQuans
                .Select(v => v.NamHoc)
                .Where(n => n != null) // Lọc bỏ NamHoc bị null nếu có
                .Distinct()
                .OrderByDescending(n => n)
                .ToListAsync();

            // Tạo SelectList cho Năm học, thêm lựa chọn "Tất cả"
            var namHocList = namHocOptions
                .Select(n => new SelectListItem { Value = n, Text = n })
                .ToList();
            namHocList.Insert(0, new SelectListItem { Value = "", Text = "--- Tất cả Năm học ---" });

            // Tạo SelectList cho Học kỳ, thêm lựa chọn "Tất cả"
            var hocKyList = new List<SelectListItem>
            {
                new SelectListItem { Value = "", Text = "--- Tất cả Học kỳ ---" },
                new SelectListItem { Value = "1", Text = "Học kỳ 1" },
                new SelectListItem { Value = "2", Text = "Học kỳ 2" }
            };

            // Gửi danh sách dropdown và giá trị đã chọn ra View
            ViewBag.NamHocs = new SelectList(namHocList, "Value", "Text", namHoc);
            ViewBag.HocKys = new SelectList(hocKyList, "Value", "Text", hocKy);


            // --- 2. Chuẩn bị các truy vấn cơ sở (IQueryable) ---
            
            var vuotTaiQuery = _db.v_VuotTai_TongQuans.AsNoTracking();
            var phanCongQuery = _db.v_PhanCong_ChiTiets.AsNoTracking();

            string filterTitle = "toàn bộ CSDL"; // Tiêu đề mặc định

            // --- 3. Áp dụng bộ lọc (nếu có) ---

            if (!string.IsNullOrEmpty(namHoc))
            {
                vuotTaiQuery = vuotTaiQuery.Where(v => v.NamHoc == namHoc);
                phanCongQuery = phanCongQuery.Where(p => p.NamHoc == namHoc);
                filterTitle = $"năm học {namHoc}";
            }

            if (hocKy.HasValue) // Nếu hocKy có giá trị (không phải null)
            {
                vuotTaiQuery = vuotTaiQuery.Where(v => v.HocKy == hocKy.Value);
                phanCongQuery = phanCongQuery.Where(p => p.HocKy == hocKy.Value);
                
                // Cập nhật tiêu đề
                if (filterTitle == "toàn bộ CSDL") filterTitle = $"kỳ {hocKy.Value}";
                else filterTitle += $" - Kỳ {hocKy.Value}";
            }
            
            ViewBag.TermInfo = filterTitle; // Gửi tiêu đề ra View

            var vm = new DashboardViewModel();

            // --- 4. Thực thi truy vấn ĐÃ LỌC ---

            // Tính KPI cards (KPIs toàn hệ thống không lọc)
            vm.TongGiangVien = await _db.GIANGVIENs.CountAsync();
            vm.TongLop = await _db.LOPs.CountAsync();
            
            // Tính KPIs vượt tải (dựa trên truy vấn đã lọc)
            vm.TongGioVuot = await vuotTaiQuery.SumAsync(v => v.GioVuot) ?? 0m;
            vm.TongThanhTien = await vuotTaiQuery.SumAsync(v => v.ThanhTien) ?? 0m;


            // Biểu đồ Cột (Top 5 GV) (dựa trên truy vấn đã lọc)
            var topGv = await vuotTaiQuery
                .Where(v => v.GioVuot > 0) 
                .OrderByDescending(v => v.GioVuot)
                .Take(5)
                .Select(v => new { v.HoTen, v.GioVuot })
                .ToListAsync();

            vm.TopVuotTaiChart.Labels = topGv.Select(g => g.HoTen ?? "Không tên").ToList();
            vm.TopVuotTaiChart.Data = topGv.Select(g => g.GioVuot ?? 0m).ToList();


            // Biểu đồ Tròn (Giờ giảng theo Đơn vị) (dựa trên truy vấn đã lọc)
            var gioTheoDonVi = await phanCongQuery
                .GroupBy(p => p.TenDonVi) 
                .Select(g => new
                {
                    DonVi = g.Key ?? "Chưa phân loại",
                    TongGio = g.Sum(p => p.GioQuyDoi)
                })
                .Where(g => g.TongGio > 0)
                .ToListAsync();

            vm.DonViChart.Labels = gioTheoDonVi.Select(d => d.DonVi).ToList();
            vm.DonViChart.Data = gioTheoDonVi.Select(d => d.TongGio ?? 0m).ToList();

            return View(vm); // Gửi ViewModel đã có dữ liệu (lọc hoặc toàn bộ) ra View
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}