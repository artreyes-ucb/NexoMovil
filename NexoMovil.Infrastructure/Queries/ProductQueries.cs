namespace NexoMovil.Infrastructure.Queries
{
    public static class ProductQueries
    {
        public const string GetById = @"
            SELECT Id, Brand, Model, Description, Price, Stock, CreatedAt
            FROM products
            WHERE Id = @Id;";

        public const string GetFiltered = @"
            SELECT Id, Brand, Model, Description, Price, Stock, CreatedAt
            FROM products
            WHERE (@Brand IS NULL OR Brand LIKE CONCAT('%', @Brand, '%'))
              AND (@Model IS NULL OR Model LIKE CONCAT('%', @Model, '%'))
              AND (@MaxPrice IS NULL OR Price <= @MaxPrice)
              AND (@OnlyAvailable = 0 OR Stock > 0)
            ORDER BY Id ASC;";

        public const string GetAllAvailable = @"
            SELECT Id, Brand, Model, Description, Price, Stock, CreatedAt
            FROM products
            WHERE Stock > 0
            ORDER BY Id ASC
            LIMIT @Limit;";
    }
}
