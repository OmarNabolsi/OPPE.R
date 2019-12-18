using Microsoft.EntityFrameworkCore;

namespace OPPE.R.Models.DB
{
    public class OPPEDbContext : DbContext
    {
        public OPPEDbContext(DbContextOptions<OPPEDbContext> options) : base(options) { }

        public DbSet<IndicatorDim> Indicators { get; set; }
        public DbSet<SubGroupDim> SubGroups { get; set; }
        public DbSet<RawDataFact> RawDataFacts { get; set; }
        public DbSet<PValueFact> PValueFacts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)  
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("FullAccess");
            modelBuilder.Entity<IndicatorDim>(entity => {
                entity.ToTable("OppeIndicatorDimX");
                entity.Property(e => e.IndicatorId).HasColumnName("OppeIndicatorId");
                entity.Property(e => e.StatisticalTestId).HasColumnName("StatisticalTestID");
                entity.HasNoKey();
            });

            modelBuilder.Entity<SubGroupDim>(entity => {
                entity.ToTable("OppePhysiciansGroupsAndSubGroupsDimX");
                entity.Property(e => e.SubGroupId).HasColumnName("OppePhysiciansGroupsAndSubGroupsId");
                entity.Property(e => e.SubGroupName).HasColumnName("SubGroupName");
                entity.HasNoKey();
            });

            modelBuilder.Entity<RawDataFact>(entity => {
                entity.ToTable("OppeRawDataFactX");
                entity.Property(e => e.CycleId).HasColumnName("OppeCycleID");
                entity.Property(e => e.IndicatorId).HasColumnName("OppeIndicatorID");
                entity.Property(e => e.SubGroupId).HasColumnName("OppePhysicianSubGroupID");
                entity.Property(e => e.PayrollId).HasColumnName("PayrollID");
                entity.Property(e => e.Numerator).HasColumnName("NumeratorValue").HasColumnType("numeric(18,2)");
                entity.Property(e => e.Denominator).HasColumnName("DenominatorValue").HasColumnType("numeric(18,2)");
                entity.HasNoKey();
            });

            modelBuilder.Entity<PValueFact>(entity => {
                entity.ToTable("OppePValueFactX_TMP");
                entity.Property(e => e.Id).HasColumnName("rt_Auto");
                entity.Property(e => e.Batch).HasColumnName("rt_Batch");
                entity.Property(e => e.DateTime).HasColumnName("rt_DateTime");
                entity.Property(e => e.CycleId).HasColumnName("OppeCycleID");
                entity.Property(e => e.IndicatorId).HasColumnName("OppeIndicatorID");
                entity.Property(e => e.SubGroupId).HasColumnName("OppePhysicianSubGroupID");
                entity.Property(e => e.PayrollId).HasColumnName("PayrollID").HasColumnType("nvarchar(50)");
                entity.Property(e => e.PhysicianSum).HasColumnName("PhysicianSum").HasColumnType("float");
                entity.Property(e => e.PhysicianCount).HasColumnName("PhysicianCount").HasColumnType("float");
                entity.Property(e => e.PhysicianMean).HasColumnName("PhysicianMean").HasColumnType("float");
                entity.Property(e => e.PeersSum).HasColumnName("PeersSum").HasColumnType("float");
                entity.Property(e => e.PeersCount).HasColumnName("PeersCount").HasColumnType("float");
                entity.Property(e => e.PeersMean).HasColumnName("PeersMean").HasColumnType("float");
                entity.Property(e => e.PValueEqualVariance).HasColumnName("PValueEqualVariance").HasColumnType("float");
                entity.Property(e => e.PValueUnequalVariance).HasColumnName("PValueUnequalVariance").HasColumnType("float");
                entity.Property(e => e.LeveneValue).HasColumnName("LeveneValue").HasColumnType("float");
                entity.Property(e => e.ChiRatio).HasColumnName("ChiRatio").HasColumnType("float");
                entity.Property(e => e.PValue).HasColumnName("PValue").HasColumnType("float");
            });
            
        }
    }
}