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
    public class NGONNGUsController : Controller
    {
        private readonly AppDbContext _context;

        public NGONNGUsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: NGONNGUs
        public async Task<IActionResult> Index()
        {
            return View(await _context.NGONNGUs.ToListAsync());
        }

        // GET: NGONNGUs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nGONNGU = await _context.NGONNGUs
                .FirstOrDefaultAsync(m => m.NgonNguId == id);
            if (nGONNGU == null)
            {
                return NotFound();
            }

            return View(nGONNGU);
        }

        // GET: NGONNGUs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: NGONNGUs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NgonNguId,TenNgonNgu,HeSo")] NGONNGU nGONNGU)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nGONNGU);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(nGONNGU);
        }

        // GET: NGONNGUs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nGONNGU = await _context.NGONNGUs.FindAsync(id);
            if (nGONNGU == null)
            {
                return NotFound();
            }
            return View(nGONNGU);
        }

        // POST: NGONNGUs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("NgonNguId,TenNgonNgu,HeSo")] NGONNGU nGONNGU)
        {
            if (id != nGONNGU.NgonNguId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nGONNGU);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NGONNGUExists(nGONNGU.NgonNguId))
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
            return View(nGONNGU);
        }

        // GET: NGONNGUs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nGONNGU = await _context.NGONNGUs
                .FirstOrDefaultAsync(m => m.NgonNguId == id);
            if (nGONNGU == null)
            {
                return NotFound();
            }

            return View(nGONNGU);
        }

        // POST: NGONNGUs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var nGONNGU = await _context.NGONNGUs.FindAsync(id);
            if (nGONNGU != null)
            {
                _context.NGONNGUs.Remove(nGONNGU);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NGONNGUExists(int id)
        {
            return _context.NGONNGUs.Any(e => e.NgonNguId == id);
        }
    }
}
