﻿using GoogleApi.Entities.Search.Common;
using Microsoft.AspNetCore.Mvc;
using Sankabinis.Data;
using Sankabinis.Models;
using System.Collections.Generic;
using System.Diagnostics;
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
        public IActionResult RegisterResult(int userId, int result, int raceId)
        {
            try
            {
                var race = _context.Race.FirstOrDefault(r => r.Id_Lenktynes == raceId && (r.User1Id == userId || r.User2Id == userId));

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

                if (race.rezultatas_pagal_pirmaji_naudotoja != 100 && race.rezultatas_pagal_antraji_naudotoja != 100)
                {
                    bool checkResult = CheckBothResults(race);
                    if (checkResult)//tikrinam ar rezultatai sutampa (ty. niekas nemelavo)
                    {
                        AddTrustScoreToBothCompetitors(race.User1Id, race.User2Id);
                        //Recalculate Elo
                        //Recalculate the experience level
                        InformRacersOfEndOfMatch(race.Id_Lenktynes);//kad parodyti kuris laimejo screena turi but ar pasibaigusios true ir final true 
                        //achievementai
                        //tikrinam ar gavo nauju achievementu
                        //jeigu gavo ifas
                        Winner(race.Id_Lenktynes);
                    }
                    else//po rezultatu patikros rezultatai nesutapo (ty.kazkas pamelavo)
                    {
                        int m = CheckTrustScoreDifference(race.User1Id, race.User2Id);
                        if (m > 30)//kadangi trust score zenkliai skiriasi , darom skunda
                       {
                           var raceData = GetDataAboutRace(race.Id_Lenktynes);

                            var racerData = GetDataAboutRacers(race.User1Id, race.User2Id);

                            if (raceData != null && racerData.Item1 != null && racerData.Item2 != null)
                            {
                                CreateAppeal(raceData, racerData.Item1, racerData.Item2);
                                InformRacersOfEndOfMatch(race.Id_Lenktynes);//atvejis kai vienas sukciavo, todel kuriamas appealas. pasibaigusios true o final score false
                                InformRacersOfAppeal(race.Id_Lenktynes);
                            }
                        }
                        else//priskiriam pergale naudotojui su didesniu trust score pasibaigusios true ir final true
                        {
                            DeclareVictor(race.User1Id, race.User2Id, race.Id_Lenktynes);
                            //recalculate Elo
                            //recalculate the experience level
                            InformRacersOfEndOfMatch(race.Id_Lenktynes);
                            //achievementai
                            //tikrinam ar gavo nauju achievements
                            //jeigu gavo iffas
                        }

                    }
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        private void DeclareVictor(int user1Id, int user2Id, int raceId)
        {
            try
            {
                // Get trust scores of both racers
                int trustScore1 = _context.Users.FirstOrDefault(u => u.Id_Naudotojas == user1Id)?.Pasitikimo_taskai ?? 0;
                int trustScore2 = _context.Users.FirstOrDefault(u => u.Id_Naudotojas == user2Id)?.Pasitikimo_taskai ?? 0;

                // Update the race in the lenktynes table based on trust scores
                bool user1Won = trustScore1 > trustScore2;

                var race = _context.Race.FirstOrDefault(r => r.Id_Lenktynes == raceId);
                if (race != null)
                {
                    race.ar_galutinis_rezultatas = true;
                    race.rezultatas_pagal_pirmaji_naudotoja = user1Won ? 1 : 0;
                    race.rezultatas_pagal_antraji_naudotoja = user1Won ? 0 : 1;
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine("Error in declaring victor: " + ex.Message);
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
        private void Winner(int lenktyniuId)
        {
            try
            {
                // Retrieve the race based on its ID
                var race = _context.Race.FirstOrDefault(r => r.Id_Lenktynes == lenktyniuId);
                if (race != null)
                {
                    // Set ar_lenktynes_pasibaigusios to true
                    race.ar_galutinis_rezultatas = true;
                    // Save the changes to the database
                    _context.SaveChanges();
                }
                else
                {
                    // Handle the case where the race is not found
                    Console.WriteLine("Race not found with ID: " + lenktyniuId);
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine("Error in informing racers of the end of the match: " + ex.Message);
            }
        }
        private void InformRacersOfEndOfMatch(int lenktyniuId)
        {
            try
            {
                // Retrieve the race based on its ID
                var race = _context.Race.FirstOrDefault(r => r.Id_Lenktynes == lenktyniuId);
                if (race != null)
                {
                    // Set ar_lenktynes_pasibaigusios to true
                    race.ar_lenktynes_pasibaigusios = true;

                    // Save the changes to the database
                    _context.SaveChanges();
                }
                else
                {
                    // Handle the case where the race is not found
                    Console.WriteLine("Race not found with ID: " + lenktyniuId);
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine("Error in informing racers of the end of the match: " + ex.Message);
            }
        }
        private void InformRacersOfAppeal(int lenktyniuId)
        {
            try
            {
                // Retrieve the race based on its ID
                var race = _context.Race.FirstOrDefault(r => r.Id_Lenktynes == lenktyniuId);
                if (race != null)
                {
                    // Set ar_lenktynes_pasibaigusios to true
                    race.ar_galutinis_rezultatas = false;

                    // Save the changes to the database
                    _context.SaveChanges();
                }
                else
                {
                    // Handle the case where the race is not found
                    Console.WriteLine("Race not found with ID: " + lenktyniuId);
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine("Error in informing racers of appeal: " + ex.Message);
            }
        }
        private int CheckTrustScoreDifference(int userId1, int userId2)
        {
            try
            {
                var user1 = _context.Users.FirstOrDefault(u => u.Id_Naudotojas == userId1);
                var user2 = _context.Users.FirstOrDefault(u => u.Id_Naudotojas == userId2);

                if (user1 == null || user2 == null)
                {
                    // If any of the users is not found, return 0 as the difference
                    return 0;
                }

                // Calculate the difference between trust scores
                int trustScoreDifference = Math.Abs(user1.Pasitikimo_taskai - user2.Pasitikimo_taskai);
                return trustScoreDifference;
            }
            catch (Exception ex)
            {
                // Handle any exceptions and return 0
                Console.WriteLine("Error in calculating trust score difference: " + ex.Message);
                return 0;
            }
        }
        public Race GetDataAboutRace(int raceId)
        {
            try
            {
                // Retrieve data about the race based on its ID
                var race = _context.Race.FirstOrDefault(r => r.Id_Lenktynes == raceId);
                return race;
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine("Error in getting data about the race: " + ex.Message);
                return null;
            }
        }
        public User GetDataAboutRacer(int id)
        {
            try
            {
                // Retrieve data about the race based on its ID
                var user = _context.Users.FirstOrDefault(r => r.Id_Naudotojas == id);
                return user;
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine("Error in getting data about the user: " + ex.Message);
                return null;
            }
        }

        private (User, User) GetDataAboutRacers(int userId1, int userId2)
        {
            try
            {
                // Retrieve data about both racers based on their User IDs
                var user1 = _context.Users.FirstOrDefault(u => u.Id_Naudotojas == userId1);
                var user2 = _context.Users.FirstOrDefault(u => u.Id_Naudotojas == userId2);
                return (user1, user2);
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine("Error in getting data about the racers: " + ex.Message);
                return (null, null);
            }
        }
        private void CreateAppeal(Race race, User racer1, User racer2)
        {
            try
            {
                // Create a new entry in the skundas table
                Complaint newAppeal = new Complaint
                {
                    Paaiskinimas = $"Naudotojo kurio id yra {racer1.Id_Naudotojas} ir naudotojo, kurio id yra {racer2.Id_Naudotojas} rezultatai nesutampa. Reikia patikrinimo",
                    Sukurimo_Data = DateTime.Now,
                    Uzdarytas = false,
                    Id_Lenktynes = race.Id_Lenktynes
                };

                // Add the new entry to the skundas table
                _context.Complaint.Add(newAppeal);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine("Error in creating appeal: " + ex.Message);
            }
        }
        [HttpPost]
        public IActionResult MatchPage(int raceId, int loggedInUserId)
        {
            Console.WriteLine("Informavome apie lenktyniu baigti");
            ViewBag.RaceId = raceId;
            ViewBag.LoggedInUserId = loggedInUserId;
            return View();
        }

    }
}