
    namespace TransGuide.Services
    {
   
        public class ServicesManager : IServicesManager
        {
            private readonly IHistoryServices _historyServices;

            public ServicesManager(IHistoryServices historyServices)
            {
                _historyServices = historyServices
                    ?? throw new ArgumentNullException(nameof(historyServices));
            }

            public IHistoryServices HistoryServices => _historyServices;
        }
    }



