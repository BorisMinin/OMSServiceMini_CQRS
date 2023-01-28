using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OMSServiceMini.Data;
using OMSServiceMini.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System;

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
                await _northwindContext.Orders.AddAsync(order, token);
                await _northwindContext.SaveChangesAsync(token);

                await UpdateOrdersByCountries(order, token);

                await _northwindContext.SaveChangesAsync(token);
                await transaction.CommitAsync(token);
            }
            catch (Exception)
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

            _northwindContext.Entry(order).State = EntityState.Modified;

            try
            {
                await _northwindContext.SaveChangesAsync(token);
            }
            catch (System.Exception)
            {
                throw;
            }

            return NoContent();
        }

        private async Task<string> GetCountryNameByCustomerId(string customerId, CancellationToken token)
        {
            var customer = await _northwindContext.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.CustomerId == customerId);

            return customer.Country;
        }

        private async Task UpdateOrdersByCountries(Order order, CancellationToken token)
        {
            var countryName = await GetCountryNameByCustomerId(order.CustomerId, token);

            var ordersByCountry = await _northwindContext.OrdersByCountries
                .FirstOrDefaultAsync(x => x.CountryName == countryName, token);

            if (ordersByCountry != null)
                ordersByCountry.OrdersCount++;
            else
                await _northwindContext.OrdersByCountries
                .AddAsync(new() { CountryName = countryName, OrdersCount = 1 }, token);

        }
    }
}