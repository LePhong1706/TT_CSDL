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
    public class MONHOCsController : Controller
    {
        private readonly AppDbContext _context;

        public MONHOCsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: MONHOCs
public async Task<IActionResult> Index(string? maMonFilter, string? tenMonFilter)
{
    // Gửi giá trị filter về View để hiển thị lại trên ô tìm kiếm
    ViewData["MaMonFilter"] = maMonFilter;
    ViewData["TenMonFilter"] = tenMonFilter;

    // Bắt đầu truy vấn (AsNoTracking để tăng hiệu suất đọc)
    var query = _context.MONHOCs.AsNoTracking();

    // Thêm điều kiện lọc nếu có
    if (!string.IsNullOrEmpty(maMonFilter))
    {
        // Thêm kiểm tra != null vì MaMonHoc là string? (nullable)
        query = query.Where(m => m.MaMonHoc != null && m.MaMonHoc.Contains(maMonFilter));
    }

    if (!string.IsNullOrEmpty(tenMonFilter))
    {
        // Thêm kiểm tra != null vì TenMonHoc là string? (nullable)
        query = query.Where(m => m.TenMonHoc != null && m.TenMonHoc.Contains(tenMonFilter));
    }

    // Thực thi truy vấn đã lọc và sắp xếp
    var data = await query.OrderBy(m => m.TenMonHoc).ToListAsync();
    
    return View(data);
}

        // GET: MONHOCs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mONHOC = await _context.MONHOCs
                .FirstOrDefaultAsync(m => m.MonHocId == id);
            if (mONHOC == null)
            {
                return NotFound();
            }

            return View(mONHOC);
        }

        // GET: MONHOCs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MONHOCs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MonHocId,MaMon,TenMonHoc")] MONHOC mONHOC)
        {
            if (ModelState.IsValid)
            {
                _context.Add(mONHOC);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(mONHOC);
        }

        // GET: MONHOCs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mONHOC = await _context.MONHOCs.FindAsync(id);
            if (mONHOC == null)
            {
                return NotFound();
            }
            return View(mONHOC);
        }

        // POST: MONHOCs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MonHocId,MaMon,TenMonHoc")] MONHOC mONHOC)
        {
            if (id != mONHOC.MonHocId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mONHOC);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MONHOCExists(mONHOC.MonHocId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(mONHOC);
        }

        // GET: MONHOCs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mONHOC = await _context.MONHOCs
                .FirstOrDefaultAsync(m => m.MonHocId == id);
            if (mONHOC == null)
            {
                return NotFound();
            }

            return View(mONHOC);
        }

        // POST: MONHOCs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mONHOC = await _context.MONHOCs.FindAsync(id);
            if (mONHOC != null)
            {
                _context.MONHOCs.Remove(mONHOC);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MONHOCExists(int id)
        {
            return _context.MONHOCs.Any(e => e.MonHocId == id);
        }
    }
}
