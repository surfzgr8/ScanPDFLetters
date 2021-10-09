using ScanPDFLetters.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Actual = ScanPDFLetters.Model.Actual;




namespace ScanPDFLetters.Helper
{


    interface IValidationReporter
    {
        bool Process(RentAndServiceLetter rentLetter,string batchNumber);
    }
    public class ValidationReporter : IValidationReporter
    {
        private readonly ICSVAdapter _csvAdapter;
        public ValidationReporter(ICSVAdapter csvAdapter)
        {
            _csvAdapter = csvAdapter;
        }

        public bool Process(RentAndServiceLetter rentLetter,string batchNumber)
        {
            var reportStringBuilder = new StringBuilder();
           // var exceptionReportFilePath = $"F:\\ScanPDFLetters\\Reports\\Exceptions\\ServiceShargeCostExceptions_{DateTime.Now.ToString("yyyyMMdd")}_{batchNumber}.txt";

            var exceptionReportFilePath = $"D:\\Code\\TGP\\ScanPDFLetters\\Reports\\Exceptions\\ServiceShargeCostExceptions_{DateTime.Now.ToString("yyyyMMdd")}_{batchNumber}.txt";


            if (!File.Exists(exceptionReportFilePath))
            { 
                File.WriteAllText(exceptionReportFilePath, "ExceptionReport:\r\n");
            } 
            
           
            var csvServiceChargeHeaderList
                = _csvAdapter.GetServiceChargesByPropertReference(rentLetter.PropertyRef);

            var rentLetterActualServiceChargeList
                = rentLetter.ServiceCharges;

            var csvServiceChargeDetailList = csvServiceChargeHeaderList?.FirstOrDefault()?.ServiceChargeDetails ?? null;

            // we may have now SCH or SCD recoeds in the csv file
            if (csvServiceChargeDetailList !=null)
            {
                foreach (var rentLetterActualServiceCharge in rentLetterActualServiceChargeList)
                {

                    // Csv Service charge found in PDF RentServiceLetter
                    var csvServiceCharge = csvServiceChargeDetailList.FirstOrDefault(scd => scd.ServiceChargeDescription == rentLetterActualServiceCharge.Description);

                    if (csvServiceCharge != null && csvServiceCharge.ServiceChargeDescription == "Service Charge Discount")
                    { 
                    }

                    if (csvServiceCharge != null && csvServiceCharge.ServiceChargeDescription == " Service Charge Discount")
                    {
                    }
                    // now compare charges, but ignore Service Charge Discounts as these can be duplicated
                    if (csvServiceCharge != null 
                        && csvServiceCharge.ServiceChargeDescription != "Service Charge Discount")
                    {
                        // Actual cost diff
                        if (csvServiceCharge.ActualCost != rentLetterActualServiceCharge.ActualCost)
                        {
                            reportStringBuilder.Clear();
                            reportStringBuilder.AppendLine($"Property Reference:{rentLetter.PropertyRef} has service charges difference");
                            reportStringBuilder.AppendLine($"For Service Type:{rentLetterActualServiceCharge.Description}");
                            reportStringBuilder.AppendLine($"Csv ActualCost:{csvServiceCharge.ActualCost}");
                            reportStringBuilder.AppendLine($"Pdf ActualCost:{rentLetterActualServiceCharge.ActualCost}");

                            File.AppendAllText(exceptionReportFilePath, reportStringBuilder.ToString());
                        }

                        // Estimated cost diff
                        if (csvServiceCharge.EstimatedCost != rentLetterActualServiceCharge.YourEstimatedCost)
                        {
                            reportStringBuilder.Clear();
                            reportStringBuilder.AppendLine($"Property Reference:{rentLetter.PropertyRef} has service charges difference");
                            reportStringBuilder.AppendLine($"For Service Type:{rentLetterActualServiceCharge.Description}");
                            reportStringBuilder.AppendLine($"Csv EstimatedCost:{csvServiceCharge.EstimatedCost}");
                            reportStringBuilder.AppendLine($"Pdf EstimatedCost:{rentLetterActualServiceCharge.YourEstimatedCost}");

                            File.AppendAllText(exceptionReportFilePath, reportStringBuilder.ToString());
                        }

                        // Area Estimated cost diff
                        if (csvServiceCharge.EstimatedSchemeCost != rentLetterActualServiceCharge.AreaEstimatedCost)
                        {
                            reportStringBuilder.Clear();
                            reportStringBuilder.AppendLine($"Property Reference:{rentLetter.PropertyRef} has service charges difference");
                            reportStringBuilder.AppendLine($"For Service Type:{rentLetterActualServiceCharge.Description}");
                            reportStringBuilder.AppendLine($"Csv Scheme Cost:{csvServiceCharge.EstimatedSchemeCost}");
                            reportStringBuilder.AppendLine($"Pdf Scheme Cost:{rentLetterActualServiceCharge.AreaEstimatedCost}");

                            File.AppendAllText(exceptionReportFilePath, reportStringBuilder.ToString());
                        }
                    }
                    else { return false; }
                }
            }


            return true;
        }
    }
}
