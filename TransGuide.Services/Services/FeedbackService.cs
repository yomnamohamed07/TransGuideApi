using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public async Task SubmitAsync(UserFeedback feedback)
        {
            await _context.Feedbacks.AddAsync(feedback);
            await _context.SaveChangesAsync();
        }
    }
}