namespace NexoMovil.Infrastructure.Queries
{
    public static class SaleQueries
    {
        public const string GetByIdWithDetails = @"
            SELECT s.Id AS SaleId,
                   DATE_FORMAT(s.Date, '%d-%m-%Y %H:%i:%s') AS Date,
                   s.ProductId, p.Brand, p.Model, p.Price AS UnitPrice,
                   s.Quantity, s.TotalAmount,
                   s.UserId,
                   CONCAT(u.FirstName, ' ', u.LastName) AS CustomerName,
                   u.Email AS CustomerEmail
            FROM sales s
            INNER JOIN products p ON s.ProductId = p.Id
            INNER JOIN users u ON s.UserId = u.Id
            WHERE s.Id = @Id;";

        public const string GetAllWithDetails = @"
            SELECT s.Id AS SaleId,
                   DATE_FORMAT(s.Date, '%d-%m-%Y %H:%i:%s') AS Date,
                   s.ProductId, p.Brand, p.Model, p.Price AS UnitPrice,
                   s.Quantity, s.TotalAmount,
                   s.UserId,
                   CONCAT(u.FirstName, ' ', u.LastName) AS CustomerName,
                   u.Email AS CustomerEmail
            FROM sales s
            INNER JOIN products p ON s.ProductId = p.Id
            INNER JOIN users u ON s.UserId = u.Id
            ORDER BY s.Date DESC, s.Id DESC
            LIMIT @Limit;";

        public const string GetFilteredWithDetails = @"
            SELECT s.Id AS SaleId,
                   DATE_FORMAT(s.Date, '%d-%m-%Y %H:%i:%s') AS Date,
                   s.ProductId, p.Brand, p.Model, p.Price AS UnitPrice,
                   s.Quantity, s.TotalAmount,
                   s.UserId,
                   CONCAT(u.FirstName, ' ', u.LastName) AS CustomerName,
                   u.Email AS CustomerEmail
            FROM sales s
            INNER JOIN products p ON s.ProductId = p.Id
            INNER JOIN users u ON s.UserId = u.Id
            WHERE (@IsActive IS NULL OR s.IsActive = @IsActive)
              AND (@UserId IS NULL OR s.UserId = @UserId)
              AND (@ProductId IS NULL OR s.ProductId = @ProductId)
            ORDER BY s.Date DESC, s.Id DESC;";

        // RN-10: el reporte solo considera ventas activas.
        public const string GetDailyReport = @"
            SELECT s.UserId,
                   CONCAT(u.FirstName, ' ', u.LastName) AS SellerName,
                   u.Email AS SellerEmail,
                   COUNT(s.Id) AS SalesCount,
                   SUM(s.TotalAmount) AS SubTotal
            FROM sales s
            INNER JOIN users u ON s.UserId = u.Id
            WHERE s.IsActive = 1
              AND s.Date >= @Start
              AND s.Date < @End
            GROUP BY s.UserId, u.FirstName, u.LastName, u.Email
            ORDER BY SellerName ASC;";
    }
}
