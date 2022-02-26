using EAScraperConnector.Dtos;

namespace EAScraperConnector.Interfaces
{
    public interface IAuditWrapper
    {
        public Task SaveToDB(Audit audit);
    }
}
