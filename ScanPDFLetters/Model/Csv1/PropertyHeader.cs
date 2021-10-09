using System.Collections.Generic;

namespace ScanPDFLetters.Model.Csv
{
    public class PropertyHeader
    {
        public int? Id { get; set; }

        public string RecordType { get; set; }
        public string PropertyRef { get; set; }
        public string Ref { get; set; }
        public string PaymentRef { get; set; }
        public string TenantName { get; set; }
        public string AreaCode { get; set; }
        public string AreaName { get; set; }
        public string Scheme { get; set; }
        public string AddressDetails { get; set; }

    }
}
