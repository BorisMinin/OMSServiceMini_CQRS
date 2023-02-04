using Microsoft.AspNetCore.Mvc;
using OMSServiceMini.Data;

namespace OMSServiceMini.Controllers
{
    public class ShippersController : BaseController
    {
        private readonly NorthwindContext _northwindContext;

        public ShippersController(NorthwindContext northwindContext)
        {
            _northwindContext = northwindContext;
        }
    }
}