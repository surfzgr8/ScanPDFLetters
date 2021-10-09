using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanPDFLetters.Model.Csv
{
    public class ServiceChargeHeader
    {
        public int? Id { get; set; }
        public string RecordType { get; set; }
        public string PropertyRef { get; set; }
        public string ServiceChargeType { get; set; }
        public string ServiceChargeDescription { get; set; }
    }
}
