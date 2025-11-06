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
    public class CHUCDANHsController : Controller
    {
        private readonly AppDbContext _context;

        public CHUCDANHsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: CHUCDANHs
        public async Task<IActionResult> Index()
        {
            return View(await _context.CHUCDANHs.ToListAsync());
        }

        // GET: CHUCDANHs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cHUCDANH = await _context.CHUCDANHs
                .FirstOrDefaultAsync(m => m.ChucDanhId == id);
            if (cHUCDANH == null)
            {
                return NotFound();
            }

            return View(cHUCDANH);
        }

        // GET: CHUCDANHs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CHUCDANHs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ChucDanhId,TenChucDanh")] CHUCDANH cHUCDANH)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cHUCDANH);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cHUCDANH);
        }

        // GET: CHUCDANHs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cHUCDANH = await _context.CHUCDANHs.FindAsync(id);
            if (cHUCDANH == null)
            {
                return NotFound();
            }
            return View(cHUCDANH);
        }

        // POST: CHUCDANHs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ChucDanhId,TenChucDanh")] CHUCDANH cHUCDANH)
        {
            if (id != cHUCDANH.ChucDanhId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cHUCDANH);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CHUCDANHExists(cHUCDANH.ChucDanhId))
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
            return View(cHUCDANH);
        }

        // GET: CHUCDANHs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cHUCDANH = await _context.CHUCDANHs
                .FirstOrDefaultAsync(m => m.ChucDanhId == id);
            if (cHUCDANH == null)
            {
                return NotFound();
            }

            return View(cHUCDANH);
        }

        // POST: CHUCDANHs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cHUCDANH = await _context.CHUCDANHs.FindAsync(id);
            if (cHUCDANH != null)
            {
                _context.CHUCDANHs.Remove(cHUCDANH);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CHUCDANHExists(int id)
        {
            return _context.CHUCDANHs.Any(e => e.ChucDanhId == id);
        }
    }
}
