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
    public class LOPsController : Controller
    {
        private readonly AppDbContext _context;

        public LOPsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: LOPs
        // (Bên trong class LOPsController)

// GET: LOPs
public async Task<IActionResult> Index(string? maLopFilter, string? tenLopFilter)
{
    // Gửi giá trị filter về View để hiển thị lại trên ô tìm kiếm
    ViewData["MaLopFilter"] = maLopFilter;
    ViewData["TenLopFilter"] = tenLopFilter;

    // Bắt đầu truy vấn (AsNoTracking để tăng hiệu suất đọc)
    var query = _context.LOPs.AsNoTracking();

    // Thêm điều kiện lọc nếu có
    if (!string.IsNullOrEmpty(maLopFilter))
    {
        // Thêm kiểm tra != null vì MaLop có thể null trong Model C#
        query = query.Where(l => l.MaLop != null && l.MaLop.Contains(maLopFilter));
    }

    if (!string.IsNullOrEmpty(tenLopFilter))
    {
        query = query.Where(l => l.TenLop.Contains(tenLopFilter));
    }

    // Thực thi truy vấn đã lọc và sắp xếp
    var data = await query.OrderBy(l => l.TenLop).ToListAsync();
    
    return View(data);
}

        // GET: LOPs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lOP = await _context.LOPs
                .FirstOrDefaultAsync(m => m.LopId == id);
            if (lOP == null)
            {
                return NotFound();
            }

            return View(lOP);
        }

        // GET: LOPs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LOPs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LopId,MaLop,TenLop,QuanSo")] LOP lOP)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lOP);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(lOP);
        }

        // GET: LOPs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lOP = await _context.LOPs.FindAsync(id);
            if (lOP == null)
            {
                return NotFound();
            }
            return View(lOP);
        }

        // POST: LOPs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LopId,MaLop,TenLop,QuanSo")] LOP lOP)
        {
            if (id != lOP.LopId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lOP);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LOPExists(lOP.LopId))
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
            return View(lOP);
        }

        // GET: LOPs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lOP = await _context.LOPs
                .FirstOrDefaultAsync(m => m.LopId == id);
            if (lOP == null)
            {
                return NotFound();
            }

            return View(lOP);
        }

        // POST: LOPs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lOP = await _context.LOPs.FindAsync(id);
            if (lOP != null)
            {
                _context.LOPs.Remove(lOP);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LOPExists(int id)
        {
            return _context.LOPs.Any(e => e.LopId == id);
        }
    }
}
