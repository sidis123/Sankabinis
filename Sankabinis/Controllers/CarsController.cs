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
    public class CarsController : Controller
    {
        private readonly SankabinisContext _context;

        public CarsController(SankabinisContext context)
        {
            _context = context;
        }

        // GET: CarsPage
        public async Task<IActionResult> Index()
        {
            var loggedInUserId = HttpContext.Session.GetInt32("UserId");
            if (loggedInUserId == null)
            {
                return RedirectToPage("/Home/NavigateToSignInPage");
                //return RedirectToAction("../Home/NavigateToSignInPage"); // Or any other appropriate action
            }

            var userCars = _context.Car
                .Where(car => car.Fk_Naudotojasid_Naudotojas == loggedInUserId.Value)
                .ToListAsync();

            return View(await userCars);
        }

        // GET: CarsPage/Details/5
        // GET: CarsPage/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loggedInUserId = HttpContext.Session.GetInt32("UserId");
            if (loggedInUserId == null)
            {
                return RedirectToAction("Login"); // Or any other appropriate action
            }

            var automobilis = await _context.Car
                .FirstOrDefaultAsync(m => m.Id_Automobilis == id && m.Fk_Naudotojasid_Naudotojas == loggedInUserId.Value);

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
        public async Task<IActionResult> Create([Bind("Id_Automobilis,Modelis,Marke,Numeris,Galingumas,Spalva,Rida,Pagaminimo_data,Svoris,Kuro_tipas,Pavaru_deze,Kebulas")] Car automobilis)
        {
            if (ModelState.IsValid)
            {
                // Retrieve the logged in user's ID from session
                var loggedInUserId = HttpContext.Session.GetInt32("UserId");

                // Set the user ID to the car object
                automobilis.Fk_Naudotojasid_Naudotojas = loggedInUserId.Value;

                _context.Add(automobilis);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Reload dropdown data for re-displaying form if validation fails
            LoadSelectLists();
            return View(automobilis);
        }

        private bool ValidateData(Car car)
        {
            return true;
        }
        private double PowerWeight(double svoris, int galia)
        {
            return (double)galia / svoris;
        }

        private int Klase(double pw)
        {
            if (pw < 0.05)
                return 1;
            else if (pw < 0.1 && pw >= 0.05)
                return 2;
            else if (pw < 0.2 && pw >= 0.1)
                return 3;
            else
                return 4;
        }
        // GET: CarsPage/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var automobilis = await _context.Car.FindAsync(id);
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
        public async Task<IActionResult> Edit(int id, [Bind("Id_Automobilis,Modelis,Marke,Numeris,Galingumas,Spalva,Rida,Pagaminimo_data,Svoris,Kuro_tipas,Pavaru_deze,Kebulas,Fk_Naudotojasid_Naudotojas")] Car automobilis)
        {
            if (id != automobilis.Id_Automobilis)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    automobilis.Klase = CalculateClass(automobilis.Svoris, automobilis.Galingumas);
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
            LoadSelectLists();
            return View(automobilis);
        }
        private void LoadSelectLists()
        {
            ViewBag.KuroTipasOptions = new SelectList(Enum.GetValues(typeof(KuroTipas)), "Value", "Text");
            ViewBag.PavaruDezeOptions = new SelectList(Enum.GetValues(typeof(PavaruDeze)), "Value", "Text");
            ViewBag.KebulasOptions = new SelectList(Enum.GetValues(typeof(Kebulas)), "Value", "Text");
            ViewBag.KlaseOptions = new SelectList(Enum.GetValues(typeof(AutomobilioKlase)), "Value", "Text");
        }

        private AutomobilioKlase CalculateClass(double svoris, int galingumas)
        {
            double pw = PowerWeight(svoris, galingumas);
            return (AutomobilioKlase)Klase(pw);
        }


        // GET: CarsPage/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var automobilis = await _context.Car
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
            var automobilis = await _context.Car.FindAsync(id);
            if (automobilis != null)
            {
                _context.Car.Remove(automobilis);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AutomobilisExists(int id)
        {
            return _context.Car.Any(e => e.Id_Automobilis == id);
        }
    }
}
