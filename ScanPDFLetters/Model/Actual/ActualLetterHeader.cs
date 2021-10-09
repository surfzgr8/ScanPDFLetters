using System.Collections.Generic;

namespace ScanPDFLetters.Model.Actual
{
    internal class ActualLetterHeader
    {
        public int? ActualLetterId { get; set; }

        public string PropertyRef { get; set; }

        public string PaymentRef { get; set; }

        public string CustomerName { get; set; }

        public decimal? AreaActualCostTotal { get; set; }

        public decimal? YourActualCostTotal { get; set; }

        public decimal? YourOriginalEstimateTotal { get; set; }

        public decimal? OriginalEstimationDelta { get; set; }

        public string SinkingFundDate { get; set; }

        public decimal? SinkingFundContributionTotal { get; set; }

        public decimal? SinkingFundInterestTotal { get; set; }

        public decimal? SinkingFundReplacementCostTotal { get; set; }

        public decimal? SinkingFundTotal { get; set; }

        public string ServiceChargeStatementDate { get; set; }

        public string HeaderDate { get; set; }

        public List<ActualLetterServiceCharge> ServiceCharges { get; set; }

        public List<ActualLetterServiceCharge> SinkingFunds { get; set; }

        public ActualLetterHeader(int letterId)
        {
            ActualLetterId = letterId;
            ServiceCharges = new List<ActualLetterServiceCharge>();
            SinkingFunds = new List<ActualLetterServiceCharge>();
        }

        public void Clear()
        {
            AreaActualCostTotal = null;
            YourActualCostTotal = null;
            YourOriginalEstimateTotal = null;
            OriginalEstimationDelta = null;
            SinkingFundContributionTotal = null;
            SinkingFundInterestTotal = null;
            SinkingFundReplacementCostTotal = null;
            SinkingFundTotal = null;

            PropertyRef = string.Empty;
            PaymentRef = string.Empty;

            CustomerName = string.Empty;
            ActualLetterId = null;

            SinkingFundDate = string.Empty;
            ServiceChargeStatementDate = string.Empty;
            HeaderDate = string.Empty;

            ServiceCharges.Clear();
            SinkingFunds.Clear();
        }
    }
}
