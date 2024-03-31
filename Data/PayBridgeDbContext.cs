using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PayBridgeAPI.Models.MainModels;
using PayBridgeAPI.Models.Transcations;
using PayBridgeAPI.Models.User;

namespace PayBridgeAPI.Data
{
    public class PayBridgeDbContext : IdentityDbContext
    {
        public PayBridgeDbContext(DbContextOptions<PayBridgeDbContext> options) : base(options)
        {
            
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Bank> Banks { get; set; }
        public DbSet<BankCard> BankCards { get; set; }
        public DbSet<CorporateBankAccount> CorporateBankAccounts { get; set; }
        public DbSet<PersonalBankAccount> PersonalBankAccounts { get; set; }
        public DbSet<CorporateAccountHolder> CorporateAccountHolders { get; set; }
        public DbSet<PersonalAccountHolder> PersonalAccountHolders { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<UserToUserTransaction> UserToUserTransactions { get; set; }
        public DbSet<CompanyBankAsset> CompanyBankAssets { get; set; }
        public DbSet<CompanyToUserTransaction> CompanyToUserTransactions { get; set; }
        public DbSet<UserToCompanyTransaction> UserToCompanyTransactions { get; set; }
        public DbSet<CompanyToCompanyTransaction> CompanyToCompanyTransactions { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<PersonalBankAccount>().HasMany(e => e.BankCards).WithOne(e => e.Account).HasForeignKey(e => e.BankCardId).IsRequired();
            builder.Entity<BankCard>().HasOne(e => e.Account).WithMany(e => e.BankCards).HasForeignKey(e => e.BankCardId).IsRequired();
            builder.Entity<Bank>().HasMany(e => e.PersonalBankAccounts).WithOne(e => e.Bank).HasForeignKey(e => e.BankId).IsRequired();
            builder.Entity<PersonalBankAccount>().HasOne(e => e.Bank).WithMany(e => e.PersonalBankAccounts).HasForeignKey(e => e.BankId).IsRequired();
            builder.Entity<Bank>().HasMany(e => e.CorporateBankAccounts).WithOne(e => e.Bank).HasForeignKey(e => e.BankId).IsRequired();
            builder.Entity<CorporateBankAccount>().HasOne(e => e.Bank).WithMany(e => e.CorporateBankAccounts).HasForeignKey(e => e.BankId).IsRequired();
            builder.Entity<CorporateBankAccount>().HasMany(e => e.BankAssets).WithOne(e => e.CorporateAccount).HasForeignKey(e => e.CorporateAccountId).IsRequired();
            builder.Entity<CompanyBankAsset>().HasOne(e => e.CorporateAccount).WithMany(e => e.BankAssets).HasForeignKey(e => e.CorporateAccountId).IsRequired();
        }
    }
}
