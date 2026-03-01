using AutoMapper;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransGuide.Data.Respositories;
using TransGuide.Data.Services;

namespace TransGuide.Services
{
   public class ServicesManager(Data.Respositories.IHistoryRepository historyRepository, IMapper mapper) : IServicesManager
    {
        private readonly Lazy<IHistoryServices> _historyServices = new Lazy<IHistoryServices>(() => new HistoryServices(historyRepository, mapper));
        public IHistoryServices historyservices => _historyServices.Value;

       
    }
    
}
