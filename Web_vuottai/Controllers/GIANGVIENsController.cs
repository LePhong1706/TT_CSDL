using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web_vuottai.Data;
using Web_vuottai.Models;

namespace Web_vuottai.Controllers
{
    public class GIANGVIENsController : Controller
    {
        private readonly AppDbContext _context;

        public GIANGVIENsController(AppDbContext context)
        {
            _context = context;
        }


        // (Bên trong class GIANGVIENsController)

public async Task<IActionResult> Index(
    string? namHoc, 
    int? hocKy,
    string? hoTen,      // Filter mới
    int? donViId,    // Filter mới
    int? chucVuId,   // Filter mới
    int? chucDanhId  // Filter mới
)
{
    // --- 1. Load data cho các Dropdown (Filters) ---

    // Load Năm học (giữ nguyên logic cũ của bạn)
    var years = await _context.THOIKHOABIEUs.Select(t => t.NamHoc)
                            .Union(_context.GV_CHUCDANHs.Select(t => t.NamHoc)).Distinct()
                            .OrderByDescending(x => x).ToListAsync();
    if (years.Count == 0) years = new List<string> { "2024-2025", "2025-2026", "2026-2027" };
    if (string.IsNullOrWhiteSpace(namHoc)) namHoc = years.First();
    ViewBag.NamHocs = new SelectList(years, namHoc);

    // Load các Dropdown mới (Đơn vị, Chức vụ, Chức danh)
    ViewBag.DonViList = new SelectList(await _context.DONVIs.OrderBy(d => d.TenDonVi).ToListAsync(), "DonViId", "TenDonVi", donViId);
    ViewBag.ChucVuList = new SelectList(await _context.CHUCVUs.OrderBy(c => c.TenChucVu).ToListAsync(), "ChucVuId", "TenChucVu", chucVuId);
    ViewBag.ChucDanhList = new SelectList(await _context.CHUCDANHs.OrderBy(c => c.TenChucDanh).ToListAsync(), "ChucDanhId", "TenChucDanh", chucDanhId);

    // --- 2. Gửi các giá trị filter đã chọn về View ---
    ViewBag.NamHoc = namHoc;
    ViewBag.HocKy = hocKy;
    ViewBag.HoTen = hoTen;
    ViewBag.DonViId = donViId;
    ViewBag.ChucVuId = chucVuId;
    ViewBag.ChucDanhId = chucDanhId;

    // --- 3. Xây dựng truy vấn (Query) ---

    // Bắt đầu với IQueryable<GIANGVIEN>
    var query = _context.GIANGVIENs.AsQueryable();

    // Áp dụng các bộ lọc đơn giản (từ bảng GIANGVIEN)
    if (!string.IsNullOrEmpty(hoTen))
    {
        query = query.Where(g => g.HoTen.Contains(hoTen));
    }
    if (donViId.HasValue)
    {
        query = query.Where(g => g.DonViId == donViId.Value);
    }
    if (chucVuId.HasValue)
    {
        query = query.Where(g => g.ChucVuId == chucVuId.Value);
    }

    // --- 4. Thực hiện Join và Select (Phức tạp) ---
    // (Giữ nguyên logic JOIN của bạn, nhưng áp dụng 'query' đã lọc ở trên)
    var resultQuery =
        from g in query // <-- Dùng 'query' (đã lọc) thay vì '_context.GIANGVIENs'
            .Include(x => x.DonVi)
            .Include(x => x.ChucVu)
        // LEFT JOIN mapping theo năm/kỳ
        join map in _context.GV_CHUCDANHs on g.GiangVienId equals map.GiangVienId
            into gj
        from m in gj.Where(x => x.NamHoc == namHoc
                             && (!hocKy.HasValue || x.HocKy == hocKy.Value))
                       .DefaultIfEmpty()
        // LEFT JOIN sang CHUCDANH
        join cd in _context.CHUCDANHs on m.ChucDanhId equals cd.ChucDanhId into cdj
        from chucDanh in cdj.DefaultIfEmpty()
        
        // Áp dụng bộ lọc cuối cùng (cho Chức danh)
        where !chucDanhId.HasValue || (m != null && m.ChucDanhId == chucDanhId.Value)

        select new GiangVienListVm
        {
            GiangVienId = g.GiangVienId,
            MaGV = g.MaGV,
            HoTen = g.HoTen,
            TenDonVi = g.DonVi != null ? g.DonVi.TenDonVi : null,
            TenChucVu = g.ChucVu != null ? g.ChucVu.TenChucVu : null,
            TenChucDanh = chucDanh != null ? chucDanh.TenChucDanh : "(chưa gán)",
            NamHoc = namHoc,
            HocKy = hocKy
        };

    var list = await resultQuery.OrderBy(x => x.HoTen).ToListAsync();

    return View(list);
}


