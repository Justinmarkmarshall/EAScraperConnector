using EAScraperConnector.Models;

namespace EAScraperConnector.Interfaces
{
    public interface IZooplaScraper
    {
        public Task<IList<House>> GetProperties(string price, bool londonOnly = true, int version=1);
    }
}
