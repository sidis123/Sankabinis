using GoogleApi.Entities.Search.Common;
using Microsoft.AspNetCore.Mvc;
using Sankabinis.Data;
using Sankabinis.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Sankabinis.Controllers
{
    public class AppealController : Controller
    {
        private readonly SankabinisContext _context;

        public AppealController(SankabinisContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<Complaint> appeals = new List<Complaint>();

            appeals = FetchAppeals();

            return View();
        }

        private List<Complaint> FetchAppeals()
        {
            return _context.Complaint.ToList();
        }

        public IActionResult AppealPage()
        {
            List<Race> matchInfo = new List<Race>();

            int raceId = new int();

            matchInfo = FetchAppealMatch(raceId);

            List<User> matchUsers = new List<User>();

            matchUsers = FetchAppealUsers(matchInfo.First().User1Id, matchInfo.First().User2Id);

            return View();
        }

        private List<Race> FetchAppealMatch(int? raceID)
        {
            return _context.Race.Where(c => c.Id_Lenktynes == raceID).ToList();
        }

        private List<User> FetchAppealUsers(int? user1ID, int? user2ID)
        {
            return _context.Users.Where(c => c.Id_Naudotojas == user1ID || c.Id_Naudotojas == user2ID).ToList();
        }

    }
}
