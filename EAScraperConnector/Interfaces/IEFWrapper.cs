using EAScraperConnector.Dtos;

namespace EAScraperConnector.Interfaces
{
    public interface IEFWrapper
    {
        public Task SaveToDB(List<Property> properties);

        public Task<List<Property>> GetFromDB();

        public Task SaveToDB(Audit audit);

        public Task<List<Property>> GetByPrice(double price);
    }
}
