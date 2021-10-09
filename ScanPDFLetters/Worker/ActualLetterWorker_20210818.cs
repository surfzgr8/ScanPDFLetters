using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DataAccessLayer.Classes;
using ScanPDFLetters.Model.Actual;

namespace ScanPDFLetters.Worker
{
    internal class ActualLetterWorker_20210818
    {
        private static readonly Regex AllNumberRegEx = new Regex("^[0-9]*$");
        private static readonly Regex PropertyReferenceRegEx = new Regex(@"^[A-Z]*_[0-9]*(_[0-9]*[A-Z]*)?$");
        private static readonly Regex AllNumberLengthOneToSevenRegEx = new Regex(@"^\d{1,7}\n.*");
        //private static readonly Regex AllNumberDotNegativeSignRegEx = new Regex(@"^-?\d*(\.\d+)?$");
        private static readonly Regex AllNumberDotNegativeSignRegEx = new Regex(@"^\(?-?\d*(\.\d+)?\)?$");
        private static readonly Regex AllNumberDotNegativeSignWithBracketRegEx = new Regex(@"^\(-?\d*(\.\d+)?\)$");
        private static readonly Regex OneOrMultipleNumberRegEx = new Regex(@"\d+");
        private static readonly Regex NumberWith2DigitalPlaceRegEx = new Regex(@"\d+.\d{2}");

        private static readonly int MaxLineSearchForPropertyRef = 10;

        private class DataBaseTableName
        {
            public static readonly string ActualLetter = "Validate.RaSQLActualLetter";
            public static readonly string ActualLetterHeader = "Validate.RaSQLActualLetterHeader";
            public static readonly string ActualLetterServiceCharge = "Validate.RaSQLActualLetterServiceCharge";
        }

        private class ServiceChargeType
        {
            public static readonly string ServiceCharge = "Service Charge";
            public static readonly string SinkingFund = "Sinking Fund";
        }


        private StringBuilder Lines { get; }
        private Label PdfStatus { get; }
        private string ConnectionName { get; }
        private string FilePath { get; }

        public ActualLetterWorker_20210818(StringBuilder sb, Label pdfStatus, string connectionName, string filePath)
        {
            Lines = sb;
            PdfStatus = pdfStatus;
            ConnectionName = connectionName;
            FilePath = filePath;
        }

        public void ProcessLetters(string batchNo, string financialYear)
        {
            int? actualLetterId = null;

            try
            {
                if (actualLetterId == 55)
                {
                    string zippy = "hello";
                }
                actualLetterId = DAL.ExecReturnInt(
                    $"insert into {DataBaseTableName.ActualLetter} (BatchNo, FilePath, FileName, DatabaseName, FinancialYear) " +
                    $"values ({batchNo}, '{Path.GetDirectoryName(FilePath)}', '{Path.GetFileName(FilePath)}', '{ConnectionName}', {financialYear}); select SCOPE_IDENTITY();",
                    ConfigurationManager.ConnectionStrings[ConnectionName].ConnectionString);
                UpdatePdfStatus($"New letter [{actualLetterId}] created.");

                var letterHeaders = new List<ActualLetterHeader>();
                var pdfLetters = Lines.ToString().Split(new[] { "Your payment reference\n" }, StringSplitOptions.None);

                UpdatePdfStatus($"Total letter headers of [{pdfLetters.Length}] detected.");
                for (var i = 0; i < pdfLetters.Length; i++)
                {
                    //if (i == 5327)
                    //{
                    //    MessageBox.Show("i = " + i.ToString());
                    //}
                    UpdatePdfStatus($"Processing [{i + 1} out of {pdfLetters.Length}]...");
                    var pdfLetter = pdfLetters[i];
                    if (!AllNumberLengthOneToSevenRegEx.IsMatch(pdfLetter))
                    {
                        continue;
                    }

                    var letterHeader = ProcessLetterHeader(pdfLetter, actualLetterId.Value);
                    letterHeaders.Add(letterHeader);
                }

                InsertLetterToSql(letterHeaders);
            }
            catch (Exception)
            {
                // Rollback
                if (actualLetterId.HasValue)
                {
                    MessageBox.Show("Letter ID = " + actualLetterId.ToString());
                    MessageBox.Show("pdf status = " + PdfStatus.ToString());

                    UpdatePdfStatus($"Rolling back inserted letter with ID = [{actualLetterId}].");
                    DAL.ExecReturnNoData($@"
                        delete from {DataBaseTableName.ActualLetterServiceCharge} where RaSQLActualLetterID = {actualLetterId}
                        delete from {DataBaseTableName.ActualLetterHeader} where RaSQLActualLetterID = {actualLetterId}
                        delete from {DataBaseTableName.ActualLetter} where RaSQLActualLetterID = {actualLetterId}",
                        ConfigurationManager.ConnectionStrings[ConnectionName].ConnectionString);

                }
                throw;
            }
        }

