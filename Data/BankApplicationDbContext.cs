using BankMvc.Models.Entity;
using Microsoft.EntityFrameworkCore;


public class BankApplicationDbContext : DbContext
{

    public BankApplicationDbContext(DbContextOptions<BankApplicationDbContext> options)
            : base(options)
    {
    }
    public DbSet<User> Users { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasKey(u => u.UserId);

        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId);
            entity.Property(e => e.AccountNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Balance).HasColumnType("decimal(18,2)");
            entity.HasIndex(e => e.AccountNumber).IsUnique();
        });

        modelBuilder.Entity<Transaction>()
            .HasKey(t => t.TransactionId);

    }
}


