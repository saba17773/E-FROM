using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Web.UI.Contexts;
using Web.UI.Domain;
using Web.UI.Domain.Repositories;
using Web.UI.Infrastructure.Entities;
using Web.UI.Interfaces;

namespace Web.UI.Services
{
    public class CreditControlService : ICreditControlService
    {
        private IDatabaseContext _databaseContext;
        private IDatatableService _datatablesService;
        private IAuthService _authService;

        public CreditControlService(
          IDatabaseContext databaseContext,
          IDatatableService datatablesService,
          IAuthService authService)
        {
            _databaseContext = databaseContext;
            _datatablesService = datatablesService;
            _authService = authService;
        }

        public string GetLatestRequestNumberAsync(string seqValue, string prefix)
        {
            var enUs = new CultureInfo("en-US");

            var year = DateTime.Now.ToString("yyyy", enUs);

            return $"{prefix}-{year}-{seqValue.PadLeft(6, '0')}";
        }

        public string GetLatestRequestNumberPromotionAsync(string seqValue, string prefix)
        {
            var enUs = new CultureInfo("en-US");

            var year = DateTime.Now.ToString("yyyy", enUs);
            var month = DateTime.Now.ToString("MM", enUs);
            return $"{prefix}-{year}-{month}-{seqValue.PadLeft(6, '0')}";
        }

        public string GetLatestRequestNumberMemoAsync(string seqValue, string prefix)
        {
            var enUs = new CultureInfo("en-US");

            var year = DateTime.Now.ToString("yyyy", enUs);
            var month = DateTime.Now.ToString("MM", enUs);
            return $"{prefix}-{year}-{seqValue.PadLeft(3, '0')}";
        }

        public string GetLatestRequestNumberAssetsAsync(string seqValue, string prefix)
        {
            var enUs = new CultureInfo("en-US");

            var year = DateTime.Now.ToString("yyyy", enUs);
            var month = DateTime.Now.ToString("MM", enUs);
            return $"{prefix}{year}-{seqValue.PadLeft(4, '0')}";
        }
    }
}
