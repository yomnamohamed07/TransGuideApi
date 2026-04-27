using AutoMapper;
using TransGuide.Data.Entities.Identity;
using TransGuide.Data.MappingProfiles;
using TransGuide.Data.Respositories;
using TransGuide.Services;

public class HistoryServices : IHistoryServices
{
    private readonly IHistoryRepository _repository;
    private readonly IMapper _mapper;

    public HistoryServices(IHistoryRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<HistoryDto> GetHistoryAsync(string userId)
    {
        var history = await _repository.GetHistoryAsync(userId);

        if (history == null)
            return new HistoryDto
            {
                UserId = userId,
                Trips = new List<TripDto>()
            };

        var dto = _mapper.Map<HistoryDto>(history);
        dto.Trips ??= new List<TripDto>();

        return dto;
    }

    public async Task<HistoryDto> CreateorUpdateHistoryAsync(HistoryDto dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.UserId))
            throw new ArgumentException("Invalid history data");

        dto.Trips ??= new List<TripDto>();

        var entity = _mapper.Map<History>(dto);

        var result = await _repository.CreateorUpdateHistoryAsync(entity);

        if (result == null)
            throw new Exception("Failed to save history");

        return await GetHistoryAsync(dto.UserId);
    }

    public Task<bool> DeleteHistoryAsync(string userId)
        => _repository.DeleteHistoryAsync(userId);

    public Task<bool> DeleteTripFromHistoryAsync(string userId, string tripId)
        => _repository.DeleteTripFromHistoryAsync(userId, tripId);

    public Task<long> GetTotalTripsAsync()
    => _repository.GetTotalTripsCountAsync();
}