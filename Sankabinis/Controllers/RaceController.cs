using GoogleApi.Entities.Search.Common;
using Microsoft.AspNetCore.Mvc;
using Sankabinis.Data;
using Sankabinis.Models;
using System.Collections.Generic;
using System.Linq;

namespace Sankabinis.Controllers
{
    public class RaceController : Controller
    {
        private readonly SankabinisContext _context;

        public RaceController(SankabinisContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<Car> c = new List<Car>();

            var loggedInUserId = HttpContext.Session.GetInt32("UserId");

            c = FetchCarsFromDatabase(loggedInUserId);

            // Uncomment when cars are actually working with userids
            // if (c.Count == 0)
            // {
            //     TempData["Message"] = "Please create a car.";
            //     return RedirectToAction("Index", "Home");
            // }

            return View(); // Pass the list of cars to the view
        }

        private List<Car> FetchCarsFromDatabase(int? userId)
        {
            return _context.Car.Where(c => c.Fk_Naudotojasid_Naudotojas == userId).ToList();
        }

        public IActionResult BeginSearch(int distance)
        {
            Console.WriteLine("BeginSearchas");
            int? loggedInUserId = HttpContext.Session.GetInt32("UserId");

            User user = _context.Users.FirstOrDefault(u => u.Id_Naudotojas == loggedInUserId);
            int userMiestas = user.CityId;

            List<User> potentialOponents = _context.Users.Where(u => u.Id_Naudotojas != loggedInUserId && u.CityId != null).ToList();

            // Collect potential opponents to remove
            List<User> opponentsToRemove = new List<User>();

            foreach (var potentialOponent in potentialOponents)
            {
                int oponentCity = potentialOponent.CityId;

                if (oponentCity != userMiestas)
                {
                    Distance distanceBetweenCities = _context.Distance.FirstOrDefault(d =>
                    (d.CityId1 == userMiestas && d.CityId2 == oponentCity) ||
                    (d.CityId1 == oponentCity && d.CityId2 == userMiestas));

                    if (distanceBetweenCities != null)
                    {
                        double atstumas = distanceBetweenCities.Atstumas;
                        if (atstumas > distance)
                        {
                            opponentsToRemove.Add(potentialOponent);
                        }
                    }
                }
            }

            // Remove the collected opponents
            foreach (var opponent in opponentsToRemove)
            {
                potentialOponents.Remove(opponent);
            }

            if (potentialOponents.Count == 0)
            {
                ViewBag.ErrorMessage = "No potential opponents found.";
                ViewBag.Distance = distance;
                return View("Index");
            }

            List<Car> usersCars = _context.Car.Where(c => c.Fk_Naudotojasid_Naudotojas == loggedInUserId).ToList();

            Console.WriteLine(loggedInUserId);
            List<AutomobilioKlase> uniqueClasses = new List<AutomobilioKlase>();

            foreach (var car in usersCars)
            {
                if (!uniqueClasses.Contains(car.Klase))
                {
                    uniqueClasses.Add(car.Klase);
                }
            }

            // Collect potential opponents to remove
            opponentsToRemove.Clear();

            foreach (var potentialOponent in potentialOponents)
            {
                List<Car> oponentsCars = _context.Car.Where(c => c.Fk_Naudotojasid_Naudotojas == potentialOponent.Id_Naudotojas).ToList();
                bool hasSameClass = false;

                if (oponentsCars.Count > 0)
                {
                    foreach (var oponentCar in oponentsCars)
                    {
                        foreach(var uniqueClass in uniqueClasses)
                        {
                            if (oponentCar.Klase == uniqueClass)
                            {
                                hasSameClass = true;
                                break;
                            }
                        }
                    }
                }

                if (!hasSameClass)
                {
                    opponentsToRemove.Add(potentialOponent);
                }
            }

            // Remove the collected opponents
            foreach (var opponent in opponentsToRemove)
            {
                potentialOponents.Remove(opponent);
            }

            if (potentialOponents.Count == 0)
            {
                ViewBag.ErrorMessage = "No potential opponents found.";
                ViewBag.Distance = distance;
                return View("Index");
            }

            List<Race> potentialRaces = new List<Race>();

            foreach (var potentialOponent in potentialOponents)
            {
                List<Race> potentialRacesWithOponent = _context.Race.Where(r =>
                    (r.User1Id == loggedInUserId && r.User2Id == potentialOponent.Id_Naudotojas) ||
                    (r.User1Id == potentialOponent.Id_Naudotojas && r.User2Id == loggedInUserId)).ToList();

                foreach (var uniqueClass in uniqueClasses)
                {
                    bool raceAlreadyExists = false;
                    foreach (var potentialRaceWithOponent in potentialRacesWithOponent)
                    {
                        if (potentialRaceWithOponent.Automobilio_klase == uniqueClass &&
                            ((potentialRaceWithOponent.User1Id == loggedInUserId && potentialRaceWithOponent.Pirmo_naudotojo_patvirtinimas == true) ||
                            (potentialRaceWithOponent.User2Id == loggedInUserId && potentialRaceWithOponent.Antro_naudotojo_patvirtinimas == true)))
                        {
                            raceAlreadyExists = true;
                            potentialRacesWithOponent.Remove(potentialRaceWithOponent);
                            break;
                        }
                    }
                    if (!raceAlreadyExists)
                    {
                        Race newRace = new Race
                        {
                            User1Id = (int)loggedInUserId,
                            User2Id = potentialOponent.Id_Naudotojas,
                            Automobilio_klase = uniqueClass,
                        };

                        potentialRaces.Add(newRace);
                    }
                }

                potentialRaces.AddRange(potentialRacesWithOponent);
            }

            if (potentialRaces.Count == 0)
            {
                ViewBag.ErrorMessage = "No potential opponents found.";
                ViewBag.Distance = distance;
                return View("Index");
            }

            potentialRaces = SortRacesByElo(potentialRaces);

            List<RaceOpponentViewModel> RaceAndOpps = new List<RaceOpponentViewModel>();

            foreach (var race in potentialRaces)
            {
                var opponentId = race.User1Id == loggedInUserId ? race.User2Id : race.User1Id;
                var opponent = _context.Users.FirstOrDefault(u => u.Id_Naudotojas == opponentId);

                RaceAndOpps.Add(new RaceOpponentViewModel
                {
                    Race = race,
                    Opponent = opponent
                });
            }

            return View("Index", RaceAndOpps);
        }

