using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Payments.StripePayment.Models
{
    public class ProductDetailModel
    {
        public string Name { get; set; }
        public long Price { get; set; }
    }
}
