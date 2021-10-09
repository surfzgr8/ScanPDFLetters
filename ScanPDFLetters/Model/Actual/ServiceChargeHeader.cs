using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileHelpers;

namespace ScanPDFLetters.Model.Actual
{
    [DelimitedRecord(",")]
    public class ServiceChargeHeader
    {
        public int? SequenceNumber;
        [FieldQuoted]
        public string RecordType;
        [FieldQuoted]
        public string PropertyRef;
        [FieldQuoted]
        public string ServiceChargeType;
        [FieldQuoted]
        public string ServiceChargeDescription;

        public List<ServiceChargeDetail> ServiceChargeDetails { get; set; } = new List<ServiceChargeDetail>();
    }
}
