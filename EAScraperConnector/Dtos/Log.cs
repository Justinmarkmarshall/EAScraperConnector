using System.ComponentModel.DataAnnotations;

namespace EAScraperConnector.Dtos
{
    public class Log
    {
        [Key]
        public int Id { get; set; }

        public string Message { get; set; } = "";
        public string? Source { get; set; }
        public string? StackTrace { get; set; }
        public string? HelpLink { get; set; }
        public string? TargetSite { get; set; }
    }
}
