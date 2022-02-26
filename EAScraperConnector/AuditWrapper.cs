using EAScraperConnector.Dtos;
using EAScraperConnector.Interfaces;

namespace EAScraperConnector
{
    public class AuditWrapper : IAuditWrapper
    {
        private DataContext _context;

        public AuditWrapper(DataContext context)
        {
            _context = context;
        }

        public async Task SaveToDB(Audit audit)
        {
            await _context.AddAsync(audit);
            await _context.SaveChangesAsync(); 
        }
    }
}
