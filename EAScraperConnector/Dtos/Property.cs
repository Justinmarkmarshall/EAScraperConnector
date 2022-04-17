using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EAScraperConnector.Dtos
{
    [Table("Properties")]
    public class Property
    {
        [Key]
        public int Id { get; set; }
        public string Description { get; set; } = String.Empty;
        public double Price { get; set; } = 0F;
        public string Area { get; set; } = String.Empty;
        public string Link { get; set; } = String.Empty;
        public int Deposit { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        public string Images { get; set; } = String.Empty;
        public string Postcode { get; set; } = String.Empty;

        public int Site { get; set; }

        public DateTime DateListed { get; set; }

    }
}
