namespace OPPE.R.Models.DB
{
    public class RawDataFact
    {
        public string CycleId { get; set; }
        public int? IndicatorId { get; set; }
        public int? SubGroupId { get; set; }
        public string PayrollId { get; set; }
        public double? Numerator { get; set; }
        public double? Denominator { get; set; }
    }
}