using AutoMapper;
using TransGuide.Data.Respositories;
using TransGuide.Data.Services;
using TransGuide.Data.MappingProfiles;
using TransGuide.Data.Entities.Identity;

namespace TransGuide.Services
{
    public class HistoryServices(IHistoryRepository _historyRepository, IMapper _mapper) : IHistoryServices
    {
        public async Task<HistoryDto> CreateorUpdateHistoryAsync(HistoryDto historydto)
        {
            var history = _mapper.Map<HistoryDto, History>(historydto);
            var created = await _historyRepository.CreateorUpdateHistoryAsync(history);
            if (created is not null)
            {
                return await GetHistoryAsync(history.Id);
            }
            else
            {
                throw new Exception("Cant Create or Update History Now Try it Later");
            }
        }

  

        public async Task<bool> DeleteHistoryAsync(string Key)
        => await _historyRepository.DeleteHistoryAsync(Key);

        public async Task<HistoryDto> GetHistoryAsync(string key)
        {
            var history = await _historyRepository.GetHistoryAsync(key);
            if (history is not null)
            {
                return _mapper.Map<History, HistoryDto>(history);
            }
            else
            {
                throw new Exception("Cant Create or Update History Now Try it Later");
            }
        }

    
    }
}

