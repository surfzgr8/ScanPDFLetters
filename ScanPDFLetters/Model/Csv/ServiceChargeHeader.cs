using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileHelpers;
using FileHelpers.Events;

namespace ScanPDFLetters.Model.Csv
{
    [DelimitedRecord(",")]
    public class ServiceChargeHeader:INotifyRead
    {
        public int? SequenceNumber;
      //  [FieldQuoted]
        public string RecordType;
      //  [FieldQuoted]
        public string PropertyRef;
      //  [FieldQuoted]
        public string ServiceChargeType;
     //   [FieldQuoted]
        public string ServiceChargeDescription;


        public void BeforeRead(BeforeReadEventArgs e)
        {
            if (e.RecordLine.Contains("MANAGECOST") || e.RecordLine.Contains("SURPDEFICIT") || e.RecordLine.Contains("SINKFUND") || e.RecordLine.Contains("SF") || e.RecordLine.Contains("SFD"))
                e.SkipThisRecord = true;
        }

        public void AfterRead(AfterReadEventArgs e)
        {

           // e.SkipThisRecord = true;

        }
    }
}
