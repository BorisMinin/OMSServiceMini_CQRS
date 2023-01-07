using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OMSServiceMini.Data;
using OMSServiceMini.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OMSServiceMini.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticsController : ControllerBase
    {
        private NorthwindContext _northwindContext;

        public StatisticsController(NorthwindContext northwindContext)
        {
            this._northwindContext = northwindContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrdersByCountry>>> GetOrdersByCountries(CancellationToken token)
        {
            return await this._northwindContext.OrdersByCountries.ToListAsync(token);
        }
    }
}
