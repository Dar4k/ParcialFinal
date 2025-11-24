using System;

// BAD: Mixing minimal APIs with Controllers folder just to confuse structure
namespace WebApi.Controllers
{
    public class OrdersController /* No ControllerBase, no attributes: unused on purpose */ 
    {
        protected OrdersController() { }
        public const string Message = "This controller does nothing. Endpoints are in Program.cs";
    }
}
