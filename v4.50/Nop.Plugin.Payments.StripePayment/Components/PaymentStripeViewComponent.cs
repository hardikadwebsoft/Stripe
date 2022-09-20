using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.StripePayment.Components
{
    [ViewComponent(Name = "PaymentStripe")]
    public class PaymentStripeViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View("~/Plugins/Payments.StripePayment/Views/PaymentInfo.cshtml");
        }
    }
}
