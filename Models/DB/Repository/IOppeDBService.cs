using System.Collections.Generic;
using System.Threading.Tasks;
using OPPE.R.Models.DataTransferObjects;
using OPPE.R.Models.Params;

namespace OPPE.R.Models.DB.Repository
{
    public interface IOppeDBService
    {
        Task<IndicatorDim> GetIndicatorDim(int indicatorId);
        Task<IEnumerable<PhysiciansDTO>> GetPhysicians(BusinessKey businessKey);
        Task<IEnumerable<TestDTO>> GetRawDataFactList(BusinessKey businessKey);
        Task<int> SavePValueFacts(IEnumerable<PValueDTO> pValueDTOs);
        Task<int> DeletePValueFacts(BusinessKey businessKey);
    }
}