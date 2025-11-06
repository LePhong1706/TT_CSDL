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
    string? namHoc, 
    int? hocKy
)
{
    // --- 1. Load data cho Dropdowns (từ chính View) ---
    var baseQuery = _db.v_VuotTai_TongQuans.AsNoTracking();

    // Lấy danh sách Năm học
    var namHocListItems = await baseQuery
        .Select(v => v.NamHoc)
        .Where(n => n != null).Distinct()
        .OrderByDescending(n => n)
        .Select(n => new SelectListItem { Value = n, Text = n })
        .ToListAsync();
    namHocListItems.Insert(0, new SelectListItem { Value = "", Text = "--- Tất cả Năm học ---" });
    ViewBag.NamHocList = new SelectList(namHocListItems, "Value", "Text", namHoc);

    // Lấy danh sách Chức danh
    var chucDanhListItems = await baseQuery
        .Select(v => v.TenChucDanh)
        .Where(n => n != null).Distinct()
        .OrderBy(n => n)
        .Select(n => new SelectListItem { Value = n, Text = n })
        .ToListAsync();
    chucDanhListItems.Insert(0, new SelectListItem { Value = "", Text = "--- Tất cả Chức danh ---" });
    ViewBag.ChucDanhList = new SelectList(chucDanhListItems, "Value", "Text", chucDanh);

    // Lấy danh sách Chức vụ
    var chucVuListItems = await baseQuery
        .Select(v => v.TenChucVu)
        .Where(n => n != null).Distinct()
        .OrderBy(n => n)
        .Select(n => new SelectListItem { Value = n, Text = n })
        .ToListAsync();
    chucVuListItems.Insert(0, new SelectListItem { Value = "", Text = "--- Tất cả Chức vụ ---" });
    ViewBag.ChucVuList = new SelectList(chucVuListItems, "Value", "Text", chucVu);


    // --- 2. Gửi các giá trị filter đã chọn về View ---
    ViewData["HoTenFilter"] = hoTen;
    ViewBag.HocKy = hocKy; // Dùng cho dropdown Học kỳ


    // --- 3. Bắt đầu truy vấn và áp dụng lọc ---
    var query = baseQuery;

    if (!string.IsNullOrEmpty(hoTen))
    {
        query = query.Where(s => s.HoTen != null && s.HoTen.Contains(hoTen));
    }
    if (!string.IsNullOrEmpty(chucDanh))
    {
        query = query.Where(s => s.TenChucDanh == chucDanh);
    }
    if (!string.IsNullOrEmpty(chucVu))
    {
        query = query.Where(s => s.TenChucVu == chucVu);
    }
    if (!string.IsNullOrEmpty(namHoc))
    {
        query = query.Where(s => s.NamHoc == namHoc);
    }
    if (hocKy.HasValue)
    {
        query = query.Where(s => s.HocKy == hocKy.Value);
    }

    // --- 4. Thực thi truy vấn ---
    var data = await query.OrderBy(v => v.HoTen).ToListAsync();
    return View(data);
}
    }
}
