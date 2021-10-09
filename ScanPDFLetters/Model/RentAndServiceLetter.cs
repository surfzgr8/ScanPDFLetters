using System.Collections.Generic;

namespace ScanPDFLetters.Model
{
    public class RentAndServiceLetter
    {
        public string PropertyRef { get; set; }

        public decimal RentsTotal { get; set; }

        public decimal ServicesTotal { get; set; }

        public decimal PrivateTotal { get; set; }

        public decimal RentServiceTotals { get; set; }

        public string PaymentFrom { get; set; }

        public string StatementDate { get; set; }

        public List<ServiceCharge> ServiceCharges { get; set; }

        public RentAndServiceLetter()
        {
            ServiceCharges = new List<ServiceCharge>();
        }

        public void Clear()
        {
            PropertyRef = string.Empty;
            RentsTotal = ServicesTotal = PrivateTotal = RentServiceTotals = 0.0m;
            ServiceCharges.Clear();
        }
    }
}
