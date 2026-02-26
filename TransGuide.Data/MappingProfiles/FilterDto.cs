using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransGuide.Data.MaPppingProfiles
{
    public class FilterDto
    {
        public string UserLocation { get; set; }

        public string StationName { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

     // public decimal  MaxDistanceKm { get; set; }



    }
}
