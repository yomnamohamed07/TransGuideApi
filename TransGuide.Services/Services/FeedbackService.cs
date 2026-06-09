
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TransGuide.Data;
using TransGuide.Data.Entities.ApplicationEntities;
using TransGuide.Data.Entities.Identity;
using TransGuide.Data.MappingProfiles;
using TransGuide.Data.Repositories;
using TransGuide.Data.Services;
using TransGuide.Services.DTOS;

namespace TransGuide.Services.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IGenericRepository<Feedback> _feedbackRepo;
        private readonly IGenericRepository<UserProfile> _userRepo;
        private readonly IGenericRepository<Rating> _ratingRepo;
        private readonly IGenericRepository<Route> _routeRepo;
        private readonly IGenericRepository<TripStatus> _tripStatusRepo;
        private readonly IMapper _mapper;

        public FeedbackService(
            IGenericRepository<Feedback> feedbackRepo,
            IGenericRepository<UserProfile> userRepo,
            IGenericRepository<Rating> ratingRepo,
            IGenericRepository<Route> routeRepo,
            IGenericRepository<TripStatus> tripStatusRepo,
            IMapper mapper)
        {
            _feedbackRepo = feedbackRepo;
            _userRepo = userRepo;
            _ratingRepo = ratingRepo;
            _routeRepo = routeRepo;
            _tripStatusRepo = tripStatusRepo;
            _mapper = mapper;
        }

        public async Task<bool> SubmitFeedbackAsync(FeedbackDto dto)
        {
            // Field + Business + Referential Validation
            if (dto == null) return false;
            if (string.IsNullOrWhiteSpace(dto.FullName)) return false;
            if (string.IsNullOrWhiteSpace(dto.Email)) return false;
            if (!await _userRepo.IsExist(dto.UserProfileId)) return false;
            if (!await _ratingRepo.IsExist(dto.RatingId)) return false;
            if (!await _routeRepo.IsExist(dto.RouteId)) return false;
            if (!await _tripStatusRepo.IsExist(dto.TripStatusId)) return false;
            if (dto.RatingId < 1 || dto.RatingId > 5) return false;

            // Mapping & Save
            var feedback = _mapper.Map<Feedback>(dto);
            feedback.CreatedAt = DateTime.UtcNow;

            var result = await _feedbackRepo.AddAsync(feedback);
            return result != null;

            
        }
        public async Task<IEnumerable<FeedbackViewDto>> GetAllFeedBacks()
        {
            var result = await _feedbackRepo.GetAllAsync();

            if (result == null || !result.Any())
                return Enumerable.Empty<FeedbackViewDto>();

            return result.Select(x => new FeedbackViewDto
            {
                Email = x.Email,
                Message = x.Message
            });
        }
        public async Task<int> CountFeedbacks()
        {
            return await _feedbackRepo.CountAsync();
        }
    }

}
