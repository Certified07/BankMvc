using BankMvc.Models.Entity;
using Microsoft.EntityFrameworkCore;

namespace BankMvc.Data;

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

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.ToTable("Transactions");

            entity.HasKey(e => e.TransactionId);

            entity.Property(e => e.TransactionId)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Amount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(e => e.TransactionType)
                .HasConversion<string>() // Store enum as string in database
                .IsRequired();

            entity.Property(e => e.TransactionDate)
                .IsRequired();

            entity.Property(e => e.Description);

            entity.Property(e => e.ReferenceNumber)
                .HasMaxLength(50);

            // Configure foreign key relationship
            entity.HasOne(t => t.Account)
                .WithMany() // or WithMany(a => a.Transactions) if you have a navigation property
                .HasForeignKey(t => t.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Transaction entity


        //modelBuilder.Entity<Transaction>(entity =>
        //{

        //    entity.Property(t => t.TransactionType)
        //        .IsRequired()
        //        .HasMaxLength(50);
        //    entity.Property(t => t.Amount);
        //    entity.Property(t => t.Description)
        //        .HasMaxLength(255);
        //    entity.HasKey(t => t.TransactionId);
        //    entity.HasOne<Account>()
        //        .WithMany()
        //        .HasForeignKey(t => t.AccountId)
        //        .OnDelete(DeleteBehavior.Cascade);

        //});


    }
}