        private List<Race> SortRacesByElo(List<Race> races)
        {
            var loggedInUserId = HttpContext.Session.GetInt32("UserId");
            int loggedInUserElo = _context.Users.FirstOrDefault(u => u.Id_Naudotojas == loggedInUserId)?.Elo ?? 0;

            Dictionary<Race, int> eloDifferences = new Dictionary<Race, int>();
            foreach (var race in races)
            {
                int opponentId = race.User1Id == loggedInUserId ? race.User2Id : race.User1Id;
                int opponentElo = _context.Users.FirstOrDefault(u => u.Id_Naudotojas == opponentId)?.Elo ?? 0;
                int difference = Math.Abs(loggedInUserElo - opponentElo);
                eloDifferences.Add(race, difference);
            }

            return races.OrderBy(race => eloDifferences[race]).ToList();
        }

        [HttpPost]
        public IActionResult ConfirmRace([FromBody]Race race)
        {

            Console.WriteLine(race.User1Id);
            Console.WriteLine(race.User2Id);

            var existingRace = _context.Race.FirstOrDefault(r => ((r.User1Id == race.User1Id && r.User2Id == race.User2Id) || (r.User1Id == race.User2Id && r.User2Id == race.User1Id)) && r.Automobilio_klase == race.Automobilio_klase);
            
            if (existingRace == null)
            {
                race.Pirmo_naudotojo_patvirtinimas = true;
                _context.Race.Add(race);
                _context.SaveChanges();
                return Ok(new { action = "newRaceCreated" });
            }
            else
            {
                existingRace.Antro_naudotojo_patvirtinimas = true;
                _context.SaveChanges();
                return Ok(new { action = "existingRaceConfirmed" });
            }
        }

        private Track ChooseRandomTrack(List<Track> tracks)
        {
            Random random = new Random();
            int index = random.Next(tracks.Count);
            return tracks[index];
        }

        //[HttpGet]
        //public IActionResult InitiateRace()
        //{
        //    List<Track> tracks = _context.Track.ToList();

        //    Track track = ChooseRandomTrack(tracks);


        //}
        [HttpPost]
        public IActionResult RegisterResult(int userId, int result)
        {
            try
            {
                var race = _context.Race
                    .Where(r => r.User1Id == userId || r.User2Id == userId)
                    .FirstOrDefault();

                if (race == null)
                {
                    return Json(new { success = false, message = "Race not found." });
                }

                if (race.User1Id == userId)
                {
                    race.rezultatas_pagal_pirmaji_naudotoja = result;
                }
                else if (race.User2Id == userId)
                {
                    race.rezultatas_pagal_antraji_naudotoja = result;
                }

                _context.SaveChanges();

                if (race.rezultatas_pagal_pirmaji_naudotoja != 100 && race.rezultatas_pagal_antraji_naudotoja != 100) //cia padariau kad tipo kol nera ivestas rezultatas reikia kad defaultinis rezultatas butu 100 , o veliau jis keiciamas i 1 jei laimi arba i 0 jei pralaimi
                {
                    bool checkResult = CheckBothResults(race);
                    if (checkResult)//kai grazina true
                    {
                        AddTrustScoreToBothCompetitors(race.User1Id, race.User2Id);
                        //Recalculate Elo
                        //Recalculate the experience level
                        InformRacersOfEndOfMatch(race.User1Id, race.User2Id);
                        //toliau su achievementais ir tt.... test darba
                    }
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        private bool CheckBothResults(Race race)
        {
            var lenktynes = _context.Race
                .FirstOrDefault(r => r.Id_Lenktynes == race.Id_Lenktynes);

            if (lenktynes == null)
            {
                // Handle the case where the lenktynes record is not found
                return false;
            }

            // Compare the results
            if (lenktynes.rezultatas_pagal_pirmaji_naudotoja != lenktynes.rezultatas_pagal_antraji_naudotoja)
            {
                return true;//true jei geri rezultatai ir niekas nesukciavo
            }

            return false;//false, jei kazkas sukciavo ir ivede bloga rezultata
        }

        private void AddTrustScoreToBothCompetitors(int user1Id, int user2Id)
        {
            var user1 = _context.Users.FirstOrDefault(u => u.Id_Naudotojas == user1Id);
            var user2 = _context.Users.FirstOrDefault(u => u.Id_Naudotojas == user2Id);

            if (user1 != null && user2 != null)
            {
                user1.Pasitikimo_taskai++;
                user2.Pasitikimo_taskai++;
                _context.SaveChanges();
            }
        }

        private void InformRacersOfEndOfMatch(int user1Id, int user2Id)
        {
            var user1 = _context.Users.FirstOrDefault(u => u.Id_Naudotojas == user1Id);
            var user2 = _context.Users.FirstOrDefault(u => u.Id_Naudotojas == user2Id);

            if (user1 != null && user2 != null)
            {
                // Send notification to user1
                // For example: user1.SendNotification("Your match has ended.");

                // Send notification to user2
                // For example: user2.SendNotification("Your match has ended.");

                // You can implement your notification mechanism here
            }
        }

    }
}