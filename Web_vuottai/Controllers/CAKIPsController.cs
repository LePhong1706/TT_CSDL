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
    public class CAKIPsController : Controller
    {
        private readonly AppDbContext _context;

        public CAKIPsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: CAKIPs
        public async Task<IActionResult> Index()
        {
            return View(await _context.CAKIPs.ToListAsync());
        }

        // GET: CAKIPs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cAKIP = await _context.CAKIPs
                .FirstOrDefaultAsync(m => m.CaKipId == id);
            if (cAKIP == null)
            {
                return NotFound();
            }

            return View(cAKIP);
        }

        // GET: CAKIPs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CAKIPs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CaKipId,TenCaKip,HeSo")] CAKIP cAKIP)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cAKIP);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cAKIP);
        }

        // GET: CAKIPs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cAKIP = await _context.CAKIPs.FindAsync(id);
            if (cAKIP == null)
            {
                return NotFound();
            }
            return View(cAKIP);
        }

        // POST: CAKIPs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CaKipId,TenCaKip,HeSo")] CAKIP cAKIP)
        {
            if (id != cAKIP.CaKipId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cAKIP);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CAKIPExists(cAKIP.CaKipId))
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
            return View(cAKIP);
        }

        // GET: CAKIPs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cAKIP = await _context.CAKIPs
                .FirstOrDefaultAsync(m => m.CaKipId == id);
            if (cAKIP == null)
            {
                return NotFound();
            }

            return View(cAKIP);
        }

        // POST: CAKIPs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cAKIP = await _context.CAKIPs.FindAsync(id);
            if (cAKIP != null)
            {
                _context.CAKIPs.Remove(cAKIP);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CAKIPExists(int id)
        {
            return _context.CAKIPs.Any(e => e.CaKipId == id);
        }
    }
}
