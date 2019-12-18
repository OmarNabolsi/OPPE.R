using System.Threading.Tasks;
using OPPE.R.Models.DataTransferObjects;
using OPPE.R.Models.Params;

namespace OPPE.R.Services
{
    public interface IRUtilService
    {
        Task<PValueDTO> GeneratePValueFactRecord(BusinessKey businessKey);
    }
}