using ClosedXML.Excel;
using EAScraperConnector.Interfaces;
using EAScraperConnector.Models;

namespace EAScraperConnector
{
    public class ExcelSaver : IExcelSaver
    {
        public void SaveToExcel(List<House> properties)
        {
            using (var workbook = new XLWorkbook())
            {
                try
                {
                    var worksheet = workbook.Worksheets.Add("Properties");
                    var currentRow = 1;
                    worksheet.Cell(currentRow, 1).Value = "Description";
                    worksheet.Cell(currentRow, 2).Value = "Price";
                    worksheet.Cell(currentRow, 3).Value = "Area";
                    worksheet.Cell(currentRow, 4).Value = "Link";
                    worksheet.Cell(currentRow, 5).Value = "MonthlyRepayments";
                    worksheet.Cell(currentRow, 6).Value = "Deposit";
                    foreach (var home in properties)
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = home.Description;
                        worksheet.Cell(currentRow, 2).Value = home.Price;
                        worksheet.Cell(currentRow, 3).Value = home.Area;
                        worksheet.Cell(currentRow, 4).Value = home.Link;
                        worksheet.Cell(currentRow, 5).Value = home.MonthlyRepayments;
                        worksheet.Cell(currentRow, 6).Value = home.Deposit;
                    }

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();

                        string projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName + "\\EAScraper\\EAScraperConnector\\EAScraperConnector\\Spreadsheet";

                        File.WriteAllBytes($"{projectDirectory}\\{DateTime.Now.Day}-{DateTime.Now.Month}-{DateTime.Now.Hour}{DateTime.Now.Minute}.csv", content);
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}