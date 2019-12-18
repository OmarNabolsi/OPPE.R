namespace OPPE.R.Models.DataTransferObjects
{
    public class PValueDTO
    {
        public string CycleId { get; set; }
        public int IndicatorId { get; set; }
        public int SubGroupId { get; set; }
        public string PayrollId { get; set; }
        public float PhysicianSum { get; set; }
        public float PhysicianCount { get; set; }
        public float PhysicianMean { get; set; }
        public float PeersSum { get; set; }
        public float PeersCount { get; set; }
        public float PeersMean { get; set; }
        public float PValueEqualVariance { get; set; }
        public float PValueUnequalVariance { get; set; }
        public float LeveneValue { get; set; }
        public float ChiRatio { get; set; }
        public float PValue { get; set; }
    }
}