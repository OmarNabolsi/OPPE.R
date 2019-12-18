namespace OPPE.R.Models.Params
{
    public class CycleParam
    {
        public string CycleId { get; set; }
        public SubGroupParam[] SubGroups { get; set; }
        public int? ToFile { get; set; }
    }
}