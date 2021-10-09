using System.Configuration;
using System.IO;
using System.Text;
using System.Windows.Forms;
using DataAccessLayer.Classes;
using ScanPDFLetters.Model;
using System.Collections.Generic;

namespace ScanPDFLetters.Worker
{
    internal class EstimationLetterWorker
    {
        private const string StartPage1 = "Your service charge – important information";
        private const string StartPage2 = "Your rent and service charge – important information";
        private const string StartPage3 = "Your rent – important information";
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
        
        private RentAndServiceLetter rentServiceLetter;

        public Label PDFStatus { get; set; }

        public string ConnectionName { get; set; }

        public string FilePath { get; set; }

        public StringBuilder lines { get; set; }

        private int RaSQLLetterID { get; set; }

        public EstimationLetterWorker()
        {
        }

        public EstimationLetterWorker(StringBuilder sb)
        {
            lines = sb;
            rentServiceLetter = new RentAndServiceLetter();
        }

        public void Clear()
        {
            lines.Clear();
            rentServiceLetter.Clear();
        }

        public void ProcessLetters(List<string> lstStartLettter, List<string> lstEndLetter)
        {
            int startLetter = GetStartLetter(0);
            int endLetter = GetEndLetter(0);
            //int startLetter = FindMatch(lstStartLettter);
            //int endLetter = FindMatch(lstEndLetter);

//            int num = 1;
//            while (startLetter != -1 && endLetter != -1)
//            {
                //PDFStatus.Text = string.Format("PDF Count :{0}", num);
                //PDFStatus.Refresh();
                //rentServiceLetter.Clear();
                GetPaymentFrom(startLetter);
                GetStatementDate(startLetter);
                GetTotalCharges(startLetter, endLetter);
                GetPropertyReference(startLetter);
                GetServiceCharges(startLetter);
                WriteLetterToSQL();
            //startLetter = GetStartLetter(startLetter + 1);
            //endLetter = GetEndLetter(endLetter + 1);
            //++num;
            //            }      
        }

        
      //  private void GetServiceCharges()
        //{
           // if (lines.ToString().IndexOf("£ £") != 1)
           // {
              //  int serviceCharge = lines.ToString().IndexOf("£ £");

               // while ()
          //  }
      //  }


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


        private int GetStartLetter(int startPos)
        {
            int num = lines.ToString().IndexOf("Your service charge – important information", startPos);
            if (num == -1)
            {
                

                if (lines.ToString().IndexOf("Your rent and service charge – important information", startPos) != -1)
                    num = lines.ToString().IndexOf("Your rent and service charge – important information", startPos);
                else if (lines.ToString().IndexOf("Your rent – important information", startPos) != -1)
                    num = lines.ToString().IndexOf("Your rent – important information", startPos);
                else if (lines.ToString().IndexOf("Privacy Notice", startPos) != -1)
                    num = lines.ToString().IndexOf("Privacy Notice", startPos);
            }
            return num;
        }

