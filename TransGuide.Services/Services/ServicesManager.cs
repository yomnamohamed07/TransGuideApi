using AutoMapper;


namespace TransGuide.Services
{
   public class ServicesManager(Data.Respositories.IHistoryRepository historyRepository, IMapper mapper) : IServicesManager
    {
        private readonly Lazy<IHistoryServices> _historyServices = new Lazy<IHistoryServices>(() => new HistoryServices(historyRepository, mapper));
        public IHistoryServices historyservices => _historyServices.Value;

       
    }
    
}
