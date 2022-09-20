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

        public string CreatePaymentRequest(long price,string productName)
        {
            try
            {
                StripeConfiguration.ApiKey = apikey;
                var options = new ProductCreateOptions
                {
                    Name = productName
                };
                var service = new ProductService();
                var productDetails = service.Create(options);

                var options1 = new PriceCreateOptions
                {
                    Product = productDetails.Id ,
                    UnitAmount = price*100,
                    Currency = "usd",
                };
                var service1 = new PriceService();
                var priceDetails = service1.Create(options1);


                var options2 = new SessionCreateOptions
                {
                    Mode = "payment",
                    LineItems = new List<SessionLineItemOptions>
                    {
                        new SessionLineItemOptions 
                        {
                            Price = priceDetails.Id, 
                            Quantity = 1 
                        },
                    },
                    SuccessUrl = WebLocalBaseUrl + PaymentSuccessUrl,
                    CancelUrl = "https://example.com/cancel",
                };
                var service2 = new SessionService();
                Session session = service2.Create(options2);
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
