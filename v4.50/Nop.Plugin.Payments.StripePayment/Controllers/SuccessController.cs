using Microsoft.AspNetCore.Mvc;
using Nop.Services.Orders;
using Stripe;
using Stripe.Checkout;

namespace Nop.Plugin.Payments.StripePayment.Controllers
{
    public class SuccessController : Controller
    {

        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IOrderService _orderService;
        private readonly StripeSettings _settings;
        public const string PaymentSuccessUrl = "checkout/completed";
        public const string WebLocalBaseUrl = "https://localhost:44369/";
        public SuccessController(IOrderProcessingService orderProcessingService, IOrderService orderService,StripeSettings settings)
        {
            _orderProcessingService = orderProcessingService;
            _orderService = orderService;
            _settings = settings;
        }
        [HttpGet("/order/success")]
        public IActionResult OrderSuccess([FromQuery] string session_id,int orderId)
        {
            StripeConfiguration.ApiKey = _settings.SecretKey;
            var sessionService = new SessionService();
            Session session = sessionService.Get(session_id);
            if(session.Status == "complete")
            {
                var order = _orderService.GetOrderByIdAsync(orderId);
                _orderProcessingService.MarkOrderAsPaidAsync(order.Result);
                return RedirectToAction("Success");
            }
            return Content("Failed");
        }
        public IActionResult Success()
        {
            return View("~/Plugins/Payments.StripePayment/Views/Success.cshtml");
        }
    }
}
