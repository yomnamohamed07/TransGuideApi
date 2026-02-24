using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransGuide.Data.Respositories;

namespace TransGuide.Services
{
    public class IServicesManager
    {
        public IHistoryRepository HistoryRepository { get; set; }
    }
}
