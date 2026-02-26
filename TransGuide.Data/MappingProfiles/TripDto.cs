using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransGuide.Data.Entities.Identity;

namespace TransGuide.Data.MappingProfiles
{
    public class TripDto
    {
        public string Id { get; set; }

        public string UserLocation { get; set; }
        public string? StationName { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        public DateOnly Date { get; set; }




    }
}

