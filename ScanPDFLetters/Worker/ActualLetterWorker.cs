using System.Configuration;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using System;
using DataAccessLayer.Classes;
using ScanPDFLetters.Model;
using ScanPDFLetters.Helper;


namespace ScanPDFLetters.Worker
{
    internal class ActualLetterWorker
    {
        private const string StartPage1 = "Your payment reference";
        private const string StartPage2 = "Your rent and service charge – important information";
        private const string StartPage3 = "Your rent – important information";
        private const string StartPage4 = "Privacy Notice";
        private const string EndPage1 = "Call Customer services";
        private const string EndPage2 = "Your rent and service charge – important information";
        private const string EndPage3 = "Your rent – important information";
        private const string RentTotal = "Rent £";
        private const string ServicesTotal = "Service charge £";
        private const string RentServiceTotal = "Total £";
        private const string PrivateTotal = "Private charges £";
        private const string WeeklyRentServiceTotalIgnore = " per week";
        private const string MonthlyRentServiceTotalIgnore = " per month";
        private const string StartOfServiceCharge = "£ £";
        private const string WeeklyEndOfServiceCharge = "Your weekly charge will be £";
        private const string MonthlyEndOfServiceCharge = "Your monthly charge will be £";
        private const char NewLine = '\n';
        private const char PoundSign = '£';
        private const char Space = ' ';
        private const string OpenBracket = "(";
        private const string CloseBracket = ")";
        private const string Management = "Management fee";
        private const string SurplusDeficit = "Surplus deficit";
        private const string ManualSurplusDeficit = "Manual Surplus Deficit";
        private const string BuildingInsurance = "Buildings Insurance";
        private const string AccountancyFee = "Accountancy Fee";
        private const string PropertyReference = "Property reference:";
        private const string IgnoreText = "Description Estimated cost Your estimated";
        private const string PaymentFrom = "Your new payment from";
        private const string StatementDate = "Estimated costs for the year";
        private const string PDFCount = "PDF Count :{0}";
        private const string AddLetter = "insert into Validate.RaSQLLetters(BatchNo, FilePath, FileName, DatabaseName,FinancialYear) values ({0}, '{1}', '{2}', '{3}', {4}); select SCOPE_IDENTITY();";
        private const string AddHeader = "insert into Validate.RaSQLHeaderLetters(PropertyRef, RentsTotal, ServicesTotal, PrivateTotal, RentServiceTotal, HeaderDate, StatementDate, RaSQLLetterID) values('{0}', {1}, {2}, {3}, {4}, '{5}', '{6}', {7})";
        private const string AddServices = "insert into Validate.RaSQLServicesLetter(PropertyRef, Service, AreaEstimatedCost, YourEstimatedCost, RaSQLLetterID) values('{0}', '{1}', {2}, {3}, {4})";
        private const int PropertyReferenceStringLength = 7;

        private RentAndServiceLetter rentServiceLetter;

        public Label PDFStatus { get; set; }

        public string ConnectionName { get; set; }

        public string FilePath { get; set; }

        public StringBuilder lines { get; set; }

        private int RaSQLLetterID { get; set; }

        //public event OnNotifyStartLetterProcess();

        public ActualLetterWorker()
        {
        }

        public ActualLetterWorker(StringBuilder sb)
        {
            lines = sb;
            rentServiceLetter = new RentAndServiceLetter();
        }

        public void Clear()
        {
            lines.Clear();
            rentServiceLetter.Clear();
        }

