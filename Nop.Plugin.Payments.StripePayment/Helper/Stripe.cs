using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Plugin.Payments.StripePayment.Models;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.StripePayment.Helper
{
    public class StripeHelper
    {
        private string apikey;
        public const string PaymentSuccessUrl = "order/history";
        public const string WebLocalBaseUrl = "https://localhost:44369/";

        public StripeHelper(string secretKey)
        {
            apikey = secretKey;
        }

        public string CreatePaymentRequest(long price)
        {
            try
            {
                StripeConfiguration.ApiKey = apikey;
                var options = new SessionCreateOptions
                {
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions
                        {
                            PriceData = new SessionLineItemPriceDataOptions
                            {
                                UnitAmount = price * 100,
                                Currency = "usd",
                                ProductData = new SessionLineItemPriceDataProductDataOptions
                                {
                                    Name = "Product",
                                },
                            },
                            Quantity = 1,
                        },
                    },
                    Mode = "payment",
                    SuccessUrl = WebLocalBaseUrl + PaymentSuccessUrl,
                    CancelUrl = "https://example.com/cancel",
                };
                var service = new SessionService();
                Session session = service.Create(options);
                var result = session.Url;
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
