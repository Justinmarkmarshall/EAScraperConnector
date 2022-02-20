using EAScraperConnector.Models;

namespace EAScraperConnector.Interfaces
{
    public interface IExcelSaver
    {
        public void SaveToExcel(List<House> properties);
    }
}
