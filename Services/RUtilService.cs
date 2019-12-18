using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OPPE.R.Models.DataTransferObjects;
using OPPE.R.Models.DB.Repository;
using OPPE.R.Models.Params;
using RDotNet;

namespace OPPE.R.Services
{
    public class RUtilService : IRUtilService
    {
        private readonly IOppeDBService _oppeDBService;
        private readonly REngine _engine;
        public RUtilService(IOppeDBService oppeDBService)
        {
            _oppeDBService = oppeDBService;
            REngine.SetEnvironmentVariables();
            _engine = REngine.GetInstance();
            _engine.Initialize();
        }

        public async Task<PValueDTO> GeneratePValueFactRecord(BusinessKey businessKey)
        {
            float leveneValue = 0;
            float pValueUnequalVariance = 0;
            float pValueEqualVariance = 0;
            float chiRatio = 0;
            float pValue = 0;

            var phys = await _oppeDBService.GetRawDataFactList(businessKey);

            var physCount = phys.Count(p => p.Name != "Peers");
            float physSum = (float)phys.Where(p => p.Name != "Peers").Sum(p => p.Value);
            float physMean = (float)(physCount == 0 ? 0 : physSum / physCount);

            var peersCount = phys.Count(p => p.Name == "Peers");
            float peersSum = (float)phys.Where(p => p.Name == "Peers").Sum(p => p.Value);
            float peersMean = (float)(peersCount == 0 ? 0 : peersSum / peersCount);

            if (businessKey.isTTest)
            {
                LeveneTestDTO leveneTestDTO = await CreateGroupsForLevene(businessKey);
                if (leveneTestDTO == null) return null;
                leveneValue = CalculateLeveneValue(leveneTestDTO);

                TTestGroupsDTO tTestGroupsDto = await CreateGroupsForTTest(businessKey);
                if (tTestGroupsDto == null) return null;
                pValueUnequalVariance = CalculateUnequalVariance(tTestGroupsDto);
                pValueEqualVariance = CalculateEqualVariance(tTestGroupsDto);

                pValue = leveneValue > 0.05 ? pValueEqualVariance : pValueUnequalVariance;

                if (float.IsNaN(leveneValue) || float.IsNaN(pValueUnequalVariance) || float.IsNaN(pValueEqualVariance)) return null;
            }
            else
            {
                if (phys.Any())
                {
                    CalculateChiForPhys(phys, out chiRatio, out pValue);
                }
            }


            PValueDTO pValueDTO = new PValueDTO
            {
                CycleId = businessKey.CycleId,
                SubGroupId = (int)businessKey.SubGroupId,
                IndicatorId = (int)businessKey.IndicatorId,
                PayrollId = businessKey.PayrollId,
                PhysicianCount = physCount,
                PhysicianSum = physSum,
                PhysicianMean = physMean,
                PeersCount = peersCount,
                PeersSum = peersSum,
                PeersMean = peersMean,
                LeveneValue = leveneValue,
                PValueUnequalVariance = pValueUnequalVariance,
                PValueEqualVariance = pValueEqualVariance,
                ChiRatio = chiRatio,
                PValue = pValue
            };

            return pValueDTO;
        }


        private void CalculateChiForPhys(IEnumerable<TestDTO> phys, out float chiRatio, out float pValue)
        {
            var physYes = phys.Where(p => p.Name != "Peers").Count(y => y.Value == 1);
            var physNo = phys.Where(p => p.Name != "Peers").Count(n => n.Value != 1);
            var peersYes = phys.Where(p => p.Name == "Peers").Count(y => y.Value == 1);
            var peersNo = phys.Where(p => p.Name == "Peers").Count(n => n.Value != 1);

            var total = physYes + physNo + peersYes + peersNo;
            chiRatio = (float)physYes / (float)total;
            pValue = 0;

            if (chiRatio < 0.05)
            {
                pValue = (float)_engine.Evaluate($"fisher.test(rbind(c({physYes},{physNo}),c({peersYes},{peersNo})))$p.value").AsNumeric().First();
            }
            else
            {
                pValue = (float)_engine.Evaluate($"chisq.test(rbind(c({physYes},{physNo}),c({peersYes},{peersNo})), correct = FALSE)$p.value").AsNumeric().First();
            }
        }

        private float CalculateEqualVariance(TTestGroupsDTO tTestGroupsDto)
        {
            NumericVector group1 = tTestGroupsDto.Group1;
            NumericVector group2 = tTestGroupsDto.Group2;

            _engine.SetSymbol("group1", group1);
            _engine.SetSymbol("group2", group2);

            GenericVector testResult = _engine.Evaluate("t.test(group1, group2, var.equal=TRUE)").AsList();

            float p = (float)testResult["p.value"].AsNumeric().First();

            return p;
        }

        private float CalculateLeveneValue(LeveneTestDTO leveneTestDTO)
        {
            CharacterVector namesV = leveneTestDTO.Names;
            NumericVector valuesV = leveneTestDTO.Values;

            _engine.Evaluate("library(car)");
            _engine.SetSymbol("namesV", namesV);
            _engine.SetSymbol("valuesV", valuesV);

            GenericVector testResult = _engine.Evaluate("levene.test(valuesV, namesV, center=mean)").AsList();
            float p = (float)testResult[2].AsNumeric().First();

            return p;
        }

        private float CalculateUnequalVariance(TTestGroupsDTO tTestGroupsDto)
        {
            NumericVector group1 = tTestGroupsDto.Group1;
            NumericVector group2 = tTestGroupsDto.Group2;

            _engine.SetSymbol("group1", group1);
            _engine.SetSymbol("group2", group2);

            GenericVector testResult = _engine.Evaluate("t.test(group1, group2, var.equal=FALSE)").AsList();

            float p = (float)testResult["p.value"].AsNumeric().First();

            return p;
        }

        private async Task<TTestGroupsDTO> CreateGroupsForTTest(BusinessKey businessKey)
        {
            var rawData = await _oppeDBService.GetRawDataFactList(businessKey);

            if (!rawData.Any()) return null;

            var grp1 = Array.ConvertAll(rawData.Where(r => r.Name == "Peers").Select(r => r.Value).ToArray(), x => (double)x);
            var grp2 = Array.ConvertAll(rawData.Where(r => r.Name != "Peers").Select(r => r.Value).ToArray(), x => (double)x);

            var tTestGroupsDto = new TTestGroupsDTO
            {
                Group1 = _engine.CreateNumericVector(grp1),
                Group2 = _engine.CreateNumericVector(grp2)
            };

            return tTestGroupsDto;
        }

        private async Task<LeveneTestDTO> CreateGroupsForLevene(BusinessKey businessKey)
        {
            var rawData = await _oppeDBService.GetRawDataFactList(businessKey);

            if (!rawData.Any()) return null;

            var namesV = rawData.Select(l => l.Name).ToArray();
            var values = Array.ConvertAll(rawData.Select(l => l.Value).ToArray(), x => (double)x);

            LeveneTestDTO leveneTestDTO = new LeveneTestDTO
            {
                Names = _engine.CreateCharacterVector(namesV),
                Values = _engine.CreateNumericVector(values)
            };

            return leveneTestDTO;
        }
    }
}