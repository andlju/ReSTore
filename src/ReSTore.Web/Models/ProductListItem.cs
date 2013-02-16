namespace ReSTore.Web.Models
{
    public enum AmountUnit
    {
        Kilogram,
        Litre,
        Meter
    }

    public class ProductItem
    {
        public string Brand { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public AmountUnit AmountUnit { get; set; }
    }
}