        private ActualLetterHeader ProcessLetterHeader(string pdfLetter, int actualLetterId)
        {
            var lines = pdfLetter.Split(new[] { "\n" }, StringSplitOptions.None);
            var letterHeader = new ActualLetterHeader(actualLetterId);
            for (var index = 0; index < lines.Length; index++)
            {
                var currentLine = lines[index];
                if (index == 0)
                {
                    letterHeader.PaymentRef = lines[0];
                }
                else if (currentLine == "Your Service Charges")
                {
                    letterHeader.HeaderDate = lines[index - 1];
                }
                else if (currentLine.StartsWith("Dear"))
                {
                    letterHeader.CustomerName = currentLine.Replace("Dear", string.Empty).Replace("'", "''").TrimStart();
                }
                else if (currentLine == "Actual costs for the year" || currentLine == "Estimated costs for the year")
                {
                    index++;
                    letterHeader.ServiceChargeStatementDate = lines[index];
                }
                else if (currentLine.Contains("Our reference:"))
                {
                    var lineSearchForPropertyRef = 1;

                    while (lineSearchForPropertyRef <= MaxLineSearchForPropertyRef)
                    {
                        var potentialPropRefLine = lines[index + lineSearchForPropertyRef];
                        if (AllNumberRegEx.IsMatch(potentialPropRefLine) || PropertyReferenceRegEx.IsMatch(potentialPropRefLine))
                        {
                            letterHeader.PropertyRef = potentialPropRefLine;
                            index += lineSearchForPropertyRef;
                            break;
                        }

                        //Removed code above as property reference is no longer always a number
                        //letterHeader.PropertyRef = potentialPropRefLine;
                        //index += lineSearchForPropertyRef;
                        

                        lineSearchForPropertyRef++;
                    }

                    if (string.IsNullOrWhiteSpace(letterHeader.PropertyRef))
                    {
                        throw new ArgumentNullException($"Unable to read property ref on letter with payment ref=[{letterHeader.PaymentRef}]");
                    }
                }
                else if (currentLine == "£ £ £")
                {
                    letterHeader.ServiceCharges = ConvertServiceCharges(actualLetterId, letterHeader.PropertyRef, index, lines, out index);
                }
                else if (currentLine.StartsWith("Total Charges"))
                {
                    ConvertTotalCharges(currentLine, out var areaActualTotal, out var yourActualTotal, out var originalEstimateTotal);
                    letterHeader.AreaActualCostTotal = areaActualTotal;
                    letterHeader.YourActualCostTotal = yourActualTotal;
                    letterHeader.YourOriginalEstimateTotal = originalEstimateTotal;
                }
                else if (currentLine.StartsWith("This means the original estimate"))
                {
                    var isTooLow = currentLine.Contains("too low");
                    var delta = currentLine.Split('£').First(s => AllNumberDotNegativeSignRegEx.IsMatch(s));
                    if (delta != null)
                    {
                        letterHeader.OriginalEstimationDelta = ConvertMoneyStringToNumber(delta) * (isTooLow ? -1m : 1m);
                    }
                }
                else if (currentLine.Contains("SINKING FUND BREAKDOWN"))
                {
                    index++;
                    letterHeader.SinkingFundDate = lines[index].Replace("FOR THE YEAR", string.Empty).TrimStart();
                }
                else if (currentLine.EndsWith("£ £"))
                {
                    letterHeader.SinkingFunds = ConvertSinkingFund(actualLetterId, letterHeader.PropertyRef, index, lines, out index);
                }
                else if (currentLine.StartsWith("Total Contribution"))
                {
                    letterHeader.SinkingFundContributionTotal = ConvertSinkingTotal(currentLine);
                }
                else if (currentLine.StartsWith("Total Interest"))
                {
                    letterHeader.SinkingFundInterestTotal = ConvertSinkingTotal(currentLine);
                }
                else if (currentLine.StartsWith("Total Replacement costs"))
                {
                    letterHeader.SinkingFundReplacementCostTotal = ConvertSinkingTotal(currentLine);
                }
                else if (currentLine.StartsWith("Total sinking fund"))
                {
                    letterHeader.SinkingFundTotal = ConvertSinkingTotal(currentLine);
                }
            }

            return letterHeader;
        }

