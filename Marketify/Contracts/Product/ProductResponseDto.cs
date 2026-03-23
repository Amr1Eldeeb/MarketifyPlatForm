namespace Marketify.Contracts.Product
{
    public record ProductResponseDto(
        int Id,
        string Name,
        string Description,
        decimal Price,
        int StockQuantity,
        int CategoryId,
        List<string> ImageUrls,
        List<string> Sizes
    );
}