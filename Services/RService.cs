using System.Collections.Generic;
using System.Threading.Tasks;
using OPPE.R.Models.DB;
using OPPE.R.Models.DB.Repository;
using OPPE.R.Models.Params;
using OPPE.R.Models.DataTransferObjects;

namespace OPPE.R.Services
{
    public class RService : IRService
    {
        private readonly IOppeDBService _oppeDBService;
        private readonly IRUtilService _utilService;
        private readonly IExtractToFile _extractToFile;
        public RService(IOppeDBService oppeDBService, IRUtilService utilService, IExtractToFile extractToFile)
        {
            _extractToFile = extractToFile;
            _utilService = utilService;
            _oppeDBService = oppeDBService;
        }

        public async Task<IndicatorDim> GetIndicatorDim(int indicatorId)
        {
            var indicatorDim = await _oppeDBService.GetIndicatorDim(indicatorId);

            return indicatorDim;
        }

        public async Task<IEnumerable<PValueDTO>> CalculatePValue(BusinessKey businessKey)
        {
            var phys = await _oppeDBService.GetPhysicians(businessKey);
            var pValueFacts = new List<PValueDTO>();

            foreach (var phy in phys)
            {
                businessKey.PayrollId = phy.PayrollId;
                await _oppeDBService.DeletePValueFacts(businessKey);
                var pValueFact = await _utilService.GeneratePValueFactRecord(businessKey);
                if (pValueFact != null)
                {
                    pValueFacts.Add(pValueFact);
                }
            }

            return pValueFacts;
        }


        public async Task<int> SavePValues(IEnumerable<PValueDTO> pValueDTOs)
        {
            return await _oppeDBService.SavePValueFacts(pValueDTOs);
        }

        public async Task<int> SaveToExcelFile(IEnumerable<PValueDTO> pValueDTO)
        {
            return await _extractToFile.SaveToExcelFile(pValueDTO);
        }

    }
}