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
            _googleApiKey = configuration["GoogleApiKey"];
        }

        public async Task<IActionResult> FindDistance(City city)
        {
            var cities = _context.City.ToList();

            // Create lists to store origins and destinations
            var origins = new List<LocationEx> { new LocationEx(new Address(city.Pavadinimas)) };
            var destinations = cities.Select(c => new LocationEx(new Address(c.Pavadinimas))).ToList();

            var request = new DistanceMatrixRequest
            {
                Key = _googleApiKey,
                Origins = origins,
                Destinations = destinations
            };

            var response = await GoogleApi.GoogleMaps.DistanceMatrix.QueryAsync(request);

            if (response.Status == Status.Ok && response.Rows.Any())
            {
                var rows = response.Rows.ToList();

                for (int i = 0; i < rows.Count; i++)
                {
                    var row = rows[i];
                    var cityToMeasure = cities[i];

                    for (int j = 0; j < row.Elements.Count(); j++)
                    {
                        var element = row.Elements.ToList()[j];

                        if (element.Status == Status.Ok)
                        {
                            var distanceText = element.Distance.Text;
                            // Create and add Distance entity to the context
                            var distance = new Sankabinis.Models.Distance
                            {
                                CityId1 = city.Id_Miestas, // Assuming city.Id is the ID of the origin city
                                CityId2 = cityToMeasure.Id_Miestas, // Assuming cityToMeasure.Id is the ID of the destination city
                                Atstumas = element.Distance.Value
                            };

                            _context.Add(distance);
                        }
                    }
                }

                // Save all changes to the database
                await _context.SaveChangesAsync();

                return Ok("Distances calculated and saved successfully.");
            }

            return BadRequest("Failed to calculate distances.");
        }
    }
}
