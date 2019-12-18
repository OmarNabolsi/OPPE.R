
using System.Collections.Generic;
using System.Threading.Tasks;
using OPPE.R.Models.Params;
using OPPE.R.Models.DB;
using OPPE.R.Models.DataTransferObjects;

namespace OPPE.R.Services
{
    public interface IRService
    {
        Task<IndicatorDim> GetIndicatorDim(int indicatorId);
        Task<IEnumerable<PValueDTO>> CalculatePValue(BusinessKey businessKey);
        Task<int> SavePValues(IEnumerable<PValueDTO> pValueDTOs);
        Task<int> SaveToExcelFile(IEnumerable<PValueDTO> pValueDTO);
    }
}