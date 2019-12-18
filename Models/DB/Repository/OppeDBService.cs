using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OPPE.R.Models.DataTransferObjects;
using OPPE.R.Models.Params;

namespace OPPE.R.Models.DB.Repository
{
    public class OppeDBService : IOppeDBService
    {
        private readonly OPPEDbContext _context;
        public OppeDBService(OPPEDbContext context)
        {
            _context = context;
        }

        public async Task<IndicatorDim> GetIndicatorDim(int indicatorId)
        {
            var indicatorDim = await _context.Indicators.FirstOrDefaultAsync(i => i.IndicatorId == indicatorId);

            return indicatorDim;
        }

        public async Task<IEnumerable<PhysiciansDTO>> GetPhysicians(BusinessKey businessKey)
        {
            var phys = await _context.RawDataFacts
                                        .Where(r => 
                                            (r.CycleId == businessKey.CycleId)
                                            && (r.SubGroupId == businessKey.SubGroupId)
                                            && (r.IndicatorId == businessKey.IndicatorId))
                                        .Select(r => new {PayrollId = r.PayrollId, OppeKey = r.CycleId})
                                        .ToListAsync();

            var p = phys.GroupBy(x => x.PayrollId).Select(x => new { PayrollId = x.Key, C = x.Count()}).ToArray();

            var physArr = new List<PhysiciansDTO>();

            for (int x = 0; x < p.Length; x++)
            {
                if(p[x].C >= 5)
                {
                    var physDto = new PhysiciansDTO
                    {
                        PayrollId = p[x].PayrollId
                    };
                    physArr.Add(physDto);
                }
            }

            return physArr;
        }

        public async Task<IEnumerable<TestDTO>> GetRawDataFactList(BusinessKey businessKey)
        {
            var physData = await _context.RawDataFacts
                                            .Where(p => (p.CycleId == businessKey.CycleId) 
                                                && (p.SubGroupId == businessKey.SubGroupId)
                                                && (p.IndicatorId == businessKey.IndicatorId)
                                                && (p.PayrollId == businessKey.PayrollId)
                                            ).ToListAsync();

            var peersData = await _context.RawDataFacts
                                            .Where(p => (p.CycleId == businessKey.CycleId)
                                                && (p.SubGroupId == businessKey.SubGroupId)
                                                && (p.IndicatorId == businessKey.IndicatorId)
                                                && (p.PayrollId != businessKey.PayrollId)
                                            ).ToListAsync();
            peersData.ForEach(p => p.PayrollId = "Peers");

            physData.AddRange(peersData);
            
            var c = peersData.Count;

            var tTestDTOList = new List<TestDTO>();


            if(c >= 5 )
            {
                physData.ForEach(p => {
                    var tTestDTO = new TestDTO
                    {
                        Name = p.PayrollId,
                        Value = p.Numerator
                    };
                    tTestDTOList.Add(tTestDTO);
                });
            }
            return tTestDTOList;
        }

        private async Task<int> GetLastPValueFactBatch()
        {
            int lastPValueFactBatch = 0;
            var lastPValueFact = await _context.PValueFacts.MaxAsync(p => (int?)p.Batch);
            if(lastPValueFact != null)
            {
                lastPValueFactBatch = (int)lastPValueFact;
            } 

            return lastPValueFactBatch;
        }

        public async Task<int> SavePValueFacts(IEnumerable<PValueDTO> pValueDTOs)
        {
            int numOfRec = 0;
            var batchdate = DateTime.Now;
            var lastPValueFact = await GetLastPValueFactBatch();


            foreach (var item in pValueDTOs)
            {
                var newRecord = new PValueFact
                {
                    Batch = lastPValueFact+1,
                    DateTime = batchdate,
                    CycleId = item.CycleId,
                    IndicatorId = item.IndicatorId,
                    SubGroupId = item.SubGroupId,
                    PayrollId = item.PayrollId,
                    PhysicianCount = item.PhysicianCount,
                    PhysicianSum = item.PhysicianSum,
                    PhysicianMean = item.PhysicianMean,
                    PeersCount = item.PeersCount,
                    PeersSum = item.PeersSum,
                    PeersMean = item.PeersMean,
                    LeveneValue = item.LeveneValue,
                    PValueUnequalVariance = item.PValueUnequalVariance,
                    PValueEqualVariance = item.PValueEqualVariance,
                    ChiRatio = item.ChiRatio,
                    PValue = item.PValue
                };

                await _context.PValueFacts.AddAsync(newRecord);
                numOfRec += await _context.SaveChangesAsync();
            }


            return numOfRec;
        }

        public async Task<int> DeletePValueFacts(BusinessKey businessKey)
        {
            var recordsToBeDeleted = await _context.PValueFacts.Where(p => 
                                                                        (p.CycleId == businessKey.CycleId)
                                                                        && (p.SubGroupId == businessKey.SubGroupId)
                                                                        && (p.IndicatorId == businessKey.IndicatorId)
                                                                        && (p.PayrollId == businessKey.PayrollId))
                                                                .ToListAsync();
            var deleted = 0;

            foreach (var rawDataFact in recordsToBeDeleted)
            {
                _context.PValueFacts.Remove(rawDataFact);
            }

            deleted = await _context.SaveChangesAsync();

            return deleted;
        }
    }
}