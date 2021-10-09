namespace ScanPDFLetters.Model.Actual
{
    internal class ActualLetterServiceCharge
    {
        public int? ActualLetterId { get; set; }

        public int? ActualLetterHeaderId { get; set; }

        public string PropertyRef { get; set; }

        public string ServiceName { get; set; }

        public string ServiceCategory { get; set; }


        public decimal? AreaActualCost { get; set; }

        public decimal? YourActualCost { get; set; }

        public decimal? YourOriginalEstimate { get; set; }

        public string ServiceChargeType { get; set; }
    }
}
