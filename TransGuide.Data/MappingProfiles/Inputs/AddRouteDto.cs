using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransGuide.Data.MappingProfiles.Inputs
{
    public class AddRouteDto
    {
        public string Name { get; set; }
        public string StartPoint { get; set; }
        public string EndPoint { get; set; }
        public string Region { get; set; }
        public string Description { get; set; }
        public decimal TicketPrice { get; set; }
        public int AverageTimeInMinutes { get; set; }
        public int RouteStatusId { get; set; }



    }
}
