namespace EAScraperConnector.Models
{
    public class House
    {
        public string Description { get; set; }
        public string Price { get; set; }
        public string? Area { get; set; }
        public int MonthlyRepayments { get; set; }
        public List<string>? Images { get; set; }
        public string Link { get; set; }
        public int Deposit { get; set; }
    }
}
