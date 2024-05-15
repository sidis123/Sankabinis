using Microsoft.AspNetCore.Mvc;
using Sankabinis.Data;
using Sankabinis.Models;
using System.Collections.Generic;
using System.Linq;

public class LeaderController : Controller
{

    private readonly SankabinisContext _context;

    public LeaderController(SankabinisContext context)
    {
        _context = context;
    }

    // Action method to get leaderboard data
    public IActionResult GetLeaderboardData()
    {
        var leaders = GetLeaderTable();
        return Json(leaders); // Return leaders as JSON
    }

    private List<User> GetLeaderTable()
    {
        // Fetch top 10 users based on Elo
        var leaders = _context.Users.OrderByDescending(u => u.Elo).Take(10).ToList();
        return leaders;
    }


}