        public void ProcessLetters(Action<int,RentAndServiceLetter> OnNotifyLetterProcessed ,List<string> lstStartLettter, List<string> lstEndLetter)
        {
            var startLetter = GetStartLetter(0);
            var endLetter = GetEndLetter(0);
            var letterCount = 0;

            var documentSection = lines.ToString(startLetter, endLetter - startLetter);

            while (startLetter != -1 && endLetter != -1)
            {
                letterCount++;
           
                var propRef = GetPropertyReference(documentSection);//?? GetServiceCharges(documentSection); 

                // C# 8 can use below snippet
                //propRef ??= GetServiceCharges(documentSection);

                if (!string.IsNullOrEmpty(propRef))
                {
                    GetServiceCharges(documentSection);
                    // Validate with Csv
                    OnNotifyLetterProcessed(letterCount, this.rentServiceLetter);
                  
                }
             
                this.rentServiceLetter.Clear();
              
                startLetter = GetStartLetter(endLetter);

                if (startLetter != -1)
                {
                    endLetter = GetEndLetter(endLetter + 25);

                    // ? end of file

                    if(endLetter !=-1)
                        documentSection = lines.ToString(startLetter, endLetter - startLetter);
                }

                Application.DoEvents();
                
            }
        }




        private int FindMatch(List<string> search)
        {
            int result = 0;

            foreach (string str in search)
            {
                if (lines.ToString().IndexOf(str) != -1 && result == 0)
                {
                    result = lines.ToString().IndexOf(str);
                }
            }

            return result;
        }




        private void WriteToDebug(string lines)
        {
            string fileName = $"F:\\ScanPDFLetters\\DebugFiles\\PDF_Debug_{DateTime.Now.ToString("yyyymmdd_HHmmssfff")}.txt";


            File.WriteAllText(fileName, lines.ToString());
        }

        private void WriteToDebug(string lines,string propertyRef)
        {
            string fileName = $"F:\\ScanPDFLetters\\DebugFiles\\{propertyRef}_{DateTime.Now.ToString("yyyymmdd_HHmmssfff")}.txt";


            File.WriteAllText(fileName, lines.ToString());
        }

 

        private int GetStartLetter(int startPos)
        {
            int num = lines.ToString().IndexOf(StartPage1, startPos);

            if (num == -1)
            {
                if (lines.ToString().IndexOf(StartPage2, startPos) != -1)
                    num = lines.ToString().IndexOf(StartPage2, startPos);
                else if (lines.ToString().IndexOf(StartPage3, startPos) != -1)
                    num = lines.ToString().IndexOf(StartPage3, startPos);
                else if (lines.ToString().IndexOf(StartPage4, startPos) != -1)
                    num = lines.ToString().IndexOf(StartPage4, startPos);
            }
            return num;
        }
        private int GetEndLetter(int startPos)
        {
            int startIndex = -1;

            // ignore first line of doc
            if (startPos == 0)
            {
                startPos = 15;
            }
            
             startIndex = lines.ToString().IndexOf(EndPage1, startPos);
            

            if (startIndex == -1)
            {
                // Some letters do not have any servie charges !!!
                System.Diagnostics.Debug.WriteLine("End letter not found");
            }

            return startIndex;
        }

        private void GetPaymentFrom(int startPos)
        {
            int startIndex = lines.ToString().IndexOf("Your new payment from", startPos) + "Your new payment from".Length + 1;
            rentServiceLetter.PaymentFrom = lines.ToString().Substring(startIndex, lines.ToString().IndexOf('\n', startIndex) - startIndex);
        }

        private void GetStatementDate(int startPos)
        {
            int startIndex = lines.ToString().IndexOf("Estimated costs for the year", startPos) + "Estimated costs for the year".Length + 1;
            rentServiceLetter.StatementDate = lines.ToString().Substring(startIndex, lines.ToString().IndexOf('\n', startIndex) - startIndex);
        }

        private void GetTotalCharges(int startPos, int endPos)
        {
            decimal result;
            if (GetText(startPos, endPos, "Rent £") != string.Empty)
            {
                decimal.TryParse(GetText(startPos, endPos, "Rent £"), out result);
                rentServiceLetter.RentsTotal = result;
            }
            if (GetText(startPos, endPos, "Service charge £") != string.Empty)
            {
                decimal.TryParse(GetText(startPos, endPos, "Service charge £"), out result);
                rentServiceLetter.ServicesTotal = result;
            }
            if (GetText(startPos, endPos, "Total £") != string.Empty)
            {
                decimal.TryParse(GetText(startPos, endPos, "Total £").Replace(" per week", string.Empty).Replace(" per month", string.Empty), out result);
                rentServiceLetter.RentServiceTotals = result;
            }
            if (!(GetText(startPos, endPos, "Private charges £") != string.Empty))
                return;
            decimal.TryParse(GetText(startPos, endPos, "Private charges £"), out result);
            rentServiceLetter.PrivateTotal = result;
        }

