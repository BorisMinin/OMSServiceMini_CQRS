using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OMSServiceMini.AppHelpers;
using OMSServiceMini.CacheService;
using OMSServiceMini.Data;
using OMSServiceMini.Models.DenormalizedModels;
using OMSServiceMini.Models.NormalizedModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OMSServiceMini.Controllers
{
    public class StatisticsController : BaseController
    {
        private readonly NorthwindContext _northwindContext;
        private Func<CacheTech, ICacheService> _cacheService;
        private readonly string cacheKey = $"{typeof(Category)}";
        private readonly static CacheTech cacheTech = CacheTech.Memory;

        public StatisticsController(NorthwindContext northwindContext, Func<CacheTech, ICacheService> cacheService)
        {
            _northwindContext = northwindContext;
            _cacheService = cacheService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrdersByCountry>>> GetOrdersByCountries(CancellationToken token)
        {
            return await _northwindContext.OrdersByCountries.ToListAsync(token);
        }

        [HttpGet("{topCountries}")]
        public async Task<List<OrdersByCountry>> GetTopOrdersByCountriesByQuantity(CancellationToken token, [FromRoute] int topCountries)
        {
            var result = await _northwindContext.OrdersByCountries
                .AsNoTracking()
                .OrderByDescending(x =>x.OrdersCount)
                .Take(topCountries)
                .ToListAsync(token);

            var cacheService = _cacheService(cacheTech).GetCache<OrdersByCountry>(result, cacheKey);

            return result;
        }

        [HttpGet("{SalesByCategories}")]
        public async Task<ActionResult<IEnumerable<SalesByCategory>>> GetSalesByCategories(CancellationToken token)
        {
            var result = await _northwindContext.SalesByCategories
                .AsNoTracking()
                .ToListAsync(token);

            var cacheService = _cacheService(cacheTech).GetCache<SalesByCategory>(result, cacheKey);

            return result;
        }
    }
}
