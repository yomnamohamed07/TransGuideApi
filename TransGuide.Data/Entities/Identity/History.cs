using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransGuide.Data.Entities.Identity
{
    public class History
    {
      
        public string  UserId { get; set; }
        public List<Trip> Trips { get; set; } = new List<Trip>();


    }
}