        private int GetEndLetter(int startPos)
        {
            int startIndex = -1;

            if (lines.ToString().IndexOf("Your weekly contributions towards the annual estimate will be £", startPos) != -1 || lines.ToString().IndexOf("Your monthly contributions towards the annual estimate will be £", startPos) != -1)
            {
                if (lines.ToString().IndexOf("Your monthly contributions towards the annual estimate will be £", startPos) == -1)
                    //  Weekly
                    startIndex = lines.ToString().IndexOf('\n', lines.ToString().IndexOf("Your weekly contributions towards the annual estimate will be £", startPos)) + 1;
                else if (lines.ToString().IndexOf("Your weekly contributions towards the annual estimate will be £", startPos) == -1)
                    // Monthly
                    startIndex = lines.ToString().IndexOf('\n', lines.ToString().IndexOf("Your monthly contributions towards the annual estimate will be £", startPos)) + 1;
                else if (lines.ToString().IndexOf("Your weekly contributions towards the annual estimate will be £", startPos) < lines.ToString().IndexOf("Your monthly contributions towards the annual estimate will be £", startPos))
                    // Both but weekly is closer
                    startIndex = lines.ToString().IndexOf('\n', lines.ToString().IndexOf("Your weekly contributions towards the annual estimate will be £", startPos)) + 1;
                else if (lines.ToString().IndexOf("Your monthly contributions towards the annual estimate will be £", startPos) < lines.ToString().IndexOf("Your weekly contributions towards the annual estimate will be £", startPos))
                    // Both but monthly is closer
                    startIndex = lines.ToString().IndexOf('\n', lines.ToString().IndexOf("Your monthly contributions towards the annual estimate will be £", startPos)) + 1;
            }
           else  if (lines.ToString().IndexOf("Your weekly charge will be £", startPos) != -1 || lines.ToString().IndexOf("Your monthly charge will be £", startPos) != -1)
            {
                if (lines.ToString().IndexOf("Your monthly charge will be £", startPos) == -1)
                    //  Weekly
                    startIndex = lines.ToString().IndexOf('\n', lines.ToString().IndexOf("Your weekly charge will be £", startPos)) + 1;
                else if (lines.ToString().IndexOf("Your weekly charge will be £", startPos) == -1)
                    // Monthly
                    startIndex = lines.ToString().IndexOf('\n', lines.ToString().IndexOf("Your monthly charge will be £", startPos)) + 1;
                else if (lines.ToString().IndexOf("Your weekly charge will be £", startPos) < lines.ToString().IndexOf("Your monthly charge will be £", startPos))
                    // Both but weekly is closer
                    startIndex = lines.ToString().IndexOf('\n', lines.ToString().IndexOf("Your weekly charge will be £", startPos)) + 1;
                else if (lines.ToString().IndexOf("Your monthly charge will be £", startPos) < lines.ToString().IndexOf("Your weekly charge will be £", startPos))
                    // Both but monthly is closer
                    startIndex = lines.ToString().IndexOf('\n', lines.ToString().IndexOf("Your monthly charge will be £", startPos)) + 1;
            }
            else if (lines.ToString().IndexOf("This will be taken off your estimate for") != -1)
                startIndex = lines.ToString().IndexOf('\n', lines.ToString().IndexOf("This will be taken off your estimate for", startPos)) + 1;

            //int startIndex = lines.ToString().IndexOf("Your weeklyy contributions towards the annual estimate will be £", startPos);
            //if (startIndex == -1)
            //{
            //    startIndex = lines.ToString().IndexOf("Your monthly contributions towards the annual estimate will be £", startPos);
            //    if (startIndex != -1)
            //    {
            //        startIndex = lines.ToString().IndexOf('\n', startIndex) + 1;
            //    }
            //    else if (lines.ToString().IndexOf("Your weekly charge will be £", startPos) != -1 && lines.ToString().IndexOf("Your weekly charge will be £", startPos) < startIndex)
            //        startIndex = lines.ToString().IndexOf('\n', lines.ToString().IndexOf("Your weekly charge will be £", startPos)) + 1;
            //    else if (lines.ToString().IndexOf("Your monthly charge will be £", startPos) != -1 && lines.ToString().IndexOf("Your monthly charge will be £", startPos) < startIndex)
            //        startIndex = lines.ToString().IndexOf('\n', lines.ToString().IndexOf("Your monthly charge will be £", startPos)) + 1;
            //}
            //else if (lines.ToString().IndexOf("Your monthly contributions towards the annual estimate will be £", startPos) != -1 && lines.ToString().IndexOf("Your monthly contributions towards the annual estimate will be £", startPos) < startIndex)
            //    startIndex = lines.ToString().IndexOf('\n', lines.ToString().IndexOf("Your monthly contributions towards the annual estimate will be £", startPos)) + 1;

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

        private void GetPropertyReference(int startPos)
        {
            //int startIndex = lines.ToString().IndexOf('\n', lines.ToString().IndexOf("Property reference:", startPos) + "Property reference:".Length + 1) + 1;
            var startIndex = lines.ToString().IndexOf("Property reference: ");
            var propRefLen = lines.ToString().IndexOf("\nYour Service Charge statement") - (startIndex + 20);
            var propref = lines.ToString().Substring(startIndex + 20, propRefLen);
            rentServiceLetter.PropertyRef = propref;// lines.ToString().Substring(startIndex, lines.ToString().IndexOf("\nYour Service Charge statement", startIndex) - startIndex);
        }

        private void GetServiceCharges(int startPos)
        {
            int startIndex = lines.ToString().IndexOf("£ £", startPos) + "£ £".Length + 1;
            bool flag = false;
            string[] strArray = null;

            if (lines.ToString().Substring(startIndex).IndexOf("Your weekly contributions towards the annual estimate will be £") != -1 || lines.ToString().Substring(startIndex).IndexOf("Your monthly contributions towards the annual estimate will be £") != -1)
            {
                if (lines.ToString().Substring(startIndex).IndexOf("Your monthly contributions towards the annual estimate will be £") == -1)
                    // Must be weekly
                    strArray = lines.ToString().Substring(startIndex, lines.ToString().IndexOf("Your weekly contributions towards the annual estimate will be £", startIndex) - startIndex).Split('\n');
                else if (lines.ToString().Substring(startIndex).IndexOf("Your weekly contributions towards the annual estimate will be £") == -1)
                    // Must be monthly
                    strArray = lines.ToString().Substring(startIndex, lines.ToString().IndexOf("Your monthly contributions towards the annual estimate will be £", startIndex) - startIndex).Split('\n');
                else if(lines.ToString().Substring(startIndex).IndexOf("Your weekly contributions towards the annual estimate will be £") < lines.ToString().Substring(startIndex).IndexOf("Your monthly contributions towards the annual estimate will be £"))
                    // Both exist but weekly is closer
                    strArray = lines.ToString().Substring(startIndex, lines.ToString().IndexOf("Your weekly contributions towards the annual estimate will be £", startIndex) - startIndex).Split('\n');
                else if (lines.ToString().Substring(startIndex).IndexOf("Your monthly contributions towards the annual estimate will be £") < lines.ToString().Substring(startIndex).IndexOf("Your weekly contributions towards the annual estimate will be £"))
                    // Both exist but monthly is closer
                    strArray = lines.ToString().Substring(startIndex, lines.ToString().IndexOf("Your monthly contributions towards the annual estimate will be £", startIndex) - startIndex).Split('\n');
            }
            else if (lines.ToString().Substring(startIndex).IndexOf("Your weekly charge will be £") != -1 || lines.ToString().Substring(startIndex).IndexOf("Your monthly charge will be £") != -1)
            {
                if (lines.ToString().Substring(startIndex).IndexOf("Your monthly charge will be £") == -1)
                    // Must be weekly
                    strArray = lines.ToString().Substring(startIndex, lines.ToString().IndexOf("Your weekly charge will be £", startIndex) - startIndex).Split('\n');
                else if (lines.ToString().Substring(startIndex).IndexOf("Your weekly charge will be £") == -1)
                    // Must be monthly
                    strArray = lines.ToString().Substring(startIndex, lines.ToString().IndexOf("Your monthly charge will be £", startIndex) - startIndex).Split('\n');
                else if (lines.ToString().Substring(startIndex).IndexOf("Your weekly charge will be £") < lines.ToString().Substring(startIndex).IndexOf("Your monthly charge will be £"))
                    // Both exist but weekly is closer
                    strArray = lines.ToString().Substring(startIndex, lines.ToString().IndexOf("Your weekly charge will be £", startIndex) - startIndex).Split('\n');
                else if (lines.ToString().Substring(startIndex).IndexOf("Your monthly charge will be £") < lines.ToString().Substring(startIndex).IndexOf("Your weekly charge will be £"))
                    // Both exist but monthly is closer
                    strArray = lines.ToString().Substring(startIndex, lines.ToString().IndexOf("Your monthly charge will be £", startIndex) - startIndex).Split('\n');

            }
            else if (lines.ToString().Substring(startIndex).IndexOf("This will be taken off your estimate for") != -1 )
                strArray = lines.ToString().Substring(startIndex, lines.ToString().IndexOf("This will be taken off your estimate for", startIndex) - startIndex).Split('\n');

            //if (lines.ToString().Substring(startIndex).IndexOf("Your weekly contributions towards the annual estimate will be £") == -1)
            //    strArray = lines.ToString().Substring(startIndex, lines.ToString().IndexOf("Your monthly contributions towards the annual estimate will be £", startIndex) - startIndex).Split('\n');
            //else if (lines.ToString().Substring(startIndex).IndexOf("Your monthly contributions towards the annual estimate will be £") == -1 || lines.ToString().Substring(startIndex).IndexOf("Your weekly contributions towards the annual estimate will be £") < lines.ToString().Substring(startIndex).IndexOf("Your monthly contributions towards the annual estimate will be £"))
            //    strArray = lines.ToString().Substring(startIndex, lines.ToString().IndexOf("Your weekly contributions towards the annual estimate will be £", startIndex) - startIndex).Split('\n');
            //else if (lines.ToString().Substring(startIndex).IndexOf("Your weekly charge will be £") == -1 || lines.ToString().Substring(startIndex).IndexOf("Your weekly charge will be £") < lines.ToString().Substring(startIndex).IndexOf("Your monthly contributions towards the annual estimate will be £"))
            //    strArray = lines.ToString().Substring(startIndex, lines.ToString().IndexOf("Your weekly contributions towards the annual estimate will be £", startIndex) - startIndex).Split('\n');
            //else
            //    strArray = lines.ToString().Substring(startIndex, lines.ToString().IndexOf("Your monthly contributions towards the annual estimate will be £", startIndex) - startIndex).Split('\n');


            StringBuilder stringBuilder = new StringBuilder();
            foreach (string str in strArray)
            {
                stringBuilder.Clear();
                if (str.IndexOf("Description Estimated cost Your estimated") == -1 && !flag)
                {
                    if (str.Trim() != string.Empty && str.Length > 1)
                    {
                        stringBuilder.Append(str.Trim());
                        ServiceCharge serviceCharges = new ServiceCharge();
                        int num1 = stringBuilder.ToString().LastIndexOf(' ') + 1;
                        decimal result1;
                        if (decimal.TryParse(stringBuilder.ToString().Substring(num1).Replace("(", string.Empty).Replace(")", string.Empty), out result1))
                            serviceCharges.YourEstimatedCost = decimal.Parse(stringBuilder.ToString().Substring(num1).Replace("(", string.Empty).Replace(")", string.Empty));
                        if (stringBuilder.ToString().Substring(num1).IndexOf("(") != -1)
                            serviceCharges.YourEstimatedCost *= -1.0m;
                        stringBuilder.Clear();
                        stringBuilder.Append(str.Trim().Substring(0, num1).Trim());
                        int num2 = stringBuilder.ToString().LastIndexOf(' ') + 1;
                        if (stringBuilder.ToString().IndexOf("Management fee") == -1 && stringBuilder.ToString().IndexOf("Surplus deficit") == -1 && (stringBuilder.ToString().IndexOf("Manual Surplus Deficit") == -1)) // stringBuilder.ToString().IndexOf("Accountancy Fee") == -1  && stringBuilder.ToString().IndexOf("Buildings Insurance"
                        {
                            if (decimal.TryParse(stringBuilder.ToString().Substring(num2).Replace("(", string.Empty).Replace(")", string.Empty), out result1))
                                serviceCharges.AreaEstimatedCost = decimal.Parse(stringBuilder.ToString().Substring(num2).Replace("(", string.Empty).Replace(")", string.Empty));
                            if (stringBuilder.ToString().Substring(num2).IndexOf("(") != -1)
                                serviceCharges.AreaEstimatedCost *= -1.0m;
                        }
                        int result2;
                        serviceCharges.Description = int.TryParse(stringBuilder.ToString()[stringBuilder.ToString().Length - 1].ToString(), out result2) || stringBuilder.ToString()[stringBuilder.ToString().Length - 1].ToString() == ")" ? stringBuilder.ToString().Substring(0, num2).Trim() : stringBuilder.ToString().Trim();
                        rentServiceLetter.ServiceCharges.Add(serviceCharges);
                    }
                }
                else
                    flag = str.IndexOf("£ £") == -1;
            }
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
