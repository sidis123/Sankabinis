using Microsoft.AspNetCore.Mvc;
using Sankabinis.Data;
using Sankabinis.Models;
using GoogleApi.Entities.Maps.DistanceMatrix.Request;
using GoogleApi.Entities.Maps.DistanceMatrix.Response;
using GoogleApi.Entities.Common;
using GoogleApi.Entities.Common.Enums;
using GoogleApi.Entities.Maps.Common;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Sankabinis.Controllers
{
    public class GoogleApiController : Controller
    {
        private readonly SankabinisContext _context;
        private readonly string _googleApiKey;

        public GoogleApiController(SankabinisContext context, IConfiguration configuration)
        {
            _context = context;
            _googleApiKey = ":)";
        }

        public IActionResult FindDistance(City city)
        {
            var cities = _context.City.Where(c => c.Id_Miestas != city.Id_Miestas).ToList();

            var origin = new LocationEx(new Address(city.Pavadinimas));

            foreach (var destinationCity in cities)
            {
                // Check if distance already exists in the database
                if (!_context.Distance.Any(d => d.CityId1 == city.Id_Miestas && d.CityId2 == destinationCity.Id_Miestas))
                {
                    var request = new DistanceMatrixRequest
                    {
                        Key = _googleApiKey,
                        Origins = new List<LocationEx> { origin },
                        Destinations = new List<LocationEx> { new LocationEx(new Address(destinationCity.Pavadinimas)) }
                    };

                    var response = GoogleApi.GoogleMaps.DistanceMatrix.QueryAsync(request).Result;

                    if (response.Status == Status.Ok && response.Rows.Any())
                    {
                        var row = response.Rows.First();
                        var element = row.Elements.FirstOrDefault();

                        if (element != null && element.Status == Status.Ok)
                        {
                            // Create and add Distance entity to the context
                            var distance = new Sankabinis.Models.Distance
                            {
                                CityId1 = city.Id_Miestas, // Assuming city.Id is the ID of the origin city
                                CityId2 = destinationCity.Id_Miestas, // Assuming destinationCity.Id is the ID of the destination city
                                Atstumas = element.Distance.Value / 1000
                            };

                            _context.Distance.Add(distance);
                            _context.SaveChanges();
                        }
                    }
                }
            }

            // Save all changes to the database
            _context.SaveChanges();

            return Ok("Distances calculated and saved successfully.");
        }

    }
}
