using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Sankabinis.Data;
using Sankabinis.Models;
using GoogleApi.Entities.Search.Video.Common;

namespace Sankabinis.Controllers
{
    public class EloController : Controller
    {
        private readonly SankabinisContext _context;

        public EloController(SankabinisContext context)
        {
            _context = context;
        }
        private int oldElo1;
        private int oldElo2;
        private Car car1;
        private Car car2;
        private int newElo1;
        private int newElo2;

        // This method is assumed to be called to calculate and update the Elo scores for two users
        public async Task<IActionResult> Init(int id1, int id2, AutomobilioKlase klase)
        {
            await GetElo(id1, id2);

            var user1 = await _context.Users.FindAsync(id1);
            var user2 = await _context.Users.FindAsync(id2);

            double E1 = CalculatePredictions(oldElo1, oldElo2);
            double E2 = CalculatePredictions(oldElo2, oldElo1);

            double K1 = CalculateK(oldElo1, user1.Laimėta_lenktyniu, user1.Pralaimėta_lenktyniu);
            double K2 = CalculateK(oldElo2, user2.Laimėta_lenktyniu, user2.Pralaimėta_lenktyniu);

            await FetchCars(id1, id2, klase);

            double P1 = CalculateP(car1);
            double P2 = CalculateP(car2);


            double S1 = 1;
            double S2 = 0;

            CalculateChange(K1, P1, S1, E1, K2, P2, S2, E2);

            if (newElo1 < 200 || newElo2 < 200)
                SetEloLow(id1, id2);
            if (newElo1 > 1000 || newElo2 > 1000)
                SetElowHigh(id1, id2);


            //SetElo()
            user1.Elo = newElo1;
            user2.Elo = newElo2;

            _context.Update(user1);
            _context.Update(user2);
            await _context.SaveChangesAsync();

            return Ok(new { User1NewElo = newElo1, User2NewElo = newElo2 });
        }

        private async Task GetElo(int id1, int id2)
        {
            var user1 = await _context.Users.FindAsync(id1);
            var user2 = await _context.Users.FindAsync(id2);

            if (user1 == null || user2 == null)
            {
                NotFound("One or both users not found.");
            }
            if (user1.Elo <= 200)
                oldElo1 = 200;
            else if (user2.Elo <= 200)
                oldElo2 = 200;
            else
                oldElo1 = user1.Elo;
                oldElo2 = user2.Elo;

        }
        private async Task FetchCars(int id1, int id2, AutomobilioKlase klase)
        {
            car1 = await _context.Car.FirstOrDefaultAsync(c => c.Fk_Naudotojasid_Naudotojas == id1 && c.Klase == klase);
            car2 = await _context.Car.FirstOrDefaultAsync(c => c.Fk_Naudotojasid_Naudotojas == id2 && c.Klase == klase);

            if (car1 == null || car2 == null)
            {
                NotFound("One or both cars not found for the specified class.");
            }
        }

        private double CalculatePredictions(int elo1, int elo2)
        {
            return 1.0 / (1 + Math.Pow(10, ((double)(elo2 - elo1) / 400)));
        }

        private double CalculateP(Car car)
        {
            // Example scoring based on the provided code and logic
            double powerToWeightRatio = car.Galingumas / car.Svoris;
            double powerToWeightScore = GetPowerToWeightScore(powerToWeightRatio);

            double fuelTypeScore = car.Kuro_tipas switch
            {
                KuroTipas.Benzinas => 3,
                KuroTipas.BenzinasElektra => 3,
                KuroTipas.Dyzelis => 3,
                KuroTipas.Dujos => 3,
                KuroTipas.Elektra => 1,
                _ => 1
            };

            double transmissionScore = car.Pavaru_deze switch
            {
                PavaruDeze.Automatine => 2,
                PavaruDeze.Mechanine => 3,
                _ => 1
            };

            double bodyTypeScore = car.Kebulas switch
            {
                Kebulas.Sedanas => 2,
                Kebulas.Hecbeakas => 2,
                Kebulas.Universalas => 3,
                Kebulas.Visureigis => 3,
                Kebulas.Pikapas => 3,
                Kebulas.Kupe => 2,
                Kebulas.Kabrioletas => 2,
                Kebulas.Vienaturis => 4,
                _ => 1
            };

            double maxScore = 4 + 3 + 3 + 4; // Sum of maximum possible scores
            double totalScore = powerToWeightScore + fuelTypeScore + transmissionScore + bodyTypeScore;

            return totalScore / maxScore;
        }

        private double GetPowerToWeightScore(double powerToWeightRatio)
        {
            if (powerToWeightRatio < 0.05) return 1;
            if (powerToWeightRatio < 0.1) return 2;
            if (powerToWeightRatio < 0.2) return 3;
            return 4;
        }

        private double RaceCoefficient(int wonRaces, int lostRaces)
        {
            if (lostRaces == 0)
                return 1.5; // Maximum boost if no races lost
            double ratio = (double)wonRaces / lostRaces;
            return 1.0 + 0.5 / (ratio + 1); // A ratio that decreases with more wins
        }

        private double CalculateK(int elo, int wonRaces, int lostRaces)
        {
            double baseK = 32; //base factor
            double raceCoefficient = RaceCoefficient(wonRaces, lostRaces);
            return baseK * raceCoefficient; // Adjust K with race coefficient
        }
        public void CalculateChange(double K1, double P1, double S1, double E1, double K2, double P2, double S2, double E2)
        {
            newElo1 = (int)(oldElo1 + (K1 * P1) * (S1 - E1));
            newElo2 = (int)(oldElo2 + (K2 * P2) * (S2 - E2));
        }
        public void SetEloLow(int id1, int id2)
        {
            if(newElo1 < 200)
            {
                newElo1 = 200;
                string content = $"user with id = {id1} has elo lower than 200";
               // _context.Messages.Add(new HomeScreenMessages(id1, content));
            }
            if(newElo2 < 200)
            {
                newElo2 = 200;
                string content = $"user with id = {id2} has elo lower than 200";
               // _context.Messages.Add(new HomeScreenMessages(id2, content));
            }
        }
        public void SetElowHigh(int id1, int id2)
        {
            if (newElo1 > 1000)
            {
                newElo1 = 1000;
                string content = $"user with id = {id1} has elo higher than 1000";
               // _context.Messages.Add(new HomeScreenMessages(id1, content));
            }
            if (newElo2 > 1000)
            {
                newElo2 = 1000;
                string content = $"user with id = {id2} has elo higher than 1000";
               // _context.Messages.Add(new HomeScreenMessages(id2, content));
            }

        }

    }
}
