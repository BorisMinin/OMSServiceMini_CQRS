using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OMSServiceMini.Data;
using OMSServiceMini.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using OMSServiceMini.Services;

namespace OMSServiceMini.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        readonly NorthwindContext _northwindContext;

        public OrdersController(NorthwindContext northwindContext)
        {
            _northwindContext = northwindContext;
        }

        // GET: api/orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders()
        {
            var orders = await _northwindContext.Orders
                //.Include(o => o.OrderDetails)

                .ToListAsync();
            return orders;
        }

        // GET: api/orders/10248
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _northwindContext.Orders
                .Where(o => o.OrderId == id)
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync();
            if (order == null) 
                return NotFound("Заказ с данным Id не найден");

            return order;
        }

        [HttpPost]            
        public async Task AddOrder([FromBody] Order order, CancellationToken token)
        {
            await this._northwindContext.AddAsync(order, token);
            await _northwindContext.SaveChangesAsync(token);

            var ordersByCountry = await this._northwindContext.OrdersByCountries.FirstOrDefaultAsync(x => x.CountryName == order.Customer.Country, token);

            if (ordersByCountry != null)
                ordersByCountry.OrdersCount++;
            else
                this._northwindContext.OrdersByCountries.Add(new() { CountryName = order.Customer.Country, OrdersCount = 1 });
        }
    }
}