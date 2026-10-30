using Microsoft.EntityFrameworkCore;
using NexoMovil.Core.Entities;
using System.Reflection;

namespace NexoMovil.Infrastructure.Data;

public partial class NexoMovilContext : DbContext
{
    public NexoMovilContext()
    {
    }

    public NexoMovilContext(DbContextOptions<NexoMovilContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Sale> Sales { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    //    => optionsBuilder.UseMySql("server=localhost;port=3306;database=DbNexoMovil;uid=root;pwd=4607", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.46-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            Assembly.GetExecutingAssembly());
    }
}