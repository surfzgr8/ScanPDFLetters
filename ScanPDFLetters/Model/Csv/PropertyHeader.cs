using System.Collections.Generic;
using FileHelpers;
using FileHelpers.Events;

namespace ScanPDFLetters.Model.Csv
{
    [DelimitedRecord(",")]
    public class PropertyHeader: INotifyRead
    {
        public int? SequenceNumber;

        public string RecordType;
        public string PropertyRef;
        public string Ref;
        public string PaymentRef;
        public string TenantName;
        public string AreaCode;
        public string AreaName;
        public string Scheme;
        public string AddressDetails;

        public void BeforeRead(BeforeReadEventArgs e)
        {
            
                e.SkipThisRecord = true;
        }

        public void AfterRead(AfterReadEventArgs e)
        {
   
                e.SkipThisRecord = true;

        }
    }
}
