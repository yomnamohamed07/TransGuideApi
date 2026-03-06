
using TransGuide.Data;
using TransGuide.Data.Entities.ApplicationEntities;
using TransGuide.Data.Respositories;
using TransGuide.Infrastructure.Repositories;

namespace TransGuide.Infrustructure.Respositories
{
    public class FeedbackRepository : GenericRepository<Feedback> ,IFeedbackRepository
    {
        private readonly TransGuideDbContext context;

        public FeedbackRepository(TransGuideDbContext context) : base(context)
        {
            this.context = context;
        }
    }


}
