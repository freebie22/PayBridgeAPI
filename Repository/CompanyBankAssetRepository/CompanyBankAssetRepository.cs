using PayBridgeAPI.Data;
using PayBridgeAPI.Models.MainModels;
using PayBridgeAPI.Repository.MainRepo;

namespace PayBridgeAPI.Repository.CompanyBankAssetRepository
{
    public class CompanyBankAssetRepository(PayBridgeDbContext context) : Repository<CompanyBankAsset>(context), ICompanyBankAssetRepository
    {
        private readonly PayBridgeDbContext _context = context;

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CompanyBankAsset companyBankAsset)
        {
            _context.Update(companyBankAsset);
            await SaveChangesAsync();
        }
    }
}
