using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OMSServiceMini.Data;
using OMSServiceMini.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OMSServiceMini.Controllers
{
    public class StatisticsController : BaseController
    {
        private NorthwindContext _northwindContext;

        public StatisticsController(NorthwindContext northwindContext)
        {
            _northwindContext = northwindContext;
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
                .ToListAsync();

            return result;
        }
    }
}
