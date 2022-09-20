using Nop.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.StripePayment
{
    public class StripeSettings : ISettings
    {
        /// <summary>
        /// Gets or sets client secret
        /// </summary>
        public string SecretKey { get; set; }

    }
}

