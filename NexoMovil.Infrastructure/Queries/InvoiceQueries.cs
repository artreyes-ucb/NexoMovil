namespace NexoMovil.Infrastructure.Queries
{
    public static class InvoiceQueries
    {
        public const string GetById = @"
            SELECT i.Id AS InvoiceId,
                   i.InvoiceNumber,
                   DATE_FORMAT(i.IssuedAt, '%d-%m-%Y %H:%i:%s') AS IssuedAt,
                   s.Id AS SaleId,
                   DATE_FORMAT(s.Date, '%d-%m-%Y %H:%i:%s') AS SaleDate,
                   s.ProductId, p.Brand, p.Model, p.Price AS UnitPrice,
                   s.Quantity, s.TotalAmount,
                   s.UserId,
                   CONCAT(u.FirstName, ' ', u.LastName) AS SellerName,
                   u.Email AS SellerEmail
            FROM invoices i
            INNER JOIN sales s ON i.SaleId = s.Id
            INNER JOIN products p ON s.ProductId = p.Id
            INNER JOIN users u ON s.UserId = u.Id
            WHERE i.Id = @Id;";

        public const string GetBySaleId = @"
            SELECT i.Id AS InvoiceId,
                   i.InvoiceNumber,
                   DATE_FORMAT(i.IssuedAt, '%d-%m-%Y %H:%i:%s') AS IssuedAt,
                   s.Id AS SaleId,
                   DATE_FORMAT(s.Date, '%d-%m-%Y %H:%i:%s') AS SaleDate,
                   s.ProductId, p.Brand, p.Model, p.Price AS UnitPrice,
                   s.Quantity, s.TotalAmount,
                   s.UserId,
                   CONCAT(u.FirstName, ' ', u.LastName) AS SellerName,
                   u.Email AS SellerEmail
            FROM invoices i
            INNER JOIN sales s ON i.SaleId = s.Id
            INNER JOIN products p ON s.ProductId = p.Id
            INNER JOIN users u ON s.UserId = u.Id
            WHERE i.SaleId = @SaleId;";
    }
}