        private string GetPropertyReference(string documentSection)
        {
            string propertyReference = string.Empty;
            
            try
            {
                string[] linesArray = documentSection.ToString().Split('\n');

                var propRefIndex = System.Array.FindIndex(linesArray, s => s.ToUpper().Contains("PROPERTY REFERENCE"));

                if (propRefIndex != -1)
                {
                    var propRefLine = linesArray[propRefIndex];
                    var propRefLineStartPos = propRefLine.LastIndexOf(":") + 1;

                    propertyReference = propRefLine?.Substring(propRefLineStartPos, propRefLine.Length - (propRefLineStartPos)).ToString().Trim();
                }

                if (propertyReference == "LH_00444_36")
                {
                    WriteToDebug(documentSection, propertyReference);
                }

                if (propertyReference == "200758")
                {
                    WriteToDebug(documentSection, propertyReference);
                }

                rentServiceLetter.PropertyRef = propertyReference;
            }
            catch (Exception ex)
            {
                throw;
            }

            return propertyReference;
        }

        /// <summary>
        /// Brittle code but needs must !
        /// </summary>
        /// <param name="documentSection"></param>
        private bool GetServiceCharges(string documentSection)
        {
            string[] serviceChargesLinesArray = documentSection.ToString().Split('\n');

            var serviceChargeLinePositionIndex = System.Array.FindIndex(serviceChargesLinesArray, s => s.Trim().Contains("Service charges"));

            // if the next character after "Service charges is not a linefeed, then this is a non-service charge section 
            // ie:Service charges are payments made by residents for services that are provided by Guinness 
            // A Valid Service charge section is:
            // Service charges
            // Communal Electricity £0.00 £11.50 £0.00
            var serviceChargeSearchLine = serviceChargesLinesArray[serviceChargeLinePositionIndex].ToString();

            if (serviceChargeSearchLine.ToString().Length > "Service charges".Length)
            {
                return false;
            }

            try
            {
                for (int serviceChargeLineCounter = serviceChargeLinePositionIndex + 1; serviceChargeLineCounter < serviceChargesLinesArray.Length; serviceChargeLineCounter++)
                {
                    var serviceCharge = new ServiceCharge();

                    //read in first line of service charges
                    var serviceChargeLine = serviceChargesLinesArray[serviceChargeLineCounter];

                    // if first character of serviceChargeLine !=£ AND serviceChargeLine CONTAINS £ 
                    // Communal Lighting Bulbs £0.00 £0.00 £26.00
                    // or
                    // Cleaning £0.00 £0.00 £210.04
                    try
                    {
                        if (!serviceChargeLine.StartsWith("£") && serviceChargeLine.Contains("£"))
                        {
                            // find first occurence of £
                            var firstPoundPos = serviceChargeLine.IndexOf('£');

                            // Get the Description
                            var servicechargeDescription = serviceChargeLine.Substring(0, firstPoundPos - 1);

                            serviceCharge.Description = servicechargeDescription;

                            // get the charges
                            var serviceChargeValues = serviceChargeLine.Substring(servicechargeDescription.Length, serviceChargeLine.Length - firstPoundPos+1).Trim();
                            var serviceChargeValuesArray = serviceChargeValues.Split(' ');

                            serviceCharge.AreaEstimatedCost = Extensions.TryParseDecimal(serviceChargeValuesArray[0]);
                            serviceCharge.YourEstimatedCost = Extensions.TryParseDecimal(serviceChargeValuesArray[1]);                       
                            serviceCharge.ActualCost = Extensions.TryParseDecimal(serviceChargeValuesArray[2]);

                            rentServiceLetter.ServiceCharges.Add(serviceCharge);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }

                    try
                    {
                        // if first character of serviceChargeLine != £ AND serviceChargeLine does NOT CONTAIN £
                        // Door Entry
                        // £0.00 £0.00 £11.52
                        if (!serviceChargeLine.StartsWith("£") && !serviceChargeLine.Contains("£") 
                            && !serviceChargeLine.Contains("Management fee") && !serviceChargeLine.Contains("Total Charges") 
                            && !serviceChargeLine.Contains("Surplus Deficit") && !serviceChargeLine.Contains("Sinking funds")
                            && !serviceChargeLine.Contains("Sinking Fund") && !serviceChargeLine.Contains("Sink Fund") && !serviceChargeLine.Contains("Equipment"))
                        {
                            serviceCharge.Description = serviceChargeLine.Trim();

                            // goto next line which should contain the service charge values
                            serviceChargeLineCounter++;

                            var serviceChargeNextLine = serviceChargesLinesArray[serviceChargeLineCounter];
                            // get the charges
                            var serviceChargeValuesArray = serviceChargeNextLine.Split(' ');
                            serviceCharge.AreaEstimatedCost = Extensions.TryParseDecimal(serviceChargeValuesArray[0]);
                            serviceCharge.YourEstimatedCost = Extensions.TryParseDecimal(serviceChargeValuesArray[1]);
                            serviceCharge.ActualCost = Extensions.TryParseDecimal(serviceChargeValuesArray[2]);

                            rentServiceLetter.ServiceCharges.Add(serviceCharge);

                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }


                    // line contains "Management fee" then end of Service charge section so quit
                    if (serviceChargeLine.Contains("Total Charges") || serviceChargeLine.Contains("Management fee") 
                        || serviceChargeLine.Contains("Fixed Management Fee") || serviceChargeLine.Contains("Sinking funds"))
                    {
                        break;
                    }

                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return true;
        }

        private string GetText(int startPos, int endPos, string item)
        {
            string str = string.Empty;
            int startIndex = lines.ToString().IndexOf(item, startPos) + item.Length;
            if (startIndex < endPos)
                str = lines.ToString().Substring(startIndex, lines.ToString().IndexOf('\n', startIndex) - startIndex);
            return str;
        }

        private void WriteLetterToSQL()
        {
            DAL.ExecReturnNoData(string.Format("insert into Validate.RaSQLHeaderLetters(PropertyRef, RentsTotal, ServicesTotal, PrivateTotal, RentServiceTotal, HeaderDate, StatementDate, RaSQLLetterID) values('{0}', {1}, {2}, {3}, {4}, '{5}', '{6}', {7})", rentServiceLetter.PropertyRef, rentServiceLetter.RentsTotal, rentServiceLetter.ServicesTotal, rentServiceLetter.PrivateTotal, rentServiceLetter.RentServiceTotals, rentServiceLetter.PaymentFrom, rentServiceLetter.StatementDate, RaSQLLetterID), ConfigurationManager.ConnectionStrings[ConnectionName].ConnectionString, false);
            foreach (ServiceCharge serviceCharge in rentServiceLetter.ServiceCharges)
                DAL.ExecReturnNoData(string.Format("insert into Validate.RaSQLServicesLetter(PropertyRef, Service, AreaEstimatedCost, YourEstimatedCost, RaSQLLetterID) values('{0}', '{1}', {2}, {3}, {4})", rentServiceLetter.PropertyRef, serviceCharge.Description, serviceCharge.AreaEstimatedCost, serviceCharge.YourEstimatedCost, RaSQLLetterID), ConfigurationManager.ConnectionStrings[ConnectionName].ConnectionString, false);
        }

        public void CreateRaSQLLetter(string batchNo, string financialYear)
        {
            //RaSQLLetterID = DAL.ExecReturnInt(string.Format("insert into Validate.RaSQLLetters(BatchNo, FilePath, FileName, DatabaseName,FinancialYear) values ({0}, '{1}', '{2}', '{3}', {4}); select SCOPE_IDENTITY();", batchNo, Path.GetDirectoryName(FilePath), Path.GetFileName(FilePath), ConnectionName, financialYear), ConfigurationManager.ConnectionStrings[ConnectionName].ConnectionString, false);
        }
    }
}
