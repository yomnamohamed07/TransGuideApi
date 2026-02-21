using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransGuide.Data;
using TransGuide.Data.Entities.Identity;
using TransGuide.Data.Repositories;
using TransGuide.Infrastructure.Repositories;

namespace TransGuide.Services.Services
{
    public class LocationServices : GenericRepository<UserProfile>, IGenericRepository<UserProfile>
    {
        public LocationServices(TransGuideDbContext context) : base(context)
        {
        }
    }
}
