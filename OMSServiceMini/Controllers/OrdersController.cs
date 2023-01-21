using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OMSServiceMini.Data;
using OMSServiceMini.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace OMSServiceMini.Controllers
{
    public class OrdersController : BaseController
    {
        readonly NorthwindContext _northwindContext;

        public OrdersController(NorthwindContext northwindContext)
        {
            _northwindContext = northwindContext;
        }

        // GET: api/orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders(CancellationToken token)
        {
            var orders = await _northwindContext.Orders
                //.Include(o => o.OrderDetails)
                .AsNoTracking()
                .ToListAsync(token);
            return orders;
        }

        // GET: api/orders/10248
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id, CancellationToken token)
        {
            var order = await _northwindContext.Orders
                .AsNoTracking()
                .Where(o => o.OrderId == id)
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(token);
            if (order == null)
                return NotFound("Заказ с данным Id не найден");

            return order;
        }

        [HttpPost]
        public async Task AddOrder([FromBody] Order order, CancellationToken token)
        {
            using var transaction = await _northwindContext.Database.BeginTransactionAsync();

            try
            {
                await this._northwindContext.Orders.AddAsync(order, token);
                await this._northwindContext.SaveChangesAsync(token);

                var countryName = await GetCountryNameByCustomerId(order.CustomerId);

                var ordersByCountry = await this._northwindContext.OrdersByCountries
                    .FirstOrDefaultAsync(x => x.CountryName == order.Customer.Country, token);

                if (ordersByCountry != null)
                    ordersByCountry.OrdersCount++;
                else
                    await this._northwindContext.OrdersByCountries
                    .AddAsync(new() { CountryName = order.Customer.Country, OrdersCount = 1 }, token);

                await this._northwindContext.SaveChangesAsync(token);
                await transaction.CommitAsync(token);
            }
            catch (System.Exception)
            {
                await transaction.RollbackAsync(token);
                throw;
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, Order order, CancellationToken token)
        {
            if (id != order.OrderId)
                return BadRequest();

            this._northwindContext.Entry(order).State = EntityState.Modified;

            try
            {
                await this._northwindContext.SaveChangesAsync(token);
            }
            catch (System.Exception)
            {
                throw;
            }

            return NoContent();
        }

        private async Task<string> GetCountryNameByCustomerId(string customerId)
        {
            var customer = await this._northwindContext.Customers
                .FirstOrDefaultAsync(x => x.CustomerId == customerId);

            return customer.Country;
        }
    }
}