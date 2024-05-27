using GoogleApi.Entities.Search.Common;
using Microsoft.AspNetCore.Mvc;
using Sankabinis.Data;
using Sankabinis.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Sankabinis.Controllers
{
    public class TimeChoiceController : Controller
    {
        private readonly SankabinisContext _context;

        public TimeChoiceController(SankabinisContext context)
        {
            _context = context;
        }

        public IActionResult Index(int id)
        {
            var race = _context.Race.Find(id);

            return View(race);
        }

        public IActionResult SubmitMatchTime(int Id_Lenktynes, DateTime pasiulytas_laikas)
        {
            UpdateMatchTime(Id_Lenktynes, pasiulytas_laikas);

            var raceId = Id_Lenktynes;
            var loggedInUserId = _context.Race.Find(Id_Lenktynes).User1Id;
            //Cia notify opponent kazka
            var race = _context.Race.Find(raceId);
            TempData["SuccessMessage"] = "Time successfully submitted!";
            //return View("Index", race);
            return RedirectToAction("MatchPage", "Race", new { raceId, loggedInUserId});
        }

        public int UpdateMatchTime(int raceId, DateTime time)
        {
            var race = _context.Race.Find(raceId);
            if (race == null)
            {
                _context.Race.Add(race);
            }

            race.pasiulytas_laikas = time;
            _context.Race.Update(race);
            _context.SaveChanges();
            return 0;
        }

        [HttpPost]
        public IActionResult TimeSelectConfirm(int Id_Lenktynes)
        {
            UpdateTimeConfirm(Id_Lenktynes);

            var raceId = Id_Lenktynes;
            var loggedInUserId = _context.Race.Find(Id_Lenktynes).User2Id;

            var race = _context.Race.Find(raceId);
            TempData["SuccessMessage"] = "Time successfully confirmed!";
            return View("Index", race);
            //return RedirectToAction("MatchPage", "Race", new { raceId = raceId, loggedInUserId = loggedInUserId });
        }

        public int UpdateTimeConfirm(int raceId)
        {
            var race = _context.Race.Find(raceId);
            race.ar_laikas_patvirtintas = true;
            _context.Race.Update(race);
            _context.SaveChanges();
            return 0;
        }

        public IActionResult TimeSelectDeny(int Id_Lenktynes)
        {
            var race = ShowTimeSelection(Id_Lenktynes);

            return View("Index", race);
        }

        public Race ShowTimeSelection(int raceId)
        {
            var race = _context.Race.Find(raceId);
            var temp1 = race.User1Id;
            var temp2 = race.User2Id;
            race.User2Id = temp1;
            race.User1Id = temp2;
            _context.Race.Update(race);
            _context.SaveChanges();

            return race;
        }

    }
}
