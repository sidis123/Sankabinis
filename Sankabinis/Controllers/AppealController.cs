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

            return View(appeals);
        }

        private List<Complaint> FetchAppeals()
        {
            return _context.Complaint.ToList();
        }

        public IActionResult AppealPage(int? raceId)
        {
            List<Race> matchInfo = new List<Race>();

            //int raceId = new int();

            matchInfo = FetchAppealMatch(raceId);

            List<User> matchUsers = new List<User>();

            matchUsers = FetchAppealUsers(matchInfo.First().User1Id, matchInfo.First().User2Id);

            RaceParticipantsViewModel raceParticipantsViewModel = new RaceParticipantsViewModel();
            raceParticipantsViewModel.User1 = matchUsers.First();
            raceParticipantsViewModel.User2 = matchUsers.Last();
            raceParticipantsViewModel.Race = matchInfo.First();

            return View(raceParticipantsViewModel);
        }

        private List<Race> FetchAppealMatch(int? raceID)
        {
            return _context.Race.Where(c => c.Id_Lenktynes == raceID).ToList();
        }

        private List<User> FetchAppealUsers(int? user1ID, int? user2ID)
        {
            return _context.Users.Where(c => c.Id_Naudotojas == user1ID || c.Id_Naudotojas == user2ID).ToList();
        }

        public IActionResult ChooseWinner(int id1, int id2, int appealid)
        {
            //Recalculate ELO
            //Recalucatle EXP

            IncreaseTrustScore(id1);

            DecreaseTrustScore(id2);

            int loserScore = FetchTrustScore(id2);

            if(loserScore < 20)
            {
                //warn
            }
            if(loserScore <= 0)
            {
                BanUser(id2);
            }

            CloseAppeal(appealid);
            List<Complaint> appeals = FetchAppeals();
            return View("Index", appeals);
        }

        private int IncreaseTrustScore(int id)
        {
            var winner = _context.Users.Find(id);
            winner.Pasitikimo_taskai += 10;
            _context.Users.Update(winner);
            return 0;
        }

        private int DecreaseTrustScore(int id)
        {
            var loser = _context.Users.Find(id);
            loser.Pasitikimo_taskai -= 10;
            _context.Users.Update(loser);
            return 0;
        }

        private int FetchTrustScore(int id)
        {
            return _context.Users.Find(id).Pasitikimo_taskai;
        }

        private int BanUser(int id)
        {
            var user = _context.Users.Find(id);
            user.Suspeduotos_busenos_skaicius = 1;
            _context.Users.Update(user);
            return 1;
        }

        private int CloseAppeal(int appealid)
        {
            var appeal = _context.Complaint.Where(x => x.Id_Lenktynes == appealid).First();
            appeal.Uzdarytas = true;
            _context.Complaint.Update(appeal);
            _context.SaveChanges();
            return 1;
        }

    }
}
