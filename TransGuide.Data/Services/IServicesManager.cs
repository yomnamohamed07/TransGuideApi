
using TransGuide.Data.Respositories;

namespace TransGuide.Services
{
    public interface IServicesManager
    {
        public IHistoryServices HistoryServices{ get;  }
    }
}
