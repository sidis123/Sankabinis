using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sankabinis.Data;
using Sankabinis.Models;

namespace Sankabinis.Controllers
{
    public class CarsPageController : Controller
    {
        private readonly SankabinisContext _context;

        public CarsPageController(SankabinisContext context)
        {
            _context = context;
        }

        // GET: CarsPage
        public async Task<IActionResult> Index()
        {
            return View(await _context.Automobilis.ToListAsync());
        }

        // GET: CarsPage/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var automobilis = await _context.Automobilis
                .FirstOrDefaultAsync(m => m.Id_Automobilis == id);
            if (automobilis == null)
            {
                return NotFound();
            }

            return View(automobilis);
        }

        // GET: CarsPage/Create
        public IActionResult Create()
        {
            ViewBag.PavaruDezeOptions = Enum.GetValues(typeof(PavaruDeze))
                .Cast<PavaruDeze>()
                .Select(d => new SelectListItem
                {
                    Text = d.ToString(),
                    Value = ((int)d).ToString()
                }).ToList();
            // Populate KuroTipas select list
            ViewBag.KuroTipasOptions = Enum.GetValues(typeof(KuroTipas))
                .Cast<KuroTipas>()
                .Select(kt => new SelectListItem
                {
                    Text = kt.ToString(),
                    Value = ((int)kt).ToString()
                }).ToList();

            // Populate Kebulas select list
            ViewBag.KebulasOptions = Enum.GetValues(typeof(Kebulas))
                .Cast<Kebulas>()
                .Select(k => new SelectListItem
                {
                    Text = k.ToString(),
                    Value = ((int)k).ToString()
                }).ToList();

            // Populate Klase select list
            ViewBag.KlaseOptions = Enum.GetValues(typeof(AutomobilioKlase))
                .Cast<AutomobilioKlase>()
                .Select(cl => new SelectListItem
                {
                    Text = cl.ToString(),
                    Value = ((int)cl).ToString()
                }).ToList();

            return View();
        }

        // POST: CarsPage/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id_Automobilis,Modelis,Marke,Numeris,Galingumas,Spalva,Rida,Pagaminimo_data,Svoris,Kuro_tipas,Pavaru_deze,Kebulas,Klase,Fk_Naudotojasid_Naudotojas")] Automobilis automobilis)
        {
            if (ModelState.IsValid)
            {
                _context.Add(automobilis);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(automobilis);
        }

        // GET: CarsPage/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var automobilis = await _context.Automobilis.FindAsync(id);
            if (automobilis == null)
            {
                return NotFound();
            }

            ViewBag.KuroTipasSelectList = new SelectList(Enum.GetValues(typeof(KuroTipas)));
            ViewBag.PavaruDezeSelectList = new SelectList(Enum.GetValues(typeof(PavaruDeze)));
            ViewBag.KebulasSelectList = new SelectList(Enum.GetValues(typeof(Kebulas)));
            ViewBag.KlaseSelectList = new SelectList(Enum.GetValues(typeof(AutomobilioKlase)));

            return View(automobilis);
        }

        // POST: CarsPage/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id_Automobilis,Modelis,Marke,Numeris,Galingumas,Spalva,Rida,Pagaminimo_data,Svoris,Kuro_tipas,Pavaru_deze,Kebulas,Klase,Fk_Naudotojasid_Naudotojas")] Automobilis automobilis)
        {
            if (id != automobilis.Id_Automobilis)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(automobilis);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AutomobilisExists(automobilis.Id_Automobilis))
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
            return View(automobilis);
        }

        // GET: CarsPage/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var automobilis = await _context.Automobilis
                .FirstOrDefaultAsync(m => m.Id_Automobilis == id);
            if (automobilis == null)
            {
                return NotFound();
            }

            return View(automobilis);
        }

        // POST: CarsPage/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var automobilis = await _context.Automobilis.FindAsync(id);
            if (automobilis != null)
            {
                _context.Automobilis.Remove(automobilis);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AutomobilisExists(int id)
        {
            return _context.Automobilis.Any(e => e.Id_Automobilis == id);
        }
    }
}
