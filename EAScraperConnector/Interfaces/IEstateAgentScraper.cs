using EAScraperConnector.Models;

namespace EAScraperConnector.Interfaces
{
    public interface IRightMoveScraper
    {
        public Task<IList<House>> GetProperties(string price);
    }
}
