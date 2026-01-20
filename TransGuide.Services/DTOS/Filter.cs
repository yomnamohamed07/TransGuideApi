using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransGuide.Services.DTOS
{
    public class Filter
    {
        public string? StationName { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

      
    }
}
