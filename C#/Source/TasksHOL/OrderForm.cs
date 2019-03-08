using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HOL
{
    public class OrderForm
    {
        public int OrderId { get; set; }
        
        public string Name{ get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public bool AddressVerified { get; set; }

        public string TaxIdentifier { get; set; }
        public int CreditScore { get; set; }
        public bool Eligible { get; set; }

        public double SalesTaxRate { get; set; }
        public string  PaymentAuthorization { get; set; }
        public string OrderStatus { get; set; }
        
        public DateTime OrderDate { get; set; }
        public DateTime ShipDate { get; set; }
        public DateTime CancelDate { get; set; }

        public override string ToString()
        {
            return "ID: " + OrderId + ", Action: " + OrderStatus;
        }
    }
}