        private List<ActualLetterServiceCharge> ConvertSinkingFund(int letterId, string propertyRef, int currentIndex, string[] lines, out int newIndex)
        {
            currentIndex++;
            var currentLine = lines[currentIndex];
            var currentCategory = string.Empty;
            var serviceCharges = new List<ActualLetterServiceCharge>();
            while (!currentLine.StartsWith("Total Contribution") && !currentLine.Contains("\u0002") && !!currentLine.Contains("\u0080"))
            {
                //if (currentLine.StartsWith("CDP") || currentLine.StartsWith("2018/19"))
                if (currentLine.StartsWith("CDP"))
                {

                }
                else if (!NumberWith2DigitalPlaceRegEx.IsMatch(currentLine))
                {
                    currentCategory = currentLine;
                }
                else
                {
                    var sinkingFunds = ConvertToActualLetterServiceChange(letterId, propertyRef, currentCategory, currentLine, ServiceChargeType.SinkingFund);
                    serviceCharges.Add(sinkingFunds);
                }
                currentIndex++;
                currentLine = lines[currentIndex];
            }

            newIndex = currentIndex - 1;
            return serviceCharges;
        }

        private decimal? ConvertSinkingTotal(string line)
        {
            var moneyString = line.Split(' ').Single(l => AllNumberDotNegativeSignRegEx.IsMatch(l));
            return ConvertMoneyStringToNumber(moneyString);
        }

        private List<ActualLetterServiceCharge> ConvertServiceCharges(int letterId, string propertyRef, int currentIndex, string[] lines, out int newIndex)
        {
            currentIndex++;
            var currentLine = lines[currentIndex];
            var currentCategory = string.Empty;
            var serviceCharges = new List<ActualLetterServiceCharge>();
            while (!currentLine.StartsWith("Total Charges"))
            {
                if (currentLine.StartsWith("CDP") || currentLine.StartsWith("2018/19"))
                {

                }
                else
                {
                    if (!NumberWith2DigitalPlaceRegEx.IsMatch(currentLine))
                    {
                        currentCategory = currentLine;
                    }
                    else
                    {
                        var serviceCharge = ConvertToActualLetterServiceChange(letterId, propertyRef, currentCategory, currentLine, ServiceChargeType.ServiceCharge);
                        serviceCharges.Add(serviceCharge);
                    }
                }
                currentIndex++;
                currentLine = lines[currentIndex];
            }

            newIndex = currentIndex - 1;
            return serviceCharges;
        }

        private void ConvertTotalCharges(string inputLine, out decimal? totalArea, out decimal? totalActual, out decimal? totalEstimation)
        {
            totalArea = totalActual = totalEstimation = null;

            //int x = 100;

            //if (inputLine.IndexOf(@"\)") != -1)
            //{
            //    x = 99;
            //}

            var lineDetails = inputLine.Split(' ');
            var numbers = lineDetails.Where(l => AllNumberDotNegativeSignRegEx.IsMatch(l)).ToArray();            

            ConvertThreeColumnsNumber(numbers, out totalArea, out totalActual, out totalEstimation);
        }

        private void InsertLetterToSql(List<ActualLetterHeader> letterHeaders)
        {
            for (var i = 0; i < letterHeaders.Count; i++)
            {
                UpdatePdfStatus($"Creating [{i + 1} out of {letterHeaders.Count}] letter headers to Database...");
                var header = letterHeaders[i];
                var actualLetterHeaderId = DAL.ExecReturnInt(
                    $@"insert into {DataBaseTableName.ActualLetterHeader} 
                        ([RaSQLActualLetterID]
                       ,[PropertyRef]
                       ,[PaymentRef]
                       ,[CustomerName]
                       ,[AreaActualCostTotal]
                       ,[YourActualCostTotal]
                       ,[YourOriginalEstimateTotal]
                       ,[OriginalEstimationDelta]
                       ,[SinkingFundDate]
                       ,[SinkingFundContributionTotal]
                       ,[SinkingFundInterestTotal]
                       ,[SinkingFundReplacementCostTotal]
                       ,[SinkingFundTotal]
                       ,[ServiceChargeStatementDate]
                       ,[HeaderDate])
                    values (
                    {header.ActualLetterId},
                    '{header.PropertyRef}',
                    '{header.PaymentRef}',
                    '{header.CustomerName}',
                    {ConvertDecimalToSqlString(header.AreaActualCostTotal)},
                    {ConvertDecimalToSqlString(header.YourActualCostTotal)},
                    {ConvertDecimalToSqlString(header.YourOriginalEstimateTotal)},
                    {ConvertDecimalToSqlString(header.OriginalEstimationDelta)},
                    '{header.SinkingFundDate}',
                    {ConvertDecimalToSqlString(header.SinkingFundContributionTotal)},
                    {ConvertDecimalToSqlString(header.SinkingFundInterestTotal)},
                    {ConvertDecimalToSqlString(header.SinkingFundReplacementCostTotal)},
                    {ConvertDecimalToSqlString(header.SinkingFundTotal)},
                    '{header.ServiceChargeStatementDate}',
                    '{header.HeaderDate}'
                    );
                    select SCOPE_IDENTITY();",
                    ConfigurationManager.ConnectionStrings[ConnectionName].ConnectionString);

                foreach (var serviceCharge in header.ServiceCharges.Concat(header.SinkingFunds))
                {
                    DAL.ExecReturnInt(
                        $@"insert into {DataBaseTableName.ActualLetterServiceCharge} 
                                ([RaSQLActualLetterID]
                               ,[RaSQLActualLetterHeaderID]
                               ,[PropertyRef]
                               ,[ServiceName]
                               ,[ServiceCategory]
                               ,[AreaActualCost]
                               ,[YourActualCost]
                               ,[YourOriginalEstimate]
                               ,[ServiceChargeType])
                        VALUES (
                        {serviceCharge.ActualLetterId},
                        {actualLetterHeaderId},
                        '{serviceCharge.PropertyRef}',
                        '{serviceCharge.ServiceName}',
                        '{serviceCharge.ServiceCategory}',
                        {ConvertDecimalToSqlString(serviceCharge.AreaActualCost)},
                        {ConvertDecimalToSqlString(serviceCharge.YourActualCost)},
                        {ConvertDecimalToSqlString(serviceCharge.YourOriginalEstimate)},
                        '{serviceCharge.ServiceChargeType}'
                        )
                        select SCOPE_IDENTITY();",
                        ConfigurationManager.ConnectionStrings[ConnectionName].ConnectionString);
                }
            }
        }

