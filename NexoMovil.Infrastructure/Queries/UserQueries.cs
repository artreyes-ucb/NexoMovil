namespace NexoMovil.Infrastructure.Queries
{
    public static class UserQueries
    {
        public const string GetAll = @"
            SELECT Id, FirstName, LastName, Email, Telephone, Role, IsActive
            FROM users
            ORDER BY Id;";

        public const string GetById = @"
            SELECT Id, FirstName, LastName, Email, Telephone, Role, IsActive
            FROM users
            WHERE Id = @Id;";
    }
}
