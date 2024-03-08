using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PayBridgeAPI.Models.MainModels;
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
        public DbSet<CorparateBankAccount> CorparateBankAccounts { get; set; }
        public DbSet<PersonalBankAccount> PersonalBankAccounts { get; set; }
        public DbSet<CorporateAccountHolder> CorporateAccountHolders { get; set; }
        public DbSet<PersonalAccountHolder> PersonalAccountHolders { get; set; }
        public DbSet<Manager> Managers { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
