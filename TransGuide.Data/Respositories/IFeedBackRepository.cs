using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransGuide.Data.Entities.ApplicationEntities;

namespace TransGuide.Data.Respositories
{
    public interface IFeedBackRepository
    {
        Task AddAsync(Feedback feedback);
        Task<List<Feedback>> GetAllAsync();
    }
}