        // ====== CREATE ======
        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();
            return View(new GiangVienUpsertVm());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GiangVienUpsertVm m)
        {
            if (!ModelState.IsValid) { await LoadDropdowns(); return View(m); }

            await using var tx = await _context.Database.BeginTransactionAsync();

            var gv = new GIANGVIEN
            {
                MaGV = m.MaGV.Trim(),
                HoTen = m.HoTen.Trim(),
                DonViId = m.DonViId,
                ChucVuId = m.ChucVuId
            };
            _context.Add(gv);
            await _context.SaveChangesAsync(); // có GiangVienId

            // Upsert GV_CHUCDANH
            var existed = await _context.GV_CHUCDANHs
                .FirstOrDefaultAsync(x => x.GiangVienId == gv.GiangVienId
                                       && x.NamHoc == m.NamHoc && x.HocKy == m.HocKy);
            if (existed == null)
                _context.Add(new GV_CHUCDANH
                {
                    GiangVienId = gv.GiangVienId,
                    ChucDanhId = m.ChucDanhId,
                    NamHoc = m.NamHoc,
                    HocKy = m.HocKy
                });
            else
                existed.ChucDanhId = m.ChucDanhId;

            await _context.SaveChangesAsync();

            // Tính lại vượt tải (để GioChuan đúng ngay nếu đã có PC/CT)
            await _context.Database.ExecuteSqlInterpolatedAsync($@"
            EXEC dbo.sp_Recalc_VuotTai_1GV 
                 @GiangVienId={gv.GiangVienId}, @NamHoc={m.NamHoc}, @HocKy={m.HocKy}");

            await tx.CommitAsync();
            return RedirectToAction(nameof(Index), new { namHoc = m.NamHoc, hocKy = m.HocKy });
        }

        // ====== EDIT ======
        public async Task<IActionResult> Edit(int id, string? namHoc, int? hocKy)
        {
            var gv = await _context.GIANGVIENs.FindAsync(id);
            if (gv == null) return NotFound();

            // lấy mapping hiện hành (nếu có)
            var map = await _context.GV_CHUCDANHs
                .FirstOrDefaultAsync(x => x.GiangVienId == id
                                       && x.NamHoc == (namHoc ?? "2024-2025")
                                       && x.HocKy == (hocKy ?? 1));

            var vm = new GiangVienUpsertVm
            {
                GiangVienId = id,
                MaGV = gv.MaGV,
                HoTen = gv.HoTen,
                DonViId = gv.DonViId,
                ChucVuId = gv.ChucVuId,
                ChucDanhId = map?.ChucDanhId ?? 0,
                NamHoc = namHoc ?? "2024-2025",
                HocKy = hocKy ?? 1
            };

            await LoadDropdowns();
            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(GiangVienUpsertVm m)
        {
            if (!ModelState.IsValid) { await LoadDropdowns(); return View(m); }

            await using var tx = await _context.Database.BeginTransactionAsync();

            var gv = await _context.GIANGVIENs.FindAsync(m.GiangVienId);
            if (gv == null) return NotFound();

            gv.MaGV = m.MaGV.Trim();
            gv.HoTen = m.HoTen.Trim();
            gv.DonViId = m.DonViId;
            gv.ChucVuId = m.ChucVuId;
            await _context.SaveChangesAsync();

            var map = await _context.GV_CHUCDANHs
                .FirstOrDefaultAsync(x => x.GiangVienId == gv.GiangVienId
                                       && x.NamHoc == m.NamHoc && x.HocKy == m.HocKy);
            if (map == null)
                _context.Add(new GV_CHUCDANH
                {
                    GiangVienId = gv.GiangVienId,
                    ChucDanhId = m.ChucDanhId,
                    NamHoc = m.NamHoc,
                    HocKy = m.HocKy
                });
            else
                map.ChucDanhId = m.ChucDanhId;

            await _context.SaveChangesAsync();

            await _context.Database.ExecuteSqlInterpolatedAsync($@"
            EXEC dbo.sp_Recalc_VuotTai_1GV 
                 @GiangVienId={gv.GiangVienId}, @NamHoc={m.NamHoc}, @HocKy={m.HocKy}");

            await tx.CommitAsync();
            return RedirectToAction(nameof(Index), new { namHoc = m.NamHoc, hocKy = m.HocKy });
        }
// GET: GIANGVIENs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gIANGVIEN = await _context.GIANGVIENs
                .Include(g => g.ChucVu)
                .Include(g => g.DonVi)
                .FirstOrDefaultAsync(m => m.GiangVienId == id);
                
            if (gIANGVIEN == null)
            {
                return NotFound();
            }

            return View(gIANGVIEN); // Trả về trang Delete.cshtml
        }
// POST: GIANGVIENs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gIANGVIEN = await _context.GIANGVIENs.FindAsync(id);
            if (gIANGVIEN == null)
            {
                return NotFound();
            }

            try
            {
                // Trước khi xóa GV, phải xóa các bản ghi liên quan (nếu bạn muốn xóa triệt để)
                // Hoặc bạn chỉ cần báo lỗi (như cách dưới đây)

                // Kiểm tra ràng buộc
                bool coPhanCong = await _context.PHANCONGs.AnyAsync(p => p.GiangVienId == id);
                bool coChucDanh = await _context.GV_CHUCDANHs.AnyAsync(g => g.GiangVienId == id);
                bool coVuotTai = await _context.VUOTTAI_SUMs.AnyAsync(v => v.GiangVienId == id);
                
                if (coPhanCong || coChucDanh || coVuotTai)
                {
                    // Nếu có ràng buộc, thêm lỗi vào ModelState và quay lại trang xác nhận
                    ModelState.AddModelError(string.Empty, "Không thể xóa giảng viên này. Giảng viên đã có dữ liệu phân công, lịch sử chức danh hoặc dữ liệu vượt tải.");
                    
                    // Nạp lại dữ liệu liên quan để hiển thị trang Delete
                    await _context.Entry(gIANGVIEN).Reference(g => g.ChucVu).LoadAsync();
                    await _context.Entry(gIANGVIEN).Reference(g => g.DonVi).LoadAsync();
                    return View(gIANGVIEN); // Trả về trang Delete với thông báo lỗi
                }

                // Nếu không còn ràng buộc, tiến hành xóa
                _context.GIANGVIENs.Remove(gIANGVIEN);
                await _context.SaveChangesAsync();
                
                TempData["ok"] = "Đã xóa giảng viên thành công."; // Thông báo thành công
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException /* ex */)
            {
                // Bắt lỗi chung từ CSDL (nếu logic kiểm tra ở trên bị sót)
                ModelState.AddModelError(string.Empty, "Đã xảy ra lỗi. Không thể xóa giảng viên này do có các bản ghi liên quan (Phân công, Chức danh...).");
                
                await _context.Entry(gIANGVIEN).Reference(g => g.ChucVu).LoadAsync();
                await _context.Entry(gIANGVIEN).Reference(g => g.DonVi).LoadAsync();
                return View(gIANGVIEN);
            }
        }

        // GET: GIANGVIENs/GetByDonVi?donViId=5
        [HttpGet]
        public async Task<IActionResult> GetByDonVi(int donViId)
        {
            var giangViens = await _context.GIANGVIENs
                .Where(g => g.DonViId == donViId)
                .Select(g => new { g.GiangVienId, g.HoTen, g.MaGV })
                .OrderBy(g => g.HoTen)
                .ToListAsync();
            return Json(giangViens);
        }

        // ====== dropdown helpers ======
        private async Task LoadDropdowns(int? selectedDonViId = null, int? selectedChucVuId = null, int? selectedChucDanhId = null,
        string? selectedNamHoc = null)
        {
            ViewBag.DonViList = new SelectList(
                await _context.DONVIs.AsNoTracking().OrderBy(x => x.TenDonVi).ToListAsync(),
                "DonViId", "TenDonVi");

            ViewBag.ChucVuList = new SelectList(
           await _context.CHUCVUs.AsNoTracking().OrderBy(x => x.TenChucVu).ToListAsync(),
           "ChucVuId", "TenChucVu");

            ViewBag.ChucDanhList = new SelectList(
                await _context.CHUCDANHs.AsNoTracking().OrderBy(x => x.TenChucDanh).ToListAsync(),
                "ChucDanhId", "TenChucDanh");

            var years = await _context.THOIKHOABIEUs.Select(t => t.NamHoc).Distinct()
                          .OrderByDescending(x => x).ToListAsync();
            if (years.Count == 0) years = new List<string> { "2024-2025", "2025-2026", "2026-2027" };
            ViewBag.NamHocList = new SelectList(years, selectedNamHoc ?? years.First());
        }

        public async Task<IActionResult> Details(int id, string? namHoc, int? hocKy)
        {
            namHoc ??= "2024-2025"; hocKy ??= 1;

            var gv = await _context.GIANGVIENs
                .Include(x => x.DonVi)
                .Include(x => x.ChucVu)
                .FirstOrDefaultAsync(x => x.GiangVienId == id);
            if (gv == null) return NotFound();

            var cd = await (from m in _context.GV_CHUCDANHs
                            join c in _context.CHUCDANHs on m.ChucDanhId equals c.ChucDanhId
                            where m.GiangVienId == id && m.NamHoc == namHoc && m.HocKy == hocKy
                            select c.TenChucDanh).FirstOrDefaultAsync();

            ViewBag.TenChucDanh = cd ?? "(chưa gán)";

            // Lấy nhanh số liệu kỳ này từ view tổng quan
            var tongQuan = await _context.Set<v_VuotTai_TongQuan>()   // map keyless cho view dbo.v_VuotTai_TongQuan
                .FromSqlInterpolated($@"SELECT * FROM dbo.v_VuotTai_TongQuan 
                                 WHERE GiangVienId={id} AND NamHoc={namHoc} AND HocKy={hocKy}")
                .AsNoTracking().FirstOrDefaultAsync();
            ViewBag.GioChuan = tongQuan?.GioChuan ?? 0;
            ViewBag.GioThucTe = tongQuan?.GioThucTe ?? 0;
            ViewBag.GioVuot = tongQuan?.GioVuot ?? 0;

            return View(gv);
        }
    }

}
