using AspNetCore;
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
            Race matchInfo = new Race();

            matchInfo = FetchAppealMatch();

            List<User> matchUsers = new List<User>();

            matchUsers = FetchAppealUsers(matchInfo.User1Id, matchInfo.User2Id);

            return View();
        }

        private Race FetchAppealMatch(int? raceID )
        {
            return _context.Race.Where(c => c.Id_Lenktynes == raceID);
        }

        private List<User> FetchAppealUsers(int? user1ID, int? user2ID)
        {
            return _context.User.Where(c => c.Id_Naudotojas == user1ID || c.Id_Naudotojas == user2ID).ToList();
        }

    }
}