        private ActualLetterServiceCharge ConvertToActualLetterServiceChange(int letterId, string propertyRef, string category, string inputLine, string serviceChargeType)
        {
            var lineDetails = inputLine.Split(' ').ToList();

            // Some Service names begin with a number.
            string nameHasNumber = string.Empty;
            if (lineDetails.Count > 1 && AllNumberDotNegativeSignRegEx.IsMatch(lineDetails[0]) && !AllNumberDotNegativeSignRegEx.IsMatch(lineDetails[1]))
            {
                nameHasNumber = lineDetails[0] + " ";
                lineDetails.Remove(lineDetails[0]);
            }

            var numbers = lineDetails.Where(l => (OneOrMultipleNumberRegEx.IsMatch(l) || l == string.Empty) && (AllNumberDotNegativeSignRegEx.IsMatch(l) || AllNumberDotNegativeSignWithBracketRegEx.IsMatch(l))).ToArray();
            foreach (var n in numbers)
            {
                lineDetails.Remove(n);
            }
            var texts = nameHasNumber + string.Join(" ", lineDetails);
            var result = new ActualLetterServiceCharge
            {
                ActualLetterId = letterId,
                PropertyRef = propertyRef,
                ServiceCategory = category,
                ServiceName = texts,
                ServiceChargeType = serviceChargeType
            };

            if (numbers.Length > 0)
            {
                ConvertThreeColumnsNumber(numbers, out var areaActual, out var yourActual, out var yourEstimate);

                result.AreaActualCost = areaActual;
                result.YourActualCost = yourActual;
                result.YourOriginalEstimate = yourEstimate;
            }

            return result;
        }

        private void ConvertThreeColumnsNumber(string[] numberAndEmptyArray, out decimal? firstNumber, out decimal? secondNumber, out decimal? thirdNumber)
        {
            var convertedArray = numberAndEmptyArray;
            if (string.IsNullOrWhiteSpace(numberAndEmptyArray.Last()))
            {
                convertedArray = numberAndEmptyArray.Take(numberAndEmptyArray.Length - 1).ToArray();
            }

            while (convertedArray.Length < 3)
            {
                convertedArray = new[] { string.Empty }.Concat(convertedArray).ToArray();
            }

            firstNumber = ConvertMoneyStringToNumber(convertedArray[0]);
            secondNumber = ConvertMoneyStringToNumber(convertedArray[1]);
            thirdNumber = ConvertMoneyStringToNumber(convertedArray[2]);
        }

        private decimal? ConvertMoneyStringToNumber(string money)
        {
            if (string.IsNullOrWhiteSpace(money))
            {
                return null;
            }

            if (AllNumberDotNegativeSignWithBracketRegEx.IsMatch(money))
            {
                return -1m * decimal.Parse(money.Replace("(", string.Empty).Replace(")", string.Empty));
            }

            return decimal.Parse(money);
        }

        private string ConvertDecimalToSqlString(decimal? number)
        {
            return number.HasValue ? number.Value.ToString(CultureInfo.InvariantCulture) : "NULL";
        }

        private void UpdatePdfStatus(string message)
        {
            PdfStatus.Text = message;
            PdfStatus.Refresh();
        }
    }
}
