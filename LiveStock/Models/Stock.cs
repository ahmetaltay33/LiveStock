namespace LiveStock.Models
{
    public class Stock
    {
        public string Symbol { get; set; }
        public decimal OpeningPrice { get; set; }
        public decimal Price { get; set; }
        public decimal PercentChange { get; set; }
    }
}