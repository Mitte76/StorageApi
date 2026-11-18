namespace StorageApi.DTOs
{
    public class ProductDto
    {
        public ProductDto(int id, string name, int price, int count)
        {
            Id = id;
            Name = name;
            Price = price;
            Count = count;
        }

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Price { get; set; }
        public int Count { get; set; }
        public int InventoryValue => Price * Count;
    }
}
