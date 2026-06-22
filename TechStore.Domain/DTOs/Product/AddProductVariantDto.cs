namespace TechStore.Domain.DTOs.Product
{
    public class AddProductVariantDto
    {
        public string Name { get; set; } = string.Empty;
        public string SKU { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int StockQuantity { get; set; }
        public bool IsDefault { get; set; }
        public List<int> OptionValueIds { get; set; } = new();
    }

}
