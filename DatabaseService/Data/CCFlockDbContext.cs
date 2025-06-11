using System;
using System.Collections.Generic;
using DatabaseService.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseService.Data;

public partial class CCFlockDbContext : DbContext
{
    public CCFlockDbContext()
    {
    }

    public CCFlockDbContext(DbContextOptions<CCFlockDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Securityqa> Securityqas { get; set; }

    public virtual DbSet<Stockdatum> Stockdata { get; set; }

    public virtual DbSet<Useraccount> Useraccounts { get; set; }

    public virtual DbSet<Userfinance> Userfinances { get; set; }

    public virtual DbSet<Userprofile> Userprofiles { get; set; }

    public virtual DbSet<Usersportfolio> Usersportfolios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5439;Database=ccflockdb;Username=shubh;Password=8342AdksnAKe");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Securityqa>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("securityqa_pkey");

            entity.ToTable("securityqa");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Securityhasharray).HasColumnName("securityhasharray");
            entity.Property(e => e.Securitykey).HasColumnName("securitykey");
        });

        modelBuilder.Entity<Stockdatum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("stockdata_pkey");

            entity.ToTable("stockdata");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Currentvalue).HasColumnName("currentvalue");
            entity.Property(e => e.Dailychange).HasColumnName("dailychange");
            entity.Property(e => e.Historicvalues)
                .HasDefaultValueSql("'{}'::double precision[]")
                .HasColumnName("historicvalues");
            entity.Property(e => e.Name).HasColumnName("name");
        });

        modelBuilder.Entity<Useraccount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("useraccounts_pkey");

            entity.ToTable("useraccounts");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Birthday)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("birthday");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Passwordhash).HasColumnName("passwordhash");
            entity.Property(e => e.Passwordkey).HasColumnName("passwordkey");
            entity.Property(e => e.Securityqaid).HasColumnName("securityqaid");
            entity.Property(e => e.Username).HasColumnName("username");

            entity.HasOne(d => d.Securityqa).WithMany(p => p.Useraccounts)
                .HasForeignKey(d => d.Securityqaid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("useraccounts_securityqaid_fkey");
        });

        modelBuilder.Entity<Userfinance>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("userfinances_pkey");

            entity.ToTable("userfinances");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Annualcashflush)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("annualcashflush");
            entity.Property(e => e.Liquidcash)
                .HasDefaultValueSql("50000.00")
                .HasColumnName("liquidcash");
            entity.Property(e => e.Stockportfolioid).HasColumnName("stockportfolioid");
            entity.Property(e => e.Stockportfoliovalue).HasColumnName("stockportfoliovalue");

            entity.HasOne(d => d.Stockportfolio).WithMany(p => p.Userfinances)
                .HasForeignKey(d => d.Stockportfolioid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("userfinances_stockportfolioid_fkey");
        });

        modelBuilder.Entity<Userprofile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("userprofiles_pkey");

            entity.ToTable("userprofiles");

            entity.HasIndex(e => e.Accountid, "userprofiles_accountid_key").IsUnique();

            entity.HasIndex(e => e.Financeid, "userprofiles_financeid_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Accountid).HasColumnName("accountid");
            entity.Property(e => e.Bio)
                .HasDefaultValueSql("'Not Added'::text")
                .HasColumnName("bio");
            entity.Property(e => e.Financeid).HasColumnName("financeid");
            entity.Property(e => e.Firstname)
                .HasDefaultValueSql("'Not Added'::text")
                .HasColumnName("firstname");
            entity.Property(e => e.Lastname)
                .HasDefaultValueSql("'Not Added'::text")
                .HasColumnName("lastname");
            entity.Property(e => e.Personallinks)
                .HasDefaultValueSql("ARRAY['Not Added'::text]")
                .HasColumnName("personallinks");
            entity.Property(e => e.Profilepicturepath)
                .HasDefaultValueSql("'assets/defaultUserPic.jpg'::text")
                .HasColumnName("profilepicturepath");
            entity.Property(e => e.Resumefilepath).HasColumnName("resumefilepath");

            entity.HasOne(d => d.Account).WithOne(p => p.Userprofile)
                .HasForeignKey<Userprofile>(d => d.Accountid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("userprofiles_accountid_fkey");

            entity.HasOne(d => d.Finance).WithOne(p => p.Userprofile)
                .HasForeignKey<Userprofile>(d => d.Financeid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("userprofiles_financeid_fkey");
        });

        modelBuilder.Entity<Usersportfolio>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("usersportfolio_pkey");

            entity.ToTable("usersportfolio");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("gen_random_uuid()")
                .HasColumnName("id");
            entity.Property(e => e.Value)
                .HasDefaultValueSql("0.00")
                .HasColumnName("value");

            entity.HasMany(d => d.Stocks).WithMany(p => p.Portfolios)
                .UsingEntity<Dictionary<string, object>>(
                    "Portfoliostock",
                    r => r.HasOne<Stockdatum>().WithMany()
                        .HasForeignKey("Stockid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("portfoliostocks_stockid_fkey"),
                    l => l.HasOne<Usersportfolio>().WithMany()
                        .HasForeignKey("Portfolioid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("portfoliostocks_portfolioid_fkey"),
                    j =>
                    {
                        j.HasKey("Portfolioid", "Stockid").HasName("portfoliostocks_pkey");
                        j.ToTable("portfoliostocks");
                        j.IndexerProperty<Guid>("Portfolioid").HasColumnName("portfolioid");
                        j.IndexerProperty<Guid>("Stockid").HasColumnName("stockid");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
