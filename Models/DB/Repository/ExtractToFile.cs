using System.Collections.Generic;
using System.Threading.Tasks;
using OPPE.R.Models.DataTransferObjects;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using System.IO;
using OfficeOpenXml.Style;
using System.Drawing;

namespace OPPE.R.Models.DB.Repository
{
    public class ExtractToFile : IExtractToFile
    {
        public ExtractToFile(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public async Task<int> SaveToExcelFile(IEnumerable<PValueDTO> pValueDTO)
        {
            var outputDir = Directory.CreateDirectory(Configuration.GetSection("FileOutputDirectory").GetSection("WorkDir").Value);
            var newFile = new FileInfo(outputDir.FullName + @"\sample1.xlsx");

            if(newFile.Exists)
            {
                newFile.Delete();
                newFile = new FileInfo(outputDir.FullName + @"\sample1.xlsx");
            }
            await Task.Run(() => {
                using (var package = new ExcelPackage(newFile))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Test");

                    worksheet.Cells["A1"].Value = "Cycle";
                    worksheet.Cells["B1"].Value = "SubGroupId";
                    worksheet.Cells["C1"].Value = "Indicator";
                    worksheet.Cells["D1"].Value = "PayrollId";
                    worksheet.Cells["E1"].Value = "PhysicianCount";
                    worksheet.Cells["F1"].Value = "PhysicianSum";
                    worksheet.Cells["G1"].Value = "PhysicianMean";
                    worksheet.Cells["H1"].Value = "PeersCount";
                    worksheet.Cells["I1"].Value = "PeersSum";
                    worksheet.Cells["J1"].Value = "PeersMean";
                    worksheet.Cells["K1"].Value = "LeveneValue";
                    worksheet.Cells["L1"].Value = "PValueUnequalVariance";
                    worksheet.Cells["M1"].Value = "PValueEqualVariance";
                    worksheet.Cells["N1"].Value = "ChiRatio";
                    worksheet.Cells["O1"].Value = "PValue";

                    var indexOfDto = 2;

                    foreach (var dto in pValueDTO)
                    {
                        worksheet.Cells[indexOfDto, 1].Value = dto.CycleId;
                        worksheet.Cells[indexOfDto, 2].Value = dto.SubGroupId;
                        worksheet.Cells[indexOfDto, 3].Value = dto.IndicatorId;
                        worksheet.Cells[indexOfDto, 4].Value = dto.PayrollId;
                        worksheet.Cells[indexOfDto, 5].Value = dto.PhysicianCount;
                        worksheet.Cells[indexOfDto, 6].Value = dto.PhysicianSum;
                        worksheet.Cells[indexOfDto, 7].Value = dto.PhysicianMean;
                        worksheet.Cells[indexOfDto, 8].Value = dto.PeersCount;
                        worksheet.Cells[indexOfDto, 9].Value = dto.PeersSum;
                        worksheet.Cells[indexOfDto, 10].Value = dto.PeersMean;
                        worksheet.Cells[indexOfDto, 11].Value = dto.LeveneValue;
                        worksheet.Cells[indexOfDto, 12].Value = dto.PValueUnequalVariance;
                        worksheet.Cells[indexOfDto, 13].Value = dto.PValueEqualVariance;
                        worksheet.Cells[indexOfDto, 14].Value = dto.ChiRatio;
                        worksheet.Cells[indexOfDto, 15].Value = dto.PValue;

                        indexOfDto++;
                    }

                    using (var range = worksheet.Cells[1,1,1,15])
                    {
                        
                        range.Style.Font.Bold = true;
                        range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
                        range.Style.Font.Color.SetColor(Color.White);
                    }
                    
                    package.Save();
                }
            });
            
            return 1;
        }
    }
}