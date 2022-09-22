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
        public const string PaymentSuccessUrl = "checkout/completed";
        public const string WebLocalBaseUrl = "https://localhost:44369/";

        public StripeHelper(string secretKey)
        {
            apikey = secretKey;
        }

        public string CreatePaymentRequest(List<ProductDetailModel> productDetailModel)
        {
            try
            {
                StripeConfiguration.ApiKey = apikey;
                var LineItems = new List<SessionLineItemOptions>();
                foreach (var item in productDetailModel)
                {
                    LineItems.Add(new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "usd",
                            UnitAmount = item.Price * 100,
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Name
                            },
                        },
                        Quantity = 1
                    });
                    
                }
                var options = new SessionCreateOptions
                {

                    LineItems = LineItems,
                    Mode = "payment",
                    SuccessUrl = WebLocalBaseUrl + PaymentSuccessUrl,
                    CancelUrl = "https://example.com/cancel",
                };
                var service2 = new SessionService();
                Session session = service2.Create(options);
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
