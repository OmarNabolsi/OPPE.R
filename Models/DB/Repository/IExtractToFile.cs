using System.Collections.Generic;
using System.Threading.Tasks;
using OPPE.R.Models.DataTransferObjects;

namespace OPPE.R.Models.DB.Repository
{
    public interface IExtractToFile
    {
        Task<int> SaveToExcelFile(IEnumerable<PValueDTO> pValueDTO);
    }
}