using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OPPE.R.Models.Params;
using OPPE.R.Models.DataTransferObjects;
using OPPE.R.Services;

namespace OPPE.R.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PValueController : ControllerBase
    {
        private readonly IRService _rService;

        public PValueController(IRService rService)
        {
            _rService = rService;
        }

        [HttpGet]
        public IActionResult TestApi()
        {
            return Ok("Success!");
        }

        [HttpPost]
        public async Task<IActionResult> GetData(CycleParam param)
        {
            var cycle = param.CycleId;
            var toFile = param.ToFile == 1;

            var pValueDTOs = new List<PValueDTO>();
            int recordsAdded = 0;

            int i = 0;
            foreach (var subGroup in param.SubGroups)
            {
                int[] indicators = new int[subGroup.Indicators.Length];
                int j = 0;
                foreach (var ind in subGroup.Indicators)
                {
                    indicators[j] = ind;
                    // 
                    var indicatorDim = await _rService.GetIndicatorDim(ind);
                    BusinessKey businessKey = new BusinessKey { };

                    switch (indicatorDim.StatisticalTestId)
                    {
                        case 1: //T-Test
                            
                            businessKey.CycleId = cycle;
                            businessKey.SubGroupId = subGroup.SubGroupId;
                            businessKey.IndicatorId = ind;
                            businessKey.isTTest = true;
                            var pValuesT = await _rService.CalculatePValue(businessKey);
                            pValueDTOs.AddRange(pValuesT);
                            break;
                        case 2: //Chi-square
                            businessKey.CycleId = cycle;
                            businessKey.SubGroupId = subGroup.SubGroupId;
                            businessKey.IndicatorId = ind;
                            businessKey.isTTest = false;
                            var pValuesC = await _rService.CalculatePValue(businessKey);
                            pValueDTOs.AddRange(pValuesC);
                            break;
                        default:
                            break;
                    }
                    j++;
                }
                i++;
            }

            
            if(toFile)
            {
                // save to file
                recordsAdded = await _rService.SaveToExcelFile(pValueDTOs);
            }
            else
            {
                // save to DB
                recordsAdded = await _rService.SavePValues(pValueDTOs);
            }

            return Ok(recordsAdded);
        }
    }
}