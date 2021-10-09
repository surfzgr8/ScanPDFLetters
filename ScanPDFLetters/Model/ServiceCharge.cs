namespace ScanPDFLetters.Model
{
    public class ServiceCharge
    {
        public string Description { get; set; }

        public decimal AreaEstimatedCost { get; set; }

        public decimal YourEstimatedCost { get; set; }

        public decimal ActualCost { get; set; }
    }
}
