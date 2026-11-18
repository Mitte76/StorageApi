namespace StorageApi.DTOs
{
    public record ProductStatsDto(
        int TotalProducts,
        decimal TotalInventoryValue,
        decimal AveragePrice
    );
}
