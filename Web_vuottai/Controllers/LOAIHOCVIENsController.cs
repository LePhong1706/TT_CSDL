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
    public class LOAIHOCVIENsController : Controller
    {
        private readonly AppDbContext _context;

        public LOAIHOCVIENsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: LOAIHOCVIENs
        public async Task<IActionResult> Index()
        {
            return View(await _context.LOAIHOCVIENs.ToListAsync());
        }

        // GET: LOAIHOCVIENs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lOAIHOCVIEN = await _context.LOAIHOCVIENs
                .FirstOrDefaultAsync(m => m.LoaiHVId == id);
            if (lOAIHOCVIEN == null)
            {
                return NotFound();
            }

            return View(lOAIHOCVIEN);
        }

        // GET: LOAIHOCVIENs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LOAIHOCVIENs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LoaiHVId,TenLoaiHV,HeSo")] LOAIHOCVIEN lOAIHOCVIEN)
        {
            if (ModelState.IsValid)
            {
                _context.Add(lOAIHOCVIEN);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(lOAIHOCVIEN);
        }

        // GET: LOAIHOCVIENs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lOAIHOCVIEN = await _context.LOAIHOCVIENs.FindAsync(id);
            if (lOAIHOCVIEN == null)
            {
                return NotFound();
            }
            return View(lOAIHOCVIEN);
        }

        // POST: LOAIHOCVIENs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LoaiHVId,TenLoaiHV,HeSo")] LOAIHOCVIEN lOAIHOCVIEN)
        {
            if (id != lOAIHOCVIEN.LoaiHVId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lOAIHOCVIEN);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LOAIHOCVIENExists(lOAIHOCVIEN.LoaiHVId))
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
            return View(lOAIHOCVIEN);
        }

        // GET: LOAIHOCVIENs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lOAIHOCVIEN = await _context.LOAIHOCVIENs
                .FirstOrDefaultAsync(m => m.LoaiHVId == id);
            if (lOAIHOCVIEN == null)
            {
                return NotFound();
            }

            return View(lOAIHOCVIEN);
        }

        // POST: LOAIHOCVIENs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lOAIHOCVIEN = await _context.LOAIHOCVIENs.FindAsync(id);
            if (lOAIHOCVIEN != null)
            {
                _context.LOAIHOCVIENs.Remove(lOAIHOCVIEN);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LOAIHOCVIENExists(int id)
        {
            return _context.LOAIHOCVIENs.Any(e => e.LoaiHVId == id);
        }
    }
}
