using System.ComponentModel.DataAnnotations;

namespace StorageApi.DTOs
{
    public class CreateProductDto
    {
        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [Range(0, 1000)]
        public int Price { get; set; }

        [Required]
        public required string Category { get; set; }

        [Required]
        public required string Shelf { get; set; }

        [Range(0, 499)]
        public int Count { get; set; }

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
    }
}
