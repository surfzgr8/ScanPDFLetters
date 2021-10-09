using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanPDFLetters.Model.Csv
{
    public class ServiceChargeDetail
    {
        public int? Id { get; set; }
        public string RecordType { get; set; }
        public string PropertyRef { get; set; }
        public string ServiceChargeType { get; set; }
        public string ServiceChargeDescription { get; set; }
        public string Spacer1 { get; set; }

        public decimal EstimatedSchemeCost { get; set; }
        public decimal EstimatedCost { get; set; }
        public decimal ActualCost { get; set; }


    }
}
