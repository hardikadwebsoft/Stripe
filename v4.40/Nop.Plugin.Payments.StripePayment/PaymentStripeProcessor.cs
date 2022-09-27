using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.StripePayment.Helper;
using Nop.Plugin.Payments.StripePayment.Models;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.StripePayment
{
    public class PaymentStripeProcessor : BasePlugin, IPaymentMethod, IMiscPlugin
    {
        private readonly ILocalizationService _localizationService;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly StripeSettings _settings;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;

        public PaymentStripeProcessor(ILocalizationService localizationService,IActionContextAccessor actionContextAccessor,
             IHttpContextAccessor httpContextAccessor,StripeSettings settings,IUrlHelperFactory urlHelperFactory, IOrderService orderService, IProductService productService)
        {
            _localizationService = localizationService;
            _actionContextAccessor = actionContextAccessor;
            _httpContextAccessor = httpContextAccessor;
            _settings = settings;
            _urlHelperFactory = urlHelperFactory;
            _orderService = orderService;
            _productService = productService;
        }
        public bool HideInWidgetList => false;

        public bool SupportCapture => false;

        public bool SupportPartiallyRefund => false;

        public bool SupportRefund => false;

        public bool SupportVoid => false;
        public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.NotSupported;

        public PaymentMethodType PaymentMethodType => PaymentMethodType.Redirection;

        public bool SkipPaymentInfo => false;

        public Task<CancelRecurringPaymentResult> CancelRecurringPaymentAsync(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            return Task.FromResult(new CancelRecurringPaymentResult { Errors = new[] { "Recurring payment not supported" } });
        }

        public Task<bool> CanRePostProcessPaymentAsync(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            //let's ensure that at least 5 seconds passed after order is placed
            //P.S. there's no any particular reason for that. we just do it
            if ((DateTime.UtcNow - order.CreatedOnUtc).TotalSeconds < 5)
                return Task.FromResult(false);

            return Task.FromResult(true);
        }

        public Task<CapturePaymentResult> CaptureAsync(CapturePaymentRequest capturePaymentRequest)
        {
            return Task.FromResult(new CapturePaymentResult { Errors = new[] { "Capture method not supported" } });
        }

        public Task<decimal> GetAdditionalHandlingFeeAsync(IList<ShoppingCartItem> cart)
        {
            return Task.FromResult(decimal.Zero);
        }

        public Task<ProcessPaymentRequest> GetPaymentInfoAsync(IFormCollection form)
        {
            return Task.FromResult(new ProcessPaymentRequest());
        }

        public async Task<string> GetPaymentMethodDescriptionAsync()
        {
            return await _localizationService.GetResourceAsync("You will be redirected to stripe site to complete the payment");
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext).RouteUrl(Stripedefaults.ConfigurationRouteName);
        }
        /// <summary>
        /// Gets a name of a view component for displaying plugin in public store ("payment info" checkout step)
        /// </summary>
        /// <returns>View component name</returns>

        public string GetPublicViewComponentName()
        {
            return "PaymentStripe";
        }

        public Task<bool> HidePaymentMethodAsync(IList<ShoppingCartItem> cart)
        {
            return Task.FromResult(false);
        }

        public override async Task InstallAsync()
        {
            await _localizationService.AddLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.Payments.Stripe.Fields.SecretKey"] = "SecretKey",
                ["Plugins.Payments.Stripe.PaymentMethodDescription"] = "Pay by Stripe payment",
            });
            await base.InstallAsync();
        }

        public async Task PostProcessPaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            List<ProductDetailModel> product = new List<ProductDetailModel>();
            StripeHelper payment = new StripeHelper(_settings.SecretKey);
            var orderItems =await _orderService.GetOrderItemsAsync(postProcessPaymentRequest.Order.Id);
            foreach (var item in orderItems)
            {
                var productDetails = _productService.GetProductByIdAsync(item.ProductId);
                product.Add(new ProductDetailModel { Name = productDetails.Result.Name, Price = (long)Math.Round(productDetails.Result.Price, 2) });
            }
            var result = payment.CreatePaymentRequest(product, postProcessPaymentRequest.Order.Id);
            if (!string.IsNullOrEmpty(result))
            {
               _httpContextAccessor.HttpContext.Response.Redirect(result);
            } 
        }

        public Task<ProcessPaymentResult> ProcessPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            return Task.FromResult(new ProcessPaymentResult());
        }

        public Task<ProcessPaymentResult> ProcessRecurringPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            return Task.FromResult(new ProcessPaymentResult { Errors = new[] { "Recurring payment not supported" } });
        }
        public Task<RefundPaymentResult> RefundAsync(RefundPaymentRequest refundPaymentRequest)
        {
            return Task.FromResult(new RefundPaymentResult { Errors = new[] { "Refund method not supported" } });
        }

        public override async Task UninstallAsync()
        {
            //Logic during uninstallation goes here...

            await base.UninstallAsync();
        }

        public Task<IList<string>> ValidatePaymentFormAsync(IFormCollection form)
        {
            return Task.FromResult<IList<string>>(new List<string>());
        }

        public Task<VoidPaymentResult> VoidAsync(VoidPaymentRequest voidPaymentRequest)
        {
            return Task.FromResult(new VoidPaymentResult { Errors = new[] { "Void method not supported" } });
        }
    }
}
