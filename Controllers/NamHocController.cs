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
    public class NamHocController : Controller
    {
        private readonly AppDbContext _context;

        public NamHocController(AppDbContext context)
        {
            _context = context;
        }

        // GET: NamHoc
        public async Task<IActionResult> Index()
        {
            return View(await _context.THOIKHOABIEUs.OrderByDescending(t => t.NamHoc).ThenBy(t => t.HocKy).ToListAsync());
        }

        // GET: NamHoc/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tHOIKHOABIEU = await _context.THOIKHOABIEUs
                .FirstOrDefaultAsync(m => m.TkbId == id);
            if (tHOIKHOABIEU == null)
            {
                return NotFound();
            }

            return View(tHOIKHOABIEU);
        }

        // GET: NamHoc/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: NamHoc/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TkbId,NamHoc,HocKy,TenTkb")] THOIKHOABIEU tHOIKHOABIEU)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tHOIKHOABIEU);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tHOIKHOABIEU);
        }

        // GET: NamHoc/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tHOIKHOABIEU = await _context.THOIKHOABIEUs.FindAsync(id);
            if (tHOIKHOABIEU == null)
            {
                return NotFound();
            }
            return View(tHOIKHOABIEU);
        }

        // POST: NamHoc/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TkbId,NamHoc,HocKy,TenTkb")] THOIKHOABIEU tHOIKHOABIEU)
        {
            if (id != tHOIKHOABIEU.TkbId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tHOIKHOABIEU);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!THOIKHOABIEUExists(tHOIKHOABIEU.TkbId))
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
            return View(tHOIKHOABIEU);
        }

        // GET: NamHoc/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tHOIKHOABIEU = await _context.THOIKHOABIEUs
                .FirstOrDefaultAsync(m => m.TkbId == id);
            if (tHOIKHOABIEU == null)
            {
                return NotFound();
            }

            return View(tHOIKHOABIEU);
        }

        // POST: NamHoc/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tHOIKHOABIEU = await _context.THOIKHOABIEUs.FindAsync(id);
            if (tHOIKHOABIEU != null)
            {
                var chiTiet = await _context.TKB_CHITIETs.Where(t => t.TkbId == id).ToListAsync();
                if (chiTiet.Any())
                {
                    var chiTietIds = chiTiet.Select(ct => ct.TkbCtId).ToList();
                    var phanCongs = await _context.PHANCONGs.Where(pc => chiTietIds.Contains(pc.TkbCtId)).ToListAsync();
                    if (phanCongs.Any())
                    {
                        _context.PHANCONGs.RemoveRange(phanCongs);
                    }
                    _context.TKB_CHITIETs.RemoveRange(chiTiet);
                }
                _context.THOIKHOABIEUs.Remove(tHOIKHOABIEU);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool THOIKHOABIEUExists(int id)
        {
            return _context.THOIKHOABIEUs.Any(e => e.TkbId == id);
        }
    }
}
