using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransGuide.Data.Entities.ApplicationEntities;
using TransGuide.Data.MappingProfiles;

namespace TransGuide.Data.Services
{
    public interface IFeedbackService
    {
        Task<bool> SubmitFeedbackAsync(FeedbackDto dto);

        public Task<IEnumerable<FeedbackViewDto>> GetAllFeedBacks();
        Task<int> CountFeedbacks();
    }
}
