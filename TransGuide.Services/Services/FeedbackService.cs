
using TransGuide.Data;
using TransGuide.Data.Entities.ApplicationEntities;
using TransGuide.Services.DTOS;

namespace TransGuide.Services.Services
{
    public class FeedbackService 
    {
        private readonly TransGuideDbContext _context;

        public FeedbackService(TransGuideDbContext context)
        {
            _context = context;
        }

        public async Task SubmitAsync(Feedback feedback)
        {
            await _context.Feedbacks.AddAsync(feedback);
            await _context.SaveChangesAsync();
        }
    }
}