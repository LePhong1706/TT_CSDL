using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Web_vuottai.Data;
using Web_vuottai.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace Web_vuottai.Controllers
{
    public class VuotTaiTongQuanController : Controller
    {
        private readonly AppDbContext _db;
        public VuotTaiTongQuanController(AppDbContext db) => _db = db;
        // (Bên trong class VuotTaiTongQuanController)

// THAY THẾ HÀM INDEX CŨ BẰNG HÀM NÀY
public async Task<IActionResult> Index(
    string? hoTen,
    string? chucDanh,
    string? chucVu,
    string? donVi, // Thêm tham số lọc mới
    string? namHoc,
    int? hocKy
)
{
    // --- 1. Load data cho Dropdowns ---
    var baseQuery = _db.v_VuotTai_TongQuans.AsNoTracking();

    // ... (Các dropdown khác giữ nguyên)
    var namHocListItems = await baseQuery.Select(v => v.NamHoc).Where(n => n != null).Distinct().OrderByDescending(n => n).Select(n => new SelectListItem { Value = n, Text = n }).ToListAsync();
    namHocListItems.Insert(0, new SelectListItem { Value = "", Text = "--- Tất cả Năm học ---" });
    ViewBag.NamHocList = new SelectList(namHocListItems, "Value", "Text", namHoc);

    var chucDanhListItems = await baseQuery.Select(v => v.TenChucDanh).Where(n => n != null).Distinct().OrderBy(n => n).Select(n => new SelectListItem { Value = n, Text = n }).ToListAsync();
    chucDanhListItems.Insert(0, new SelectListItem { Value = "", Text = "--- Tất cả Chức danh ---" });
    ViewBag.ChucDanhList = new SelectList(chucDanhListItems, "Value", "Text", chucDanh);

    var chucVuListItems = await baseQuery.Select(v => v.TenChucVu).Where(n => n != null).Distinct().OrderBy(n => n).Select(n => new SelectListItem { Value = n, Text = n }).ToListAsync();
    chucVuListItems.Insert(0, new SelectListItem { Value = "", Text = "--- Tất cả Chức vụ ---" });
    ViewBag.ChucVuList = new SelectList(chucVuListItems, "Value", "Text", chucVu);

    // MỚI: Lấy danh sách Đơn vị
    var donViListItems = await baseQuery
        .Select(v => v.TenDonVi)
        .Where(n => n != null).Distinct()
        .OrderBy(n => n)
        .Select(n => new SelectListItem { Value = n, Text = n })
        .ToListAsync();
    donViListItems.Insert(0, new SelectListItem { Value = "", Text = "--- Tất cả Đơn vị ---" });
    ViewBag.DonViList = new SelectList(donViListItems, "Value", "Text", donVi);


    ViewData["HoTenFilter"] = hoTen;
    ViewBag.HocKy = hocKy;

    // --- 2. Áp dụng lọc ---
    var query = baseQuery;
    if (!string.IsNullOrEmpty(hoTen)) query = query.Where(s => s.HoTen != null && s.HoTen.Contains(hoTen));
    if (!string.IsNullOrEmpty(chucDanh)) query = query.Where(s => s.TenChucDanh == chucDanh);
    if (!string.IsNullOrEmpty(chucVu)) query = query.Where(s => s.TenChucVu == chucVu);
    if (!string.IsNullOrEmpty(donVi)) query = query.Where(s => s.TenDonVi == donVi); // Áp dụng lọc đơn vị
    if (!string.IsNullOrEmpty(namHoc)) query = query.Where(s => s.NamHoc == namHoc);
    if (hocKy.HasValue) query = query.Where(s => s.HocKy == hocKy.Value);

    // --- 3. Thực thi truy vấn và lấy danh sách chi tiết ---
    var filteredData = await query.OrderBy(v => v.HoTen).ToListAsync();

    // --- 4. Tính toán KPIs và chuẩn bị ViewModel (giữ nguyên) ---
    var viewModel = new DashboardViewModel
    {
        VuotTaiList = filteredData,
        TongGioVuot = filteredData.Sum(x => x.GioVuot ?? 0),
        TongThanhTien = filteredData.Sum(x => x.ThanhTien ?? 0),
        SoGiangVienVuotTai = filteredData.Where(x => x.GioVuot > 0).Select(x => x.GiangVienId).Distinct().Count(),
        DonViChart = new ChartData
        {
            Labels = filteredData.GroupBy(x => x.TenDonVi)
                                .Select(g => g.Key ?? "Chưa xác định")
                                .ToList(),
            Data = filteredData.GroupBy(x => x.TenDonVi)
                               .Select(g => g.Sum(s => s.GioVuot ?? 0))
                               .ToList()
        }
    };

    // --- 5. Trả về View với ViewModel mới ---
    return View(viewModel);
}
    }
}
