using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FileHelpers;

namespace ScanPDFLetters.Model.Csv
{
    [DelimitedRecord(",")]
    public class ServiceChargeDetail
    {
        public int? SequenceNumber;
       // [FieldQuoted]
        public string RecordType;
       // [FieldQuoted]
        public string PropertyRef;
      //  [FieldQuoted]
        public string ServiceChargeType;
       // [FieldQuoted]
        public string ServiceChargeDescription;
     //   [FieldQuoted]
        public string Spacer1;
        [FieldConverter(ConverterKind.Decimal, ".")]
        public decimal EstimatedSchemeCost;
        [FieldConverter(ConverterKind.Decimal, ".")]
        public decimal ActualCost;
        [FieldConverter(ConverterKind.Decimal, ".")]
        public decimal EstimatedCost;
        [FieldConverter(ConverterKind.Decimal, ".")]
        public decimal TotalCost;


    }
}